using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
   [TestFixture]
   class HttpStatusTaskTest
   {

		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void PopulateFromReflectorWithAllOptions()
		{
			const string xml = @"
<checkHttpStatus>
   <description>ADesc</description>
      <httpRequest uri=""http://example.com/""/>
      
      <successStatusCodes>200,203</successStatusCodes>
      <retries>7</retries>
      <retryDelay units=""seconds"">5</retryDelay>
      <taskTimeout units=""minutes"">5</taskTimeout>
      <includeContent>true</includeContent>
</checkHttpStatus>";

		   HttpStatusTask task = (HttpStatusTask)NetReflector.Read(xml);
         Assert.AreEqual("ADesc", task.Description);
		   Assert.NotNull(task.RequestSettings, "Request settings are required");

         Assert.AreEqual("200,203", task.SuccessStatusCodes);
         Assert.AreEqual(7, task.Retries);
         Assert.AreEqual(5000, task.RetryDelay.Millis);
         Assert.IsTrue(task.HasTimeout);
         Assert.AreEqual(300000, task.Timeout.Millis);
         Assert.AreEqual(true, task.IncludeContent);
		}

      [Test]
      public void PopulateFromReflectorWithOnlyRequiredOptions()
      {
         const string xml = @"
<checkHttpStatus>
      <httpRequest uri=""http://example.com/""/>
</checkHttpStatus>";

         HttpStatusTask task = (HttpStatusTask)NetReflector.Read(xml);
         Assert.IsNull(task.Description);
         Assert.NotNull(task.RequestSettings, "Request settings are required");

         Assert.AreEqual("200", task.SuccessStatusCodes);
         Assert.AreEqual(3, task.Retries);
         
         Assert.IsFalse(task.HasTimeout);
         Assert.IsNull(task.Timeout);
         
         Assert.IsFalse(task.IncludeContent);
      }

	}
}
