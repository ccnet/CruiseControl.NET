using System;
using System.Linq;
using ThoughtWorks.CruiseControl.Remote;

namespace CCNet.CSharpDemos.Plugin.Extensions
{
    public class DoSomething
        : ICruiseServerExtension
    {
        private ICruiseServer server;
        private int count = 0;
        private int maxCount = 4;

        public void Initialise(ICruiseServer server, 
            ExtensionConfiguration extensionConfig)
        {
            var projectsElement = extensionConfig.Items
                .SingleOrDefault(n => n.Name == "allowedProjects");
            if (projectsElement != null)
            {
                this.maxCount = int.Parse(
                    projectsElement.InnerText) - 1;
            }

            this.server = server;
            this.server.ProjectStarting += (o, e) =>
            {
                if (this.count >= this.maxCount)
                {
                    e.Cancel = true;
                }
                else
                {
                    this.count++;
                }
            };
            this.server.ProjectStopped += (o, e) =>
            {
                this.count--;
            };
            Console.WriteLine("Initialise");
        }

        public void Start()
        {
            Console.WriteLine("Start");
        }

        public void Stop()
        {
            Console.WriteLine("Stop");
        }

        public void Abort()
        {
            Console.WriteLine("Abort");
        }
    }
}
