//-----------------------------------------------------------------------
// <copyright file="TaskContextTests.cs" company="Craig Sutherland">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using Constraints = Rhino.Mocks.Constraints;

    /// <summary>
    /// Unit tests for <see cref="TaskContext"/>.
    /// </summary>
    [TestFixture]
    public class TaskContextTests
    {
        #region Private fields
        #region mocks
        /// <summary>
        /// The mocks repository.
        /// </summary>
        private MockRepository mocks;
        #endregion
        #endregion

        #region Setup
        /// <summary>
        /// Initialises the mocks for each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }
        #endregion

        #region Tests
        #region CreateResultStream() tests
        /// <summary>
        /// Tests that CreateResultStream() ensures task name is not null or empty.
        /// </summary>
        [Test]
        public void CreateResultStreamValidatesTaskNameIsNotNullOrEmpty()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            var context = new TaskContext(ioSystem, basePath);

            this.mocks.ReplayAll();
            var error1 = Assert.Throws<ArgumentException>(() => context.CreateResultStream(null, "type")) as ArgumentException;
            var error2 = Assert.Throws<ArgumentException>(() => context.CreateResultStream(string.Empty, "type")) as ArgumentException;
            this.mocks.VerifyAll();

            Assert.AreEqual("taskName", error1.ParamName);
            Assert.AreEqual("taskName", error2.ParamName);
        }

        /// <summary>
        /// Tests that CreateResultStream() ensures task type is not null or empty.
        /// </summary>
        [Test]
        public void CreateResultStreamValidatesTaskTypeIsNotNullOrEmpty()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            var context = new TaskContext(ioSystem, basePath);

            this.mocks.ReplayAll();
            var error1 = Assert.Throws<ArgumentException>(() => context.CreateResultStream("name", null)) as ArgumentException;
            var error2 = Assert.Throws<ArgumentException>(() => context.CreateResultStream("name", string.Empty)) as ArgumentException;
            this.mocks.VerifyAll();

            Assert.AreEqual("taskType", error1.ParamName);
            Assert.AreEqual("taskType", error2.ParamName);
        }

        /// <summary>
        /// Tests that CreateResultStream() opens a new file stream and appends .xml to the file name.
        /// </summary>
        [Test]
        public void CreateResultStreamOpensAStream()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            var filePath = Path.Combine(basePath, "Test.xml");
            Expect.Call(ioSystem.FileExists(filePath)).Return(false);
            using (var fileStream = new MemoryStream())
            {
                Expect.Call(ioSystem.OpenOutputStream(filePath)).Return(fileStream);
                var context = new TaskContext(ioSystem, basePath);

                this.mocks.ReplayAll();
                var stream = context.CreateResultStream("Test", "Type");

                this.mocks.VerifyAll();
                Assert.AreSame(fileStream, stream);
            }
        }

        /// <summary>
        /// Tests that CreateResultStream() opens a new file stream with a unique filename and changes the extension.
        /// </summary>
        [Test]
        public void CreateResultStreamOpensAStreamForDuplicatedName()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            var filePath1 = Path.Combine(basePath, "Test.xml");
            var filePath2 = Path.Combine(basePath, "Test-1.xml");
            Expect.Call(ioSystem.FileExists(filePath1)).Return(true);
            Expect.Call(ioSystem.FileExists(filePath2)).Return(false);
            using (var fileStream = new FileStream(filePath2, FileMode.Create))
            {
                Expect.Call(ioSystem.OpenOutputStream(filePath2)).Return(fileStream);
                var context = new TaskContext(ioSystem, basePath);

                this.mocks.ReplayAll();
                var stream = context.CreateResultStream("Test.txt", "Type");

                this.mocks.VerifyAll();
                Assert.AreSame(fileStream, stream);
            }
        }

        /// <summary>
        /// Tests that CreateResultStream() opens a new file stream without changing the extension.
        /// </summary>
        [Test]
        public void CreateResultStreamOpensAStreamWithOriginalExtension()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            var filePath = Path.Combine(basePath, "Test.txt");
            Expect.Call(ioSystem.FileExists(filePath)).Return(false);
            using (var fileStream = new MemoryStream())
            {
                Expect.Call(ioSystem.OpenOutputStream(filePath)).Return(fileStream);
                var context = new TaskContext(ioSystem, basePath);

                this.mocks.ReplayAll();
                var stream = context.CreateResultStream("Test.txt", "Type", true);

                this.mocks.VerifyAll();
                Assert.AreSame(fileStream, stream);
            }
        }
        #endregion

        #region StartChildContext() tests
        /// <summary>
        /// StartChildContext() starts a new context with the same artifact folder.
        /// </summary>
        [Test]
        public void StartChildContextStartANewContext()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            var likeCondition = basePath.Replace("\\", "\\\\") + "[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}";
            Expect.Call(() => ioSystem.EnsureFolderExists(null))
                .Constraints(
                    new Constraints.Like(likeCondition));
            var context = new TaskContext(ioSystem, basePath);

            mocks.ReplayAll();
            var childContext = context.StartChildContext();
            mocks.VerifyAll();

            StringAssert.IsMatch(likeCondition, childContext.ArtifactFolder);
            Assert.IsFalse(childContext.IsFinialised);
        }
        #endregion

        #region MergeChildContext() tests
        /// <summary>
        /// MergeChildContext() finialises the child context.
        /// </summary>
        [Test]
        public void MergeChildContextFinialisesTheChildContext()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            Expect.Call(() => ioSystem.EnsureFolderExists(null)).IgnoreArguments();
            var context = new TaskContext(ioSystem, basePath);

            mocks.ReplayAll();
            var childContext = context.StartChildContext();
            context.MergeChildContext(childContext);
            mocks.VerifyAll();

            Assert.IsTrue(childContext.IsFinialised);
        }

        /// <summary>
        /// MergeChildContext() finialises the child context.
        /// </summary>
        [Test]
        public void MergeChildContextMergesChildTaskResults()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            Expect.Call(() => ioSystem.EnsureFolderExists(null)).IgnoreArguments();
            using (var fileStream = new MemoryStream())
            {
                var likeCondition = basePath.Replace("\\", "\\\\") + "[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\\\\test.xml";
                Expect.Call(ioSystem.FileExists(null))
                    .Return(false)
                    .Constraints(new Constraints.Like(likeCondition));
                Expect.Call(ioSystem.OpenOutputStream(null))
                    .Return(fileStream)
                    .Constraints(new Constraints.Like(likeCondition));
                Expect.Call(ioSystem.FileExists(Path.Combine(basePath, "test.xml"))).Return(false);
                Expect.Call(() => ioSystem.MoveFile(null, null))
                    .Constraints(new Constraints.Like(likeCondition), new Constraints.Equal(Path.Combine(basePath, "test.xml")));
                var context = new TaskContext(ioSystem, basePath);

                mocks.ReplayAll();
                var childContext = context.StartChildContext();
                var stream = childContext.CreateResultStream("test", "type");
                context.MergeChildContext(childContext);
                mocks.VerifyAll();

                Assert.IsTrue(childContext.IsFinialised);
            }
        }

        /// <summary>
        /// MergeChildContext() validates the child context argument.
        /// </summary>
        [Test]
        public void MergeChildContextValidatesChildContextArgument()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            var context = new TaskContext(ioSystem, basePath);
            var error = Assert.Throws<ArgumentNullException>(() =>
            {
                context.MergeChildContext(null);
            });
            Assert.AreEqual("childContext", error.ParamName);
        }

        /// <summary>
        /// MergeChildContext() finialises the child context.
        /// </summary>
        [Test]
        public void MergeChildContextChckesTheContextMatches()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            var context1 = new TaskContext(ioSystem, basePath);
            var context2 = new TaskContext(ioSystem, basePath);
            var childContext = context1.StartChildContext();
            var error = Assert.Throws<ArgumentException>(() =>
            {
                context2.MergeChildContext(childContext);
            });
            Assert.AreEqual("childContext", error.ParamName);
        }
        #endregion

        #region Finialise() tests
        /// <summary>
        /// Finialise() should mark the context as finialised and write out an XML index file.
        /// </summary>
        [Test]
        public void FinialiseFinialisesTheContextAndGeneratesTheIndex()
        {
            var basePath = Path.GetTempPath();
            var ioSystem = this.mocks.StrictMock<IFileSystem>();
            var filePath = Path.Combine(basePath, "Test.xml");
            var indexPath = Path.Combine(basePath, "ccnet-task-index.xml");
            Expect.Call(ioSystem.FileExists(filePath)).Return(false);
            string xmlIndex;
            using (var fileStream = new MemoryStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    Expect.Call(ioSystem.OpenOutputStream(filePath)).Return(fileStream);
                    Expect.Call(ioSystem.OpenOutputStream(indexPath)).Return(memoryStream);
                    var context = new TaskContext(ioSystem, basePath);

                    this.mocks.ReplayAll();
                    var stream = context.CreateResultStream("Test", "Type");
                    context.Finialise();

                    this.mocks.VerifyAll();
                    Assert.IsTrue(context.IsFinialised);

                    // Read the index so it can be verified
                    using (var readerStream = new MemoryStream(memoryStream.GetBuffer()))
                    {
                        using (var reader = new StreamReader(readerStream))
                        {
                            xmlIndex = reader.ReadToEnd();
                        }
                    }
                }
            }

            var expectedIndex = "<task>" +
                    "<result file=\"" + filePath + "\" name=\"Test\" type=\"Type\" />" +
                "</task>";
            var endPos = xmlIndex.IndexOf('\x0');
            if (endPos > 0)
            {
                xmlIndex = xmlIndex.Substring(0, endPos);
            }

            Assert.AreEqual(expectedIndex, xmlIndex);
        }
        #endregion
        #endregion
    }
}
