using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console
{
    public class ConsoleArgs
    {
        public string Command { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string LogFile { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool GenerateXsd { get; set; }
    }
}
