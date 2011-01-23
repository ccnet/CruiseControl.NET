using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;
using System.Xml.Serialization;
using System.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    [TestFixture]
    public class CruiseServerClientTests
    {
        private MockRepository mocks = new MockRepository();

        [Test]
        public void ProcessMessageCorrectlyHandlesAValidMessage()
        {
            // Setup the messages
            ProjectRequest request = new ProjectRequest("123-45", "A test project");
            Response response = new Response(request);
            response.Result = ResponseResult.Success;

            // Initialises the mocks
            ICruiseServer server = mocks.DynamicMock<ICruiseServer>();
            Expect.Call(server.ForceBuild(request)).Return(response);
            mocks.ReplayAll();

            // Run the actual test
            var manager = new ThoughtWorks.CruiseControl.Core.CruiseServerClient(server);
            string responseText = manager.ProcessMessage("ForceBuild", request.ToString());
            Assert.AreEqual(response.ToString(), responseText);
            mocks.VerifyAll();
        }

        [Test]
        public void ProcessMessageCorrectlyHandlesAnUnknownMessage()
        {
            // Initialises the mocks
            ICruiseServer server = mocks.DynamicMock<ICruiseServer>();
            mocks.ReplayAll();

            // Run the actual test
            var manager = new ThoughtWorks.CruiseControl.Core.CruiseServerClient(server);
            string responseText = manager.ProcessMessage("ForceBuild", "<garbage><data/></garbage>");
            Response response = ConvertXmlToResponse(responseText);
            Assert.AreEqual(ResponseResult.Failure, response.Result, "Result is unexpected");
        }

        [Test]
        public void ProcessMessageCorrectlyHandlesAnUnknownAction()
        {
            // Setup the messages
            ProjectRequest request = new ProjectRequest("123-45", "A test project");

            // Initialises the mocks
            ICruiseServer server = mocks.DynamicMock<ICruiseServer>();
            mocks.ReplayAll();

            // Run the actual test
            var manager = new ThoughtWorks.CruiseControl.Core.CruiseServerClient(server);
            string responseText = manager.ProcessMessage("UnknownAction", request.ToString());
            Response response = ConvertXmlToResponse(responseText);
            Assert.AreEqual(ResponseResult.Failure, response.Result, "Result is unexpected");
        }

        /// <summary>
        /// Converts an XML instance into a response
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private Response ConvertXmlToResponse(string xml)
        {
            Response value = null;
            XmlSerializer serialiser = new XmlSerializer(typeof(Response));
            using (StringReader reader = new StringReader(xml))
            {
                value = serialiser.Deserialize(reader) as Response;
            }
            return value;
        }
    }
}
