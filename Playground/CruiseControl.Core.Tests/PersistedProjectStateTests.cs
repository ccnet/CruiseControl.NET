namespace CruiseControl.Core.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using CruiseControl.Core.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class PersistedProjectStateTests
    {
        #region Tests
        [Test]
        public void SetValueStoresANewValue()
        {
            var name = "TestItem";
            var value = "TestValue";
            var task = new Comment();
            var state = new PersistedProjectState();
            Assert.IsNull(GetValueFromState(state, task, name));
            state.SetValue(task, name, value);
            var actual = GetValueFromState(state, task, name);
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Owner, task.UniversalName);
            Assert.AreEqual(actual.Name, name);
            Assert.AreEqual(actual.Value, value);
        }

        [Test]
        public void SetValueUpdatesAnExistingValue()
        {
            var name = "TestItem";
            var value = "TestValue";
            var task = new Comment();
            var state = new PersistedProjectState();
            state.Values.Add(new ProjectValue(task.UniversalName, name, "SomeOtherValue"));
            state.SetValue(task, name, value);
            var actual = GetValueFromState(state, task, name);
            Assert.IsNotNull(actual);
            Assert.AreEqual(task.UniversalName, actual.Owner);
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(value, actual.Value);
        }

        [Test]
        public void SetValueRemovesValueIfNull()
        {
            var name = "TestItem";
            var task = new Comment();
            var state = new PersistedProjectState();
            state.Values.Add(new ProjectValue
                                 {
                                     Owner = task.UniversalName,
                                     Name = name,
                                     Value = "SomeOtherValue"
                                 });
            state.SetValue(task, name, null);
            var actual = GetValueFromState(state, task, name);
            Assert.IsNull(actual);
        }

        [Test]
        public void GetValueReturnsValueIfItExists()
        {
            var name = "TestItem";
            var value = "TestValue";
            var task = new Comment();
            var state = new PersistedProjectState();
            state.Values.Add(new ProjectValue(task.UniversalName, name, value));
            var actual = state.GetValue(task, name);
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void GetValueReturnsNullIfValueDoesNotExist()
        {
            var name = "TestItem";
            var task = new Comment();
            var state = new PersistedProjectState();
            var actual = state.GetValue(task, name);
            Assert.IsNull(actual);
        }

        [Test]
        public void StateCanBePersisted()
        {
            var task = new Comment();
            var state = new PersistedProjectState
                            {
                                LastIntegration = new IntegrationSummary
                                                      {
                                                          FinishTime = new DateTime(2010, 1, 1, 12, 1, 2),
                                                          StartTime = new DateTime(2010, 1, 1, 12, 1, 1),
                                                          Status = IntegrationStatus.Success
                                                      }
                            };
            state.SetValue(task, "Test", "Value");
            string expected;
            using (var stream = AssemblyHelper.RetrieveExampleFile("ExampleProjectState"))
            {
                using (var reader = new StreamReader(stream))
                {
                    expected = reader.ReadToEnd();
                }
            }

            using (var stream = new MemoryStream())
            {
                state.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var xaml = reader.ReadToEnd();
                    Assert.AreEqual(expected, xaml);
                }
            }
        }
        #endregion

        #region Helper methods
        private static ProjectValue GetValueFromState(PersistedProjectState state, ProjectItem task, string name)
        {
            var value = state.Values.SingleOrDefault(v => v.Owner == task.UniversalName && v.Name == name);
            return value;
        }
        #endregion
    }
}
