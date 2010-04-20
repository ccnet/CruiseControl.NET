namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;
    using System.Security.Cryptography;
    using System.IO;

    public class EncryptingConnectionTests
    {
        #region Tests
        [Test]
        public void ConstructorWiresUpEvents()
        {
            var innerConnection = new TestConnection();
            var outerConnection = new EncryptingConnection(innerConnection);
            var sendMessageFired = false;
            var requestSendingFired = false;
            var responseReceivedFired = false;
            outerConnection.SendMessageCompleted += (o, e) => { sendMessageFired = true; };
            outerConnection.RequestSending += (o, e) => { requestSendingFired = true; };
            outerConnection.ResponseReceived += (o, e) => { responseReceivedFired = true; };
            innerConnection.TriggerSendMessageCompleted(null, null, false, null);
            innerConnection.TriggerRequestSending(null, null);
            innerConnection.TriggerResponseReceived(null, null);
            Assert.IsTrue(sendMessageFired);
            Assert.IsTrue(requestSendingFired);
            Assert.IsTrue(responseReceivedFired);
        }

        [Test]
        public void PropertiesAreRetrievedFromInnerConnection()
        {
            var innerConnection = new TestConnection
            {
                Type = "innerTest",
                ServerName = "theServerName",
                Address = "http://lostintranslation",
                IsBusy = false
            };
            var outerConnection = new EncryptingConnection(innerConnection);
            Assert.AreEqual(innerConnection.Type, outerConnection.Type);
            Assert.AreEqual(innerConnection.ServerName, outerConnection.ServerName);
            Assert.AreEqual(innerConnection.Address, outerConnection.Address);
            Assert.AreEqual(innerConnection.IsBusy, outerConnection.IsBusy);
            innerConnection.IsBusy = true;
            Assert.AreEqual(innerConnection.IsBusy, outerConnection.IsBusy);
        }

        [Test]
        public void DisposedIsPassedOn()
        {
            var innerConnection = new TestConnection();
            var outerConnection = new EncryptingConnection(innerConnection);
            outerConnection.Dispose();
            Assert.IsTrue(innerConnection.Disposed);
        }

        [Test]
        public void SendMessageEncryptsMessage()
        {
            var innerConnection = new TestConnection();
            var outerConnection = new EncryptingConnection(innerConnection);
            var request = new ServerRequest();
            var expectedResponse = new Response(request);
            var actionName = "DoSomething";
            string iv = null;
            string key = null;

            innerConnection.SendMessageAction = (a, r) =>
            {
                Response sendResponse = null;
                if (a == "RetrievePublicKey")
                {
                    sendResponse = this.GenerateKeyResponse(r);
                }
                else if (a == "InitialiseSecureConnection")
                {
                    Assert.IsInstanceOf<LoginRequest>(r);
                    sendResponse = this.GenerateConnectioResponse(r as LoginRequest, out iv, out key);
                }
                else if (a == "ProcessSecureRequest")
                {
                    Assert.IsInstanceOf<EncryptedRequest>(r);
                    var actualRequest = r as EncryptedRequest;
                    Assert.AreEqual(actionName, actualRequest.Action);

                    var crypto = new RijndaelManaged();
                    crypto.Key = Convert.FromBase64String(key);
                    crypto.IV = Convert.FromBase64String(iv);
                    var requestData = DecryptMessage(crypto, actualRequest.EncryptedData);
                    Assert.AreEqual(request.ToString(), requestData);

                    var encryptedResponse = new EncryptedResponse();
                    encryptedResponse.Result = ResponseResult.Success;
                    encryptedResponse.EncryptedData = EncryptMessage(crypto, expectedResponse.ToString());
                    sendResponse = encryptedResponse;
                }
                else
                {
                    Assert.Fail("Unknown action: " + a);
                }

                return sendResponse;
            };
            var response = outerConnection.SendMessage(actionName, request);
            Assert.IsNotNull(response);
            Assert.AreEqual(expectedResponse.RequestIdentifier, response.RequestIdentifier);
        }
        #endregion

        #region Helper methods
        public DataResponse GenerateKeyResponse(ServerRequest request)
        {
            var response = new DataResponse(request);

            // Either generate or retrieve the key for CruiseControl.NET Server
            var cp = new CspParameters();
            cp.KeyContainerName = "CruiseControl.NET Server";
            var provider = new RSACryptoServiceProvider(cp);

            // Return the public key
            response.Data = provider.ToXmlString(false);
            response.Result = ResponseResult.Success;

            return response;
        }

        public Response GenerateConnectioResponse(LoginRequest request, out string iv, out string key)
        {
            // Decrypt the password
            var cp = new CspParameters();
            cp.KeyContainerName = "CruiseControl.NET Server";
            var provider = new RSACryptoServiceProvider(cp);
            var originalKey = request.FindCredential(LoginRequest.UserNameCredential).Value;
            key = UTF8Encoding.UTF8.GetString(
                provider.Decrypt(Convert.FromBase64String(originalKey), false));
            var originalIv = request.FindCredential(LoginRequest.PasswordCredential).Value;
            iv = UTF8Encoding.UTF8.GetString(
                provider.Decrypt(Convert.FromBase64String(originalIv), false));

            // Generate a response
            var response = new Response(request);
            response.Result = ResponseResult.Success;
            return response;
        }

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

        #region Private classes
        private class TestConnection
            : ServerConnectionBase, IServerConnection, IDisposable
        {
            public string Type { get; set; }

            public string ServerName { get; set; }

            public bool IsBusy { get; set; }

            public string Address { get; set; }

            public bool Disposed { get; set; }

            public Func<string, ServerRequest, Response> SendMessageAction { get; set; }

            public Response SendMessage(string action, ServerRequest request)
            {
                return this.SendMessageAction(action, request);
            }

            public void SendMessageAsync(string action, ServerRequest request)
            {
                this.SendMessageAsync(action, request, null);
            }

            public void SendMessageAsync(string action, ServerRequest request, object userState)
            {
                var response = this.SendMessageAction(action, request);
                this.TriggerSendMessageCompleted(response, null, false, userState);
            }

            public Action<object> CancelAsyncAction { get; set; }

            public void CancelAsync()
            {
                this.CancelAsync(null);
            }

            public void CancelAsync(object userState)
            {
                this.CancelAsyncAction(userState);
            }

            public event EventHandler<MessageReceivedEventArgs> SendMessageCompleted;

            public void TriggerSendMessageCompleted(Response response, Exception error, bool cancelled, object userState)
            {
                var args = new MessageReceivedEventArgs(response, error, cancelled, userState);
                this.SendMessageCompleted(this, args);
            }

            public void TriggerRequestSending(string action, ServerRequest request)
            {
                this.FireRequestSending(action, request);
            }

            public void TriggerResponseReceived(string action, Response response)
            {
                this.FireResponseReceived(action, response);
            }

            public void Dispose()
            {
                this.Disposed = true;
            }
        }
        #endregion
    }
}
