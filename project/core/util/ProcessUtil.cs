using System;
using System.Diagnostics;
using System.IO;

namespace tw.ccnet.core.util
{
	public class ProcessUtil
	{
		public static Process CreateProcess(string executable, string args, string workingDirectory)
		{
			Process process = CreateProcess(executable, args);
			process.StartInfo.WorkingDirectory = workingDirectory;
			return process;
		}

		public static Process CreateProcess(string executable, string args)
		{
			Process process = new Process();
			process.StartInfo.FileName = executable;
			process.StartInfo.Arguments = args;
			return process;
		}
	
		public static TextReader ExecuteRedirected(Process process)
		{
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;			
			process.Start();
			return process.StandardOutput;			
		}
		
		public static TextReader GetTextReader(string path)
		{
			FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new StreamReader(stream);
		}
	}
}
