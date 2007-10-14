using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Provides functionality for writing information about the stage of the build.
    /// On starting tasks, certain information will be written to a file, and this info is visualised
    /// again by cctray and the dashboard. CCNet can provide only the starting of tasks and the like.
    /// If inside information of a certain task is wanted, that task (executable) must provide it by
    /// appending/overwriting the listener file. This way CCNet does not need to know the innerworking
    /// of every task. (Nant, MSBuild, Nunit, DevEnv, ...)
    /// The visualiation stays the same.
    /// </summary>
    class ListenerFile
    {
        /// <summary>
        /// Writes the information to the listenerfile
        /// </summary>
        /// <param name="listenerFileLocation">name and location of the listener file</param>
        /// <param name="message">the information to log</param>
        public static void WriteInfo(string listenerFileLocation,string message)                                                               
        {
         System.IO.FileInfo ListenFile = new System.IO.FileInfo(listenerFileLocation) ;
                                                       
         if ( ListenFile.Directory.Exists != true) return;

         System.IO.StreamWriter ListenerFile;
         ListenerFile = new System.IO.StreamWriter(listenerFileLocation, false);
         ListenerFile.AutoFlush = true;
                                                                                                       
         ListenerFile.WriteLine("<data>");
         ListenerFile.WriteLine("<Item Time=\"{0}\" Data=\"{1}\" />",
                                 System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                                 CleanUpMessageForXMLLogging(message));
         ListenerFile.WriteLine("</data>");
                                                                                                                                               
         ListenerFile.Close();
     }
                                                                                                                                        
      /// <summary>
      /// Deletes the listenerfile
      /// </summary>
      /// <param name="listenerFileLocation">the listenerfile to delete (name and location)</param>                                                                                                                                                 
     public static void RemoveListenerFile(string listenerFileLocation)
     {
         if (System.IO.File.Exists(listenerFileLocation))
         {
             System.IO.File.Delete(listenerFileLocation);        
         }
     }
                                                                                                                                                                                                                                                                                       
     private static string CleanUpMessageForXMLLogging(string msg)
     {
         return msg.Replace("\"",string.Empty);
     }
 }
                                                                                                                                            
}

