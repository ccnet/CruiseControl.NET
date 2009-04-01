using System;
using System.Threading;
using agsXMPP;
using agsXMPP.protocol.client;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// A publisher to send out results via XMPP.
    /// </summary>
    /// <remarks>
    /// This relies on the argsXMPP SDK from Ag Software - http://www.ag-software.de/index.php?page=agsxmpp-sdk.
    /// </remarks>
    [ReflectorType("xmpp")]
    public class XmppPublisher
        : ITask
    {
        #region Private fields
        private string userName;
        private string password;
        private string server = "gmail.com";
        private string connectServer = "talk.google.com";
        private int timeOutPeriod = 120;
        private int sendPeriod = 30;
        private string[] recipients;
        private XmppClientConnection client;
        private bool hasError;
        private DateTime? messagesSent;
        private IIntegrationResult result;
        #endregion

        #region Public properties
        #region LoginName
        /// <summary>
        /// The login name to use.
        /// </summary>
        [ReflectorProperty("loginName", Required = true)]
        public string LoginName
        {
            get { return userName; }
            set { userName = value; }
        }
        #endregion

        #region LoginPassword
        /// <summary>
        /// The login password to use.
        /// </summary>
        [ReflectorProperty("password", Required = true)]
        public string LoginPassword
        {
            get { return password; }
            set { password = value; }
        }
        #endregion

        #region Server
        /// <summary>
        /// The server to use - defaults to gmail.com.
        /// </summary>
        [ReflectorProperty("server", Required = false)]
        public string Server
        {
            get { return server; }
            set { server = value; }
        }
        #endregion

        #region ConnectServer
        /// <summary>
        /// The connections server to use - defaults to talk.gmail.com.
        /// </summary>
        [ReflectorProperty("connectServer", Required = false)]
        public string ConnectServer
        {
            get { return connectServer; }
            set { connectServer = value; }
        }
        #endregion

        #region TimeOutPeriod
        /// <summary>
        /// The time-out period to wait before disconnecting the client.
        /// </summary>
        [ReflectorProperty("timeOutPeriod", Required = false)]
        public int TimeOutPeriod
        {
            get { return timeOutPeriod; }
            set { timeOutPeriod = value; }
        }
        #endregion

        #region SendPeriod
        /// <summary>
        /// The period to wait for the messages to be sent.
        /// </summary>
        [ReflectorProperty("sendPeriod", Required = false)]
        public int SendPeriod
        {
            get { return sendPeriod; }
            set { sendPeriod = value; }
        }
        #endregion

        #region Recipients
        /// <summary>
        /// The list of people to send the completion messages to.
        /// </summary>
        [ReflectorArray("recipients", Required = true)]
        public string[] Recipients
        {
            get { return recipients; }
            set { recipients = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Run()
        /// <summary>
        /// Attempt to send out the results via XMPP.
        /// </summary>
        /// <param name="result"></param>
        public void Run(IIntegrationResult result)
        {
            this.result = result;

            try
            {
                // Initialise the client and login
                hasError = false;
                messagesSent = null;
                client = new XmppClientConnection();
                client.Server = server;
                client.ConnectServer = connectServer;
                client.AutoRoster = false;
                client.Resource = null;
                client.OnLogin += new ObjectHandler(xmppClient_OnLogin);
                client.OnError += new ErrorHandler(xmppClient_OnError);
                client.OnAuthError += new XmppElementHandler(client_OnAuthError);
                Log.Debug("Connecting to XMPP server: " + server);
                client.Open(userName, password);

                // Wait for the messages to be sent
                bool continueWaiting = true;
                DateTime startWait = DateTime.Now;
                while (continueWaiting)
                {
                    Thread.Sleep(100);
                    if (hasError)
                    {
                        continueWaiting = false;
                    }
                    else if (client.Authenticated)
                    {
                        if (messagesSent.HasValue)
                        {
                            TimeSpan timeWaited = (DateTime.Now - messagesSent.Value);
                            continueWaiting = (timeWaited.TotalSeconds < sendPeriod);
                        }
                    }

                    // Time-out after the specified time
                    if (continueWaiting)
                    {
                        TimeSpan timeWaited = (DateTime.Now - startWait);
                        if (timeWaited.TotalSeconds > timeOutPeriod)
                        {
                            continueWaiting = false;
                            Log.Warning("Time-out period has expired - send was not successful");
                        }
                    }
                }

                // Clean up
                client.Close();
                client = null;
            }
            catch (Exception error)
            {
                Log.Error(error);
                result.ExceptionResult = error;
                result.Status = ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Exception;
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region xmppClient_OnError()
        /// <summary>
        /// Handle any errors - basically log them and tell the send loop to cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private void xmppClient_OnError(object sender, Exception ex)
        {
            result.ExceptionResult = ex;
            result.Status = ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Exception;
            hasError = true;
            Log.Error(ex);
        }
        #endregion

        #region client_OnAuthError()
        /// <summary>
        /// Catch any authentication errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            result.ExceptionResult = new Exception(
                string.Format("XPPP Authentication Failed - Remote server response was {0}", e.InnerXml));
            result.Status = ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Exception;
            hasError = true;
            Log.Error(result.ExceptionResult);
        }
        #endregion

        #region xmppClient_OnLogin()
        /// <summary>
        /// Performs the actual message send after the client has successfully logged on.
        /// </summary>
        /// <param name="sender"></param>
        private void xmppClient_OnLogin(object sender)
        {
            string messageBody = string.Format("{0} has finished building - result is {1}", result.ProjectName, result.Status);

            // Iterate through the list of recipients and send a message to each one.
            foreach (string recipient in recipients)
            {
                Log.Debug("Sending XMPP message to  " + recipient);
                try
                {
                    Message message = new Message(recipient, MessageType.chat, messageBody);
                    client.Send(message);
                }
                catch (Exception error)
                {
                    Log.Error(error);
                }
            }

            // Tell the waiting loop that the messages have been sent.
            messagesSent = DateTime.Now;
        }
        #endregion
        #endregion
    }
}
