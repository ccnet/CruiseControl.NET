using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Web.test
{
	[TestFixture]
	public class ServerLogFileReaderTest
	{
		[Test]
		public void ReadSingleLineFromLogFile()
		{
			string content = @"SampleLine";
			string filename = TempFileUtil.CreateTempFile(Path.GetTempPath(), "ReadSingleLineFromLogFile.log", content);
			ServerLogFileReader reader = new ServerLogFileReader(filename, 10);
			Assertion.AssertEquals(content, reader.Read());
		}

		[Test]
		public void ReadLessThanInFile()
		{
			const int numFileLines = 15;
			string[] contentLines = GenerateContentLines(numFileLines);
			string filename = TempFileUtil.CreateTempFile(Path.GetTempPath(), "ReadLessThanInFile.log", LinesToString(contentLines));

			const int numReadLines = 10;
			ServerLogFileReader reader = new ServerLogFileReader(filename, numReadLines);
			String[] readLines = StringToLines(reader.Read());

			Assertion.AssertEquals("Wrong number of lines read from file", numReadLines, readLines.Length);
			for (int i = 0; i < readLines.Length; i++)
			{
				Assertion.AssertEquals(contentLines[numFileLines - numReadLines + i], readLines[i]);
			}
		}

		[Test]
		public void ReadMoreThanInFile()
		{
			const int numFileLines = 5;
			string[] contentLines = GenerateContentLines(numFileLines);
			string filename = TempFileUtil.CreateTempFile(Path.GetTempPath(), "ReadMoreThanInFile.log", LinesToString(contentLines));

			const int numReadLines = 10;
			ServerLogFileReader reader = new ServerLogFileReader(filename, numReadLines);
			String[] readLines = StringToLines(reader.Read());
			
			// All of file should be read
			Assertion.AssertEquals("Wrong number of lines read from file", numFileLines, readLines.Length);
			for (int i = 0; i < readLines.Length; i++)
			{
				Assertion.AssertEquals(contentLines[i], readLines[i]);
			}
		}

		[Test]
		public void ReadExactInFile()
		{
			const int numLines = 10;
			string[] contentLines = GenerateContentLines(numLines);
			string filename = TempFileUtil.CreateTempFile(Path.GetTempPath(), "ReadExactInFile.log", LinesToString(contentLines));

			ServerLogFileReader reader = new ServerLogFileReader(filename, numLines);
			String[] readLines = StringToLines(reader.Read());
			
			// All of file should be read
			Assertion.AssertEquals("Wrong number of lines read from file", numLines, readLines.Length);
			for (int i = 0; i < readLines.Length; i++)
			{
				Assertion.AssertEquals(contentLines[i], readLines[i]);
			}
		}

		[Test]
		public void ReadEmptyFile()
		{
			string filename = TempFileUtil.CreateTempFile(Path.GetTempPath(), "ReadEmptyFile.log");
			ServerLogFileReader reader = new ServerLogFileReader(filename, 10);
			Assertion.AssertEquals("Error reading empty log file", "", reader.Read());
		}

		[Test]
		public void ReadNullFile()
		{
			ServerLogFileReader reader = new ServerLogFileReader(null, 10);
			Assertion.AssertEquals("Error reading file with null name", "", reader.Read());
		}

		[Test]
		public void ReadTwice()
		{
			string filename = TempFileUtil.CreateTempFile(Path.GetTempPath(), "Twice.Log");
			ServerLogFileReader reader = new ServerLogFileReader(filename, 10);
			String first = reader.Read();
			String second = reader.Read();
			Assertion.AssertEquals("Error reading file twice with same reader", first, second);
		}

		[Test]
	    [ExpectedException(typeof(FileNotFoundException))]
		public void ReadUnknownFile()
		{
			ServerLogFileReader reader = new ServerLogFileReader("BogusFileName", 10);
			Assertion.AssertEquals("Error reading unknown file", "", reader.Read());
		}

		private string[] GenerateContentLines(int lines)
		{
			String[] contentLines = new String[lines];
			for (int i = 0; i < lines; i++)
			{
				contentLines[i] = string.Format("Line {0}", i + 1);
			}

			return contentLines;
		}
		
		private string LinesToString(string[] contentLines)
		{
			return string.Join(Environment.NewLine, contentLines);
		}

		private string[] StringToLines(string content)
		{
			Regex regexLines = new Regex(@"\r\n");
			return regexLines.Split(content);
		}
	}
}
