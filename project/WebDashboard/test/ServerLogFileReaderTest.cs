using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Test
{
	[TestFixture]
	public class ServerLogFileReaderTest : Assertion
	{
		private const string TEMP_DIR = "ServerLogFileReaderTest";

		[SetUp]
		protected void CreateTempDir()
		{
			TempFileUtil.CreateTempDir(TEMP_DIR);
		}

		[TearDown]
		protected void DeleteTempDir()
		{
			TempFileUtil.DeleteTempDir(TEMP_DIR);
		}

		[Test]
		public void ReadSingleLineFromLogFile()
		{
			string content = @"SampleLine";
			string filename = TempFileUtil.CreateTempFile(TEMP_DIR, "ReadSingleLineFromLogFile.log", content);
			ServerLogFileReader reader = new ServerLogFileReader(filename, 10);
			AssertEquals(content, reader.Read());
		}

		[Test]
		public void ReadLessThanInFile()
		{
			const int numFileLines = 15;
			string[] contentLines = GenerateContentLines(numFileLines);
			string filename = TempFileUtil.CreateTempFile(TEMP_DIR, "ReadLessThanInFile.log", LinesToString(contentLines));

			const int numReadLines = 10;
			ServerLogFileReader reader = new ServerLogFileReader(filename, numReadLines);
			String[] readLines = StringToLines(reader.Read());

			AssertEquals("Wrong number of lines read from file", numReadLines, readLines.Length);
			for (int i = 0; i < readLines.Length; i++)
			{
				AssertEquals(contentLines[numFileLines - numReadLines + i], readLines[i]);
			}
		}

		[Test]
		public void ReadMoreThanInFile()
		{
			const int numFileLines = 5;
			string[] contentLines = GenerateContentLines(numFileLines);
			string filename = TempFileUtil.CreateTempFile(TEMP_DIR, "ReadMoreThanInFile.log", LinesToString(contentLines));

			const int numReadLines = 10;
			ServerLogFileReader reader = new ServerLogFileReader(filename, numReadLines);
			String[] readLines = StringToLines(reader.Read());
			
			// All of file should be read
			AssertEquals("Wrong number of lines read from file", numFileLines, readLines.Length);
			for (int i = 0; i < readLines.Length; i++)
			{
				AssertEquals(contentLines[i], readLines[i]);
			}
		}

		[Test]
		public void ReadExactInFile()
		{
			const int numLines = 10;
			string[] contentLines = GenerateContentLines(numLines);
			string filename = TempFileUtil.CreateTempFile(TEMP_DIR, "ReadExactInFile.log", LinesToString(contentLines));

			ServerLogFileReader reader = new ServerLogFileReader(filename, numLines);
			String[] readLines = StringToLines(reader.Read());
			
			// All of file should be read
			AssertEquals("Wrong number of lines read from file", numLines, readLines.Length);
			for (int i = 0; i < readLines.Length; i++)
			{
				AssertEquals(contentLines[i], readLines[i]);
			}
		}

		[Test]
		public void ReadEmptyFile()
		{
			string filename = TempFileUtil.CreateTempFile(TEMP_DIR, "ReadEmptyFile.log");
			ServerLogFileReader reader = new ServerLogFileReader(filename, 10);
			AssertEquals("Error reading empty log file", "", reader.Read());
		}

//		[Test]
//		public void ReadNullFile()
//		{
//			ServerLogFileReader reader = new ServerLogFileReader(null, 10);
//			AssertEquals("Error reading file with null name", "", reader.Read());
//		}
//
		[Test]
		public void ReadTwice()
		{
			string filename = TempFileUtil.CreateTempFile(TEMP_DIR, "Twice.Log");
			ServerLogFileReader reader = new ServerLogFileReader(filename, 10);
			String first = reader.Read();
			String second = reader.Read();
			AssertEquals("Error reading file twice with same reader", first, second);
		}

		[Test, ExpectedException(typeof(FileNotFoundException))]
		public void ReadUnknownFile()
		{
			ServerLogFileReader reader = new ServerLogFileReader("BogusFileName", 10);
			AssertEquals("Error reading unknown file", "", reader.Read());
		}

		[Test]
		public void ReadFromLockedLogFile()
		{
			string tempFile = TempFileUtil.GetTempFilePath(TEMP_DIR, "LockedFilename.log");				
			using (StreamWriter stream = File.CreateText(tempFile))
			{
				stream.Write("foo");
				stream.Write("bar");
				stream.Flush();

				ServerLogFileReader reader = new ServerLogFileReader(tempFile, 10);
				AssertEquals("foobar", reader.Read());
			}
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
