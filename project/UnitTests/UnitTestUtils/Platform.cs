using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.UnitTests
{
    public class Platform 
    {
        public static bool IsMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }
        
        public static bool IsWindows
        {
            get
            {
                return Path.DirectorySeparatorChar == '\\';
            }
        }
    }
}