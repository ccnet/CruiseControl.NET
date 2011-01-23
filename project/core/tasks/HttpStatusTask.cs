//-----------------------------------------------------------------------
// <copyright file="HttpStatusTask.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// <para>
    /// Sends an HTTP request to the specified URL.
    /// </para>
    /// </summary>
    /// <title>HTTP Status Task</title>
    /// <version>1.5</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;checkHttpStatus&gt;
    /// &lt;httpRequest&gt;
    /// &lt;uri&gt;http://somewhere.com&lt;/uri&gt;
    /// &lt;/httpRequest&gt;
    /// &lt;/checkHttpStatus&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;checkHttpStatus&gt;
    /// &lt;dynamicValues /&gt;
    /// &lt;includeContent&gt;False&lt;/includeContent&gt;
    /// &lt;httpRequest&gt;
    /// &lt;method&gt;GET&lt;/method&gt;
    /// &lt;uri&gt;http://somewhere.com/&lt;/uri&gt;
    /// &lt;useDefaultCredentials&gt;false&lt;/useDefaultCredentials&gt;
    /// &lt;/httpRequest&gt;
    /// &lt;retries&gt;3&lt;/retries&gt;
    /// &lt;successStatusCodes&gt;200&lt;/successStatusCodes&gt;
    /// &lt;/checkHttpStatus&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// Task supplied by Patrik Husfloen.
    /// </para>
    /// </remarks>
    [ReflectorType("checkHttpStatus")]
    public class HttpStatusTask 
        : TaskBase
    {
        #region Private fields
        #region successStatusCodes

        /// <summary>
        /// The successful status codes.
        /// </summary>
        private int[] successStatusCodes;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpStatusTask"/> class.
        /// </summary>
        public HttpStatusTask()
        {
            this.Retries = 3;
            this.successStatusCodes = new[] { (int)HttpStatusCode.OK };
        }
        #endregion

        #region Public properties
        #region SuccessStatusCodes
        /// <summary>
        /// The list of exit codes that indicate success, separated by commas.
        /// </summary>
        /// <version>1.5</version>
        /// <default>200</default>
        [ReflectorProperty("successStatusCodes", Required = false)]
        public string SuccessStatusCodes
        {
            get
            {
                string result = string.Empty;
                if (this.successStatusCodes != null)
                {
                    foreach (int code in this.successStatusCodes)
                    {
                        if (!(result != null && result.Length == 0))
                        {
                            result = result + ",";
                        }

                        result = result + code.ToString(CultureInfo.CurrentCulture);
                    }
                }

                return result;
            }

            set
            {
                string[] codes = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (codes.Length == 0)
                {
                    this.successStatusCodes = null;
                    return;
                }

                this.successStatusCodes = new int[codes.Length];

                for (int i = 0; i < codes.Length; ++i)
                {
                    this.successStatusCodes[i] = Int32.Parse(codes[i].Trim(), CultureInfo.CurrentCulture);
                }
            }
        }
        #endregion

        #region RequestSettings
        /// <summary>
        /// The request settings.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("httpRequest", Required = true)]
        public HttpRequestSettings RequestSettings { get; set; }
        #endregion

        #region Retries
        /// <summary>
        /// The number of retries to allow.
        /// </summary>
        /// <version>1.5</version>
        /// <default>3</default>
        [ReflectorProperty("retries", Required = false)]
        public int Retries { get; set; }
        #endregion

        #region IncludeContent
        /// <summary>
        /// Whether to include the content of the call in the log.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("includeContent", Required = false)]
        public bool IncludeContent { get; set; }
        #endregion

        #region Timeout
        /// <summary>
        /// The timeout period to allow.
        /// </summary>
        /// <version>1.5</version>
        /// <default>5 seconds</default>
        [ReflectorProperty("taskTimeout", typeof(TimeoutSerializerFactory), Required = false)]
        public Timeout Timeout { get; set; }
        #endregion

        #region HasTimeout
        /// <summary>
        /// Gets a value indicating whether this instance has timeout.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has timeout; otherwise, <c>false</c>.
        /// </value>
        public bool HasTimeout
        {
            get { return this.Timeout != null; }
        }
        #endregion

        #region RetryDelay
        /// <summary>
        /// Gets or sets the retry delay.
        /// </summary>
        /// <value>The retry delay.</value>
        [ReflectorProperty("retryDelay", typeof(TimeoutSerializerFactory), Required = false)]
        public Timeout RetryDelay { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Execute the actual task functionality.
        /// </summary>
        /// <param name="result">The current build result.</param>
        /// <returns>
        /// True if the task was successful, false otherwise.
        /// </returns>
        protected override bool Execute(IIntegrationResult result)
        {
            Log.Debug("HttpStatusTask is executing");

            bool taskTimedOut = false;
            string msg = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Checking http status of URI: '{0}'", this.RequestSettings.Uri.ToString());
            if (!String.IsNullOrEmpty(Description))
            {
                msg = Description + " (" + msg + ")";
            }

            Log.Debug(msg);
            result.BuildProgressInformation.SignalStartRunTask(msg);
            XmlTaskResult taskResult = new XmlTaskResult();
            List<HttpRequestStatus> resp = new List<HttpRequestStatus>();

            long startTimeStamp = -1;
            bool keepTrying;
            int attempt = 0;
            if (this.HasTimeout)
            {
                startTimeStamp = Stopwatch.GetTimestamp();
            }

            do
            {
                try
                {
                    if (this.HasTimeout)
                    {
                        long taskDurationTicks = Stopwatch.GetTimestamp() - startTimeStamp;
                        long taskDurationMilliSecs = (long)Math.Round(((double)taskDurationTicks / Stopwatch.Frequency) * 1000.0);
                        long msecsLeft = this.Timeout.Millis - taskDurationMilliSecs;

                        if (msecsLeft < 0)
                        {
                            taskTimedOut = true;
                            break;
                        }

                        if (msecsLeft < this.RequestSettings.Timeout.Millis)
                        {
                            this.RequestSettings.Timeout = new Timeout((int)msecsLeft, TimeUnits.MILLIS);
                        }

                        Log.Debug("Task timeout in T-{0:N1} seconds", msecsLeft / 1000.0);
                    }

                    HttpRequestStatus status = this.GetRequestStatus(this.RequestSettings);
                    if (status.Success)
                    {
                        Log.Debug("Checking returned status code ({0}) against success status codes: {1}", ((int)status.StatusCode).ToString(CultureInfo.CurrentCulture), this.SuccessStatusCodes);
                        for (int i = 0; i < this.successStatusCodes.Length; i++)
                        {
                            if ((int)status.StatusCode == this.successStatusCodes[i])
                            {
                                Log.Debug("Success, exiting...");
                                taskResult.Success = true;
                                break;
                            }
                        }
                    }

                    resp.Add(status);
                }
                catch (Exception ex)
                {
                    throw new BuilderException(this, string.Format(System.Globalization.CultureInfo.CurrentCulture,"An exception occured in HttpStatusTask while checking status for: '{0}'", this.RequestSettings.Uri), ex);
                }

                // sleeping
                keepTrying = !taskResult.Success && (this.Retries == -1 || (this.Retries - attempt++) > 0);

                if (keepTrying)
                {
                    var delay = this.RetryDelay ?? new Timeout(5000);
                    if (delay.Millis > 0)
                    {
                        delay.Normalize();
                        Log.Debug("Retrying in {0}...", delay.ToString());
                        System.Threading.Thread.Sleep(delay.Millis);
                    }
                    else
                    {
                        Log.Debug("Retrying...");
                    }
                }
            }
            while (keepTrying);

            if (taskTimedOut)
            {
                throw new BuilderException(this, string.Format(System.Globalization.CultureInfo.CurrentCulture,"HttpStatusTask timed out while checking status for: '{0}'", this.RequestSettings.Uri));
            }

            Log.Debug("Writing output...");
            XmlWriter writer = taskResult.GetWriter();
            writer.WriteStartElement("httpStatus");
            writer.WriteAttributeString("uri", this.RequestSettings.Uri.ToString());
            writer.WriteAttributeString("success", XmlConvert.ToString(taskResult.Success));
            if (!String.IsNullOrEmpty(this.Description))
            {
                writer.WriteElementString("description", this.Description);
            }

            foreach (HttpRequestStatus status in resp)
            {
                status.WriteTo(writer, this.IncludeContent);
            }

            writer.WriteEndElement();
            writer.Close();
            result.AddTaskResult(taskResult);
            Log.Debug("HttpStatusTask finished, return value: {0}", taskResult.Success.ToString(CultureInfo.CurrentCulture));
            return taskResult.Success;
        }
        #endregion

        #region GetRequestStatus()
        /// <summary>
        /// Gets the request status.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>A <see cref="HttpRequestStatus"/>.</returns>
        protected HttpRequestStatus GetRequestStatus(HttpRequestSettings settings)
        {
            HttpRequestStatus ret = new HttpRequestStatus { Settings = settings, Success = false };

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(settings.Uri);
            HttpWebResponse response = null;

            if (settings.HasHeaders)
            {
                foreach (HttpRequestHeader header in settings.Headers)
                {
                    request.Headers.Set(header.Name, header.Value);
                }
            }

            if (settings.HasTimeout || settings.HasOverrideTimeout)
            {
                request.Timeout = (settings.OverrideTimeout ?? settings.Timeout).Millis;
            }

            if (settings.HasReadWriteTimeout)
            {
                request.ReadWriteTimeout = settings.ReadWriteTimeout.Millis;
            }

            request.Method = settings.Method;
            if (settings.HasCredentials)
            {
                request.Credentials = settings.Credentials;
            }
            else
            {
                request.UseDefaultCredentials = settings.UseDefaultCredentials;
            }

            ret.RequestTime = DateTime.Now;
            long start = Stopwatch.GetTimestamp();
            Timeout timeout = new Timeout(request.Timeout);
            timeout.Normalize();
            Timeout readWriteTimeout = new Timeout(request.ReadWriteTimeout);
            readWriteTimeout.Normalize();
            Log.Debug("Requesting uri (timeout: {0}, read/write-timeout: {1})...", timeout.ToString(), readWriteTimeout.ToString());
            try
            {
                // TODO this could be better, support chunked transfer etc?
                if (settings.HasBody || settings.HasSendFile) 
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        if (settings.HasBody)
                        {
                            using (TextWriter writer = new StreamWriter(requestStream, new UTF8Encoding(false, true)))
                            {
                                writer.Write(settings.Body);
                            }
                        }
                        else
                        {
                            using (FileStream inputFile = File.OpenRead(settings.SendFile))
                            {
                                int read;
                                byte[] buff = new byte[16384];
                                do
                                {
                                    read = inputFile.Read(buff, 0, buff.Length);
                                    requestStream.Write(buff, 0, read);
                                } 
                                while (read > 0);
                            }
                        }

                        requestStream.Close();
                    }
                }

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException wEx)
            {
                ret.Exception = wEx;
                if (wEx.Status == WebExceptionStatus.Timeout)
                {
                    ret.TimedOut = true;
                }

                if (wEx.Response is HttpWebResponse)
                {
                    response = (HttpWebResponse)wEx.Response;
                }
            }

            if (response != null)
            {
                ret.StatusCode = response.StatusCode;
                ret.StatusDescription = response.StatusDescription;
                ret.ContentEncoding = response.ContentEncoding;
                ret.CharacterSet = response.CharacterSet;
                ret.ContentType = response.ContentType;
                ret.Headers = response.Headers;
                if (this.IncludeContent)
                {
                    try
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            try
                            {
                                ret.ResponseEncoding = Encoding.GetEncoding(response.CharacterSet);
                            }
                            catch (ArgumentException)
                            {
                                ret.ResponseEncoding = null;
                            }

                            if (ret.ResponseEncoding != null)
                            {
                                using (TextReader reader = new StreamReader(responseStream, ret.ResponseEncoding))
                                {
                                    ret.Content = reader.ReadToEnd();
                                }
                            }
                            else 
                            {
                                // BASE64 encode a blob
                                using (MemoryStream memBuffer = new MemoryStream())
                                {
                                    int read;
                                    byte[] buff = new byte[512];
                                    do
                                    {
                                        read = responseStream.Read(buff, 0, buff.Length);
                                        memBuffer.Write(buff, 0, read);
                                    } 
                                    while (read > 0);

                                    if (memBuffer.Length > 0)
                                    {
                                        ret.ContentIsBase64Encoded = true;
                                        ret.Content = Convert.ToBase64String(
                                            memBuffer.ToArray(),
                                            Base64FormattingOptions.InsertLineBreaks);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // keep the first exception
                        if (ret.Exception == null)
                        {
                            ret.Exception = ex;
                        }

                        ret.Content = null;
                    }
                }

                ret.Success = true;
            }

            ret.Duration = TimeSpan.FromSeconds((double)(Stopwatch.GetTimestamp() - start) / Stopwatch.Frequency);
            return ret;
        }
        #endregion
        #endregion

        #region Protected classes
        #region HttpRequestStatus
        /// <summary>
        /// An HTTP request status.
        /// </summary>
        protected class HttpRequestStatus
        {
            #region Public properties
            #region Settings
            /// <summary>
            /// Gets or sets the settings.
            /// </summary>
            /// <value>The settings.</value>
            public HttpRequestSettings Settings { get; set; }
            #endregion

            #region Content
            /// <summary>
            /// Gets or sets the content.
            /// </summary>
            /// <value>The content.</value>
            public string Content { get; set; }
            #endregion

            #region StatusCode
            /// <summary>
            /// Gets or sets the status code.
            /// </summary>
            /// <value>The status code.</value>
            public HttpStatusCode StatusCode { get; set; }
            #endregion

            #region StatusDescription
            /// <summary>
            /// Gets or sets the status description.
            /// </summary>
            /// <value>The status description.</value>
            public string StatusDescription { get; set; }
            #endregion

            #region TimedOut
            /// <summary>
            /// Gets or sets a value indicating whether [timed out].
            /// </summary>
            /// <value><c>true</c> if [timed out]; otherwise, <c>false</c>.</value>
            public bool TimedOut { get; set; }
            #endregion

            #region Success
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="HttpRequestStatus"/> is success.
            /// </summary>
            /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
            public bool Success { get; set; }
            #endregion

            #region ContentIsBase64Encoded
            /// <summary>
            /// Gets or sets a value indicating whether [content is base64 encoded].
            /// </summary>
            /// <value>
            /// <c>true</c> if [content is base64 encoded]; otherwise, <c>false</c>.
            /// </value>
            public bool ContentIsBase64Encoded { get; set; }
            #endregion

            #region Duration
            /// <summary>
            /// Gets or sets the duration.
            /// </summary>
            /// <value>The duration.</value>
            public TimeSpan Duration { get; set; }
            #endregion

            #region RequestTime
            /// <summary>
            /// Gets or sets the request time.
            /// </summary>
            /// <value>The request time.</value>
            public DateTime RequestTime { get; set; }
            #endregion

            #region ResponseEncoding
            /// <summary>
            /// Gets or sets the response encoding.
            /// </summary>
            /// <value>The response encoding.</value>
            public Encoding ResponseEncoding { get; set; }
            #endregion

            #region Exception
            /// <summary>
            /// Gets or sets the exception.
            /// </summary>
            /// <value>The exception.</value>
            public Exception Exception { get; set; }
            #endregion

            #region ContentEncoding
            /// <summary>
            /// Gets or sets the content encoding.
            /// </summary>
            /// <value>The content encoding.</value>
            public string ContentEncoding { get; set; }
            #endregion

            #region CharacterSet
            /// <summary>
            /// Gets or sets the character set.
            /// </summary>
            /// <value>The character set.</value>
            public string CharacterSet { get; set; }
            #endregion

            #region ContentType
            /// <summary>
            /// Gets or sets the type of the content.
            /// </summary>
            /// <value>The type of the content.</value>
            public string ContentType { get; set; }
            #endregion

            #region Headers
            /// <summary>
            /// Gets or sets the headers.
            /// </summary>
            /// <value>The headers.</value>
            public WebHeaderCollection Headers { get; set; }
            #endregion
            #endregion

            #region Public methods
            #region WriteTo()
            /// <summary>
            /// Writes to the specified writer.
            /// </summary>
            /// <param name="writer">The writer to use.</param>
            /// <param name="writeContent">iF set to <c>true</c> then the content will be written.</param>
            public void WriteTo(XmlWriter writer, bool writeContent)
            {
                writer.WriteStartElement("httpRequest");
                writer.WriteAttributeString("requestTime", XmlConvert.ToString(this.RequestTime, XmlDateTimeSerializationMode.Local));
                writer.WriteAttributeString("duration", XmlConvert.ToString(this.Duration.TotalSeconds));
                writer.WriteAttributeString("success", XmlConvert.ToString(this.Success));
                if (!this.Success)
                {
                    string reason;
                    if (this.TimedOut)
                    {
                        reason = "timeout";
                    }
                    else if (this.Exception != null)
                    {
                        reason = "exception";
                    }
                    else
                    {
                        reason = this.StatusDescription;
                    }

                    writer.WriteAttributeString("reason", reason);
                }
                else
                {
                    writer.WriteAttributeString("statusCode", XmlConvert.ToString((int)this.StatusCode));
                }

                if (this.Headers != null)
                {
                    for (int i = 0; i < this.Headers.Count; i++)
                    {
                        writer.WriteStartElement("header");
                        writer.WriteAttributeString("name", this.Headers.GetKey(i));
                        writer.WriteString(this.Headers[i]);
                        writer.WriteEndElement();
                    }
                }

                if (writeContent && this.Content != null)
                {
                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("type", this.ContentType);
                    writer.WriteAttributeString("encoding", this.ContentEncoding);
                    writer.WriteAttributeString("isBase64Encoded", XmlConvert.ToString(this.ContentIsBase64Encoded));
                    writer.WriteString(this.Content);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            #endregion
            #endregion
        }
        #endregion
        #endregion
    }
}
