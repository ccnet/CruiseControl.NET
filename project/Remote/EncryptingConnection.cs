using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Messages;
using System.Security.Cryptography;
using System.IO;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A server connection that will encrypt any transmitted data.
    /// </summary>
    public class EncryptingConnection
        : ServerConnectionBase, IServerConnection, IDisposable
    {
        #region Private fields
        private IServerConnection innerConnection;
        private byte[] cryptoKey = new byte[0];
        private byte[] cryptoIv = new byte[0];
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="EncryptingConnection"/>.
        /// </summary>
        /// <param name="innerConnection">The connection for sending messages.</param>
        public EncryptingConnection(IServerConnection innerConnection)
        {
            this.innerConnection = innerConnection;
            innerConnection.SendMessageCompleted += PassOnSendMessageCompleted;
            innerConnection.RequestSending += PassOnRequestSending;
            innerConnection.ResponseReceived += PassOnResponseReceived;
        }
        #endregion

        #region Properties
        #region Type
        /// <summary>
        /// The type of connection.
        /// </summary>
        public string Type
        {
            get { return innerConnection.Type; }
        }

        #region Address
        /// <summary>
        /// The address of the client.
        /// </summary>
        public virtual string Address
        {
            get { return innerConnection.Address; }
        }
        #endregion
        #endregion

        #region ServerName
        /// <summary>
        /// The name of the server that this connection is for.
        /// </summary>
        public string ServerName
        {
            get { return innerConnection.ServerName; }
        }
        #endregion

        #region IsBusy
        /// <summary>
        /// Is this connection busy performing an operation.
        /// </summary>
        public bool IsBusy
        {
            get { return innerConnection.IsBusy; }
        }
        #endregion
        #endregion

        #region Methods
        #region SendMessage()
        /// <summary>
        /// Sends a message to a remote server.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="request">The request to send to the server.</param>
        /// <returns>The response from the server.</returns>
        public Response SendMessage(string action, ServerRequest request)
        {
            // Make sure there is a password
            if ((cryptoKey.Length == 0) || (cryptoIv.Length == 0)) InitialisePassword();

            // Generate the encrypted request
            var encryptedRequest = new EncryptedRequest();
            encryptedRequest.Action = action;
            var crypto = new RijndaelManaged();
            crypto.Key = cryptoKey;
            crypto.IV = cryptoIv;
            encryptedRequest.EncryptedData = EncryptMessage(crypto, request.ToString());

            // Send the request
            var response = innerConnection.SendMessage("ProcessSecureRequest", encryptedRequest);
            var encryptedResponse = response as EncryptedResponse;

            // Generate the actual response
            if ((response.Result == ResponseResult.Success) && (encryptedResponse != null))
            {
                var data = DecryptMessage(crypto, encryptedResponse.EncryptedData);
                response = XmlConversionUtil.ProcessResponse(data);
            }
            return response;
        }
        #endregion

        #region SendMessageAsync()
        /// <summary>
        /// Sends a message to a remote server asychronously.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="request">The request to send to the server.</param>
        public void SendMessageAsync(string action, ServerRequest request)
        {
            innerConnection.SendMessageAsync(action, request);
        }

        /// <summary>
        /// Sends a message to a remote server asychronously.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="request">The request to send to the server.</param>
        /// <param name="userState">Any user state data.</param>
        public void SendMessageAsync(string action, ServerRequest request, object userState)
        {
            innerConnection.SendMessageAsync(action, request, userState);
        }
        #endregion

        #region CancelAsync()
        /// <summary>
        /// Cancels an asynchronous operation.
        /// </summary>
        public void CancelAsync()
        {
            innerConnection.CancelAsync();
        }

        /// <summary>
        /// Cancels an asynchronous operation.
        /// </summary>
        /// <param name="userState"></param>
        public void CancelAsync(object userState)
        {
            innerConnection.CancelAsync(userState);
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Disposes the .NET remoting client.
        /// </summary>
        public virtual void Dispose()
        {
            var disposable = innerConnection as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        #endregion
        #endregion

        #region Public events
        #region SendMessageCompleted
        /// <summary>
        /// A SendMessageAsync has completed.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> SendMessageCompleted;
        #endregion
        #endregion

        #region Private methods
        #region InitialisePassword()
        /// <summary>
        /// Initialise the password.
        /// </summary>
        private void InitialisePassword()
        {
            try
            {
                // Request the public key
                var publicKeyRequest = new ServerRequest();
                var publicKeyResponse = innerConnection.SendMessage("RetrievePublicKey", publicKeyRequest);
                if (publicKeyResponse.Result == ResponseResult.Failure)
                {
                    throw new CommunicationsException("Server does not export a public key: " + publicKeyResponse.ConcatenateErrors());
                }

                // Generate a password 
                var crypto = new RijndaelManaged();
                crypto.KeySize = 128;
                crypto.GenerateKey();
                crypto.GenerateIV();
                cryptoKey = crypto.Key;
                cryptoIv = crypto.IV;
                
                // Encrypt the password
                var passwordKey = Convert.ToBase64String(cryptoKey);
                var passwordIv = Convert.ToBase64String(cryptoIv);
                var provider = new RSACryptoServiceProvider();
                provider.FromXmlString((publicKeyResponse as DataResponse).Data);
                var encryptedPasswordKey = Convert.ToBase64String(
                    provider.Encrypt(
                        UTF8Encoding.UTF8.GetBytes(passwordKey), false));
                var encryptedPasswordIv = Convert.ToBase64String(
                    provider.Encrypt(
                        UTF8Encoding.UTF8.GetBytes(passwordIv), false));

                // Send the password to the server
                var loginRequest = new LoginRequest(encryptedPasswordKey);
                loginRequest.AddCredential(LoginRequest.PasswordCredential, encryptedPasswordIv);
                var loginResponse = innerConnection.SendMessage("InitialiseSecureConnection", loginRequest);
                if (loginResponse.Result == ResponseResult.Failure)
                {
                    throw new CommunicationsException("Server did not allow the connection to be secured: " + loginResponse.ConcatenateErrors());
                }
            }
            catch
            {
                // Reset the password on any exception
                cryptoIv = new byte[0];
                cryptoKey = new byte[0];
                throw;
            }
        }
        #endregion

        #region EncryptMessage()
        private static string EncryptMessage(RijndaelManaged crypto, string message)
        {
            var encryptStream = new MemoryStream();
            var encrypt = new CryptoStream(encryptStream, 
                crypto.CreateEncryptor(), 
                CryptoStreamMode.Write);

            var dataToEncrypt = Encoding.UTF8.GetBytes(message);
            encrypt.Write(dataToEncrypt, 0, dataToEncrypt.Length); 
            encrypt.FlushFinalBlock();
            encrypt.Close();

            var data = Convert.ToBase64String(encryptStream.ToArray());
            return data;
        }
        #endregion

        #region DecryptMessage()
        private static string DecryptMessage(RijndaelManaged crypto, string message)
        {
            var inputStream = new MemoryStream(Convert.FromBase64String(message));
            string data;
            using (var decryptionStream = new CryptoStream(inputStream,
                crypto.CreateDecryptor(),
                CryptoStreamMode.Read))
            {
                using (var reader = new StreamReader(decryptionStream))
                {
                    data = reader.ReadToEnd();
                }
            }
            return data;
        }
        #endregion

        #region PassOnSendMessageCompleted()
        /// <summary>
        /// Passes on the <see cref="SendMessageCompleted"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PassOnSendMessageCompleted(object sender, MessageReceivedEventArgs args)
        {
            if (SendMessageCompleted != null)
            {
                SendMessageCompleted(this, args);
            }
        }
        #endregion

        #region PassOnRequestSending()
        /// <summary>
        /// Passes on the RequestSending event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PassOnRequestSending(object sender, CommunicationsEventArgs args)
        {
            FireRequestSending(args.Action, args.Message as ServerRequest);
        }
        #endregion

        #region PassOnResponseReceived()
        /// <summary>
        /// Passes on the ResponseReceived event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PassOnResponseReceived(object sender, CommunicationsEventArgs args)
        {
            FireResponseReceived(args.Action, args.Message as Response);
        }
        #endregion
        #endregion
    }
}
