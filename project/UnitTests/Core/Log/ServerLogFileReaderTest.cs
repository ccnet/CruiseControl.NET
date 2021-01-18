using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Logging;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Logging
{
	[TestFixture]
	public class ServerLogFileReaderTest
	{
		private SystemPath tempDir;

		[SetUp]
		protected void CreateTempDir()
		{
			tempDir = SystemPath.Temp;
		}

		[TearDown]
		protected void DeleteTempDir()
		{
			if (tempDir.Exists()) tempDir.DeleteDirectory();
		}

		[Test]
		public void ReadSingleLineFromLogFile()
		{
			string content = @"SampleLine";
			SystemPath filename = tempDir.CreateTextFile("ReadSingleLineFromLogFile.log", content);
			ServerLogFileReader reader = new ServerLogFileReader(filename.ToString(), 10);
			Assert.AreEqual(content, reader.Read());
		}

		[Test]
		public void ReadLessThanInFile()
		{
			const int numFileLines = 15;
			string[] contentLines = GenerateContentLines(numFileLines);
			SystemPath filename = tempDir.CreateTextFile("ReadLessThanInFile.log", LinesToString(contentLines));

			const int numReadLines = 10;
			ServerLogFileReader reader = new ServerLogFileReader(filename.ToString(), numReadLines);
			String[] readLines = StringToLines(reader.Read());

			Assert.AreEqual(numReadLines, readLines.Length);
			for (int i = 0; i < readLines.Length; i++)
			{
				Assert.AreEqual(contentLines[numFileLines - numReadLines + i], readLines[i]);
			}
		}

		[Test]
		public void ReadMoreThanInFile()
		{
			const int numFileLines = 5;
			string[] contentLines = GenerateContentLines(numFileLines);
			SystemPath filename = tempDir.CreateTextFile("ReadMoreThanInFile.log", LinesToString(contentLines));

			const int numReadLines = 10;
			ServerLogFileReader reader = new ServerLogFileReader(filename.ToString(), numReadLines);
			String[] readLines = StringToLines(reader.Read());

			// All of file should be read
			Assert.AreEqual(numFileLines, readLines.Length);
			for (int i = 0; i < readLines.Length; i++)
			{
				Assert.AreEqual(contentLines[i], readLines[i]);
			}
		}

		[Test]
		public void ReadExactInFile()
		{
			const int numLines = 10;
			string[] contentLines = GenerateContentLines(numLines);
			SystemPath filename = tempDir.CreateTextFile("ReadExactInFile.log", LinesToString(contentLines));

			ServerLogFileReader reader = new ServerLogFileReader(filename.ToString(), numLines);
			String[] readLines = StringToLines(reader.Read());

			// All of file should be read
			Assert.AreEqual(numLines, readLines.Length);
			for (int i = 0; i < readLines.Length; i++)
			{
				Assert.AreEqual(contentLines[i], readLines[i]);
			}
		}

		[Test]
		public void ReadEmptyFile()
		{
			SystemPath filename = tempDir.CreateEmptyFile("ReadEmptyFile.log");
			ServerLogFileReader reader = new ServerLogFileReader(filename.ToString(), 10);
			Assert.AreEqual("", reader.Read(), "Error reading empty log file");
		}

//		[Test]
//		public void ReadNullFile()
//		{
//			ServerLogFileReader reader = new ServerLogFileReader(null, 10);
//			Assert.AreEqual("Error reading file with null name", "", reader.Read());
//		}
//
		[Test]
		public void ReadTwice()
		{
			SystemPath filename = tempDir.CreateEmptyFile("Twice.Log");
			ServerLogFileReader reader = new ServerLogFileReader(filename.ToString(), 10);
			String first = reader.Read();
			String second = reader.Read();
			Assert.AreEqual(first, second, "Error reading file twice with same reader");
		}

		[Test]
		public void ReadUnknownFile()
		{
			ServerLogFileReader reader = new ServerLogFileReader("BogusFileName", 10);
			Assert.That(delegate { reader.Read(); },
                        Throws.TypeOf<FileNotFoundException>());
		}

		[Test]
		public void ReadFromLockedLogFile()
		{
			SystemPath tempFile = tempDir.CreateEmptyFile("LockedFilename.log");
			using (StreamWriter stream = File.CreateText(tempFile.ToString()))
			{
				stream.Write("foo");
				stream.Write("bar");
				stream.Flush();

				ServerLogFileReader reader = new ServerLogFileReader(tempFile.ToString(), 10);
				Assert.AreEqual("foobar", reader.Read());
			}
		}

		[Test]
		public void ReadOutputFromSpecifiedProject()
		{
			string content = @"2006-11-24 20:09:52,000 [CCNet Server:INFO] Starting CruiseControl.NET Server
2006-11-24 20:09:53,000 [foo:INFO] Starting integrator for project: foo
2006-11-24 20:09:54,000 [bar:INFO] Starting integrator for project: bar
2006-11-24 20:09:55,000 [foo:INFO] No modifications detected
2006-11-24 20:09:56,000 [bar:INFO] No modifications detected.";
			SystemPath tempFile = tempDir.CreateTextFile("MultiProject.log", content);
			
			ServerLogFileReader reader = new ServerLogFileReader(tempFile.ToString(), 10);
			Assert.AreEqual(@"2006-11-24 20:09:53,000 [foo:INFO] Starting integrator for project: foo" + Environment.NewLine + "2006-11-24 20:09:55,000 [foo:INFO] No modifications detected", reader.Read("foo"));
			Assert.AreEqual(@"2006-11-24 20:09:54,000 [bar:INFO] Starting integrator for project: bar" + Environment.NewLine + "2006-11-24 20:09:56,000 [bar:INFO] No modifications detected.", reader.Read("bar"));
		}
		
		private static string[] GenerateContentLines(int lines)
		{
			String[] contentLines = new String[lines];
			for (int i = 0; i < lines; i++)
			{
				contentLines[i] = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Line {0}", i + 1);
			}

			return contentLines;
		}

		private static string LinesToString(string[] contentLines)
		{
			return string.Join(Environment.NewLine, contentLines);
		}

		private static string[] StringToLines(string content)
		{
			Regex regexLines = new Regex(Environment.NewLine);
			return regexLines.Split(content);
		}
	}
}