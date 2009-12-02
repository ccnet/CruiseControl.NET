using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
   [TestFixture]
   public class HttpRequestSettingsTest
   {
      [Test]
      public void PopulateFromReflectorWithAllOptions()
      {
         const string xml = @"
<httpRequest>
   <useDefaultCredentials>false</useDefaultCredentials>
   <credentials userName=""someUser"" password=""somePass"" domain=""someDomain"" />
   <method>POST</method>
   <uri>http://example.com/</uri>
   <timeout units=""seconds"">60</timeout>
   <readWriteTimeout units=""minutes"">5</readWriteTimeout>
   <headers>
      <header name=""header1"" value=""value1""/>
   </headers>
   <body>foo bar baz</body>
</httpRequest>
";

         HttpRequestSettings requestSettings = (HttpRequestSettings)NetReflector.Read(xml);
  
         Assert.AreEqual(false, requestSettings.UseDefaultCredentials);
         Assert.NotNull(requestSettings.Credentials, "Credentials was specified in the settings");
         Assert.AreEqual("someUser", requestSettings.Credentials.UserName);
         Assert.AreEqual("somePass", requestSettings.Credentials.Password);
         Assert.AreEqual("someDomain", requestSettings.Credentials.Domain);

         Assert.AreEqual("POST", requestSettings.Method);
         Assert.AreEqual("http://example.com/", requestSettings.Uri.ToString());
         Assert.AreEqual(60000, requestSettings.Timeout.Millis);
         Assert.AreEqual(300000, requestSettings.ReadWriteTimeout.Millis);

         Assert.NotNull(requestSettings.Headers, "Headers were specified in the settings");
         Assert.AreEqual(1, requestSettings.Headers.Length);
         Assert.AreEqual("header1", requestSettings.Headers[0].Name);
         Assert.AreEqual("value1", requestSettings.Headers[0].Value);

         Assert.IsFalse(requestSettings.HasSendFile);
         Assert.IsTrue(requestSettings.HasBody);
         Assert.AreEqual("foo bar baz", requestSettings.Body);
      }

      [Test]
      public void PopulateFromReflectorWithOnlyRequiredOptions()
      {
         const string xml = @"<httpRequest uri=""http://example.com/""/>";

         HttpRequestSettings requestSettings = (HttpRequestSettings)NetReflector.Read(xml);
         
         Assert.IsFalse(requestSettings.UseDefaultCredentials);
         Assert.Null(requestSettings.Credentials, "Credentials was not specified in the settings");

         Assert.AreEqual("GET", requestSettings.Method);
         Assert.AreEqual("http://example.com/", requestSettings.Uri.ToString());

         Assert.IsFalse(requestSettings.HasTimeout);
         Assert.IsNull(requestSettings.Timeout);

         Assert.IsFalse(requestSettings.HasReadWriteTimeout);
         Assert.IsNull(requestSettings.ReadWriteTimeout);

         Assert.Null(requestSettings.Headers, "No headers were specified in the settings");

         Assert.IsFalse(requestSettings.HasBody);
         Assert.IsNull(requestSettings.Body);

         Assert.IsFalse(requestSettings.HasSendFile);
         Assert.IsNull(requestSettings.SendFile);
      }
   }
}