namespace ThoughtWorks.CruiseControl.UnitTests.CodeChecks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exortech.NetReflector;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.WebDashboard.Configuration;

    [TestFixture]
    public class ReflectorChecks
    {
        #region Tests
        #region Obsolete tests
        [Test]
        public void RemoteHasNoObsoleteAttributes()
        {
            this.CheckForObsolete(typeof(BuildCondition).Assembly);
        }

        [Test]
        public void CoreHasNoObsoleteAttributes()
        {
            this.CheckForObsolete(typeof(Project).Assembly);
        }

        [Test]
        public void WebDashboardHasNoObsoleteAttributes()
        {
            this.CheckForObsolete(typeof(DashboardConfigurationLoader).Assembly);
        }
        #endregion

        #region Fields as reflection property tests
        [Test]
        public void RemoteHasNoFieldsForReflection()
        {
            this.CheckForFieldAsReflectorProperty(typeof(BuildCondition).Assembly);
        }

        [Test]
        public void CoreHasNoFieldsForReflection()
        {
            this.CheckForFieldAsReflectorProperty(typeof(Project).Assembly);
        }

        [Test]
        public void WebDashboardHasNoFieldsForReflection()
        {
            this.CheckForFieldAsReflectorProperty(typeof(DashboardConfigurationLoader).Assembly);
        }
        #endregion
        #endregion

        #region Helpers
        #region CheckForObsolete()
        /// <summary>
        /// Checks an assembly and makes sure there are no obsolete NetReflectorAttributes being used.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        private void CheckForObsolete(Assembly assembly)
        {
            var failedTypes = new List<string>();
            foreach (var type in assembly.GetTypes().OrderBy(t => t.FullName))
            {
                if (this.HasAttribute(typeof(ReflectorTypeAttribute), type))
                {
                    foreach (var property in type.GetProperties().OrderBy(p => p.Name))
                    {
                        if (this.HasAttribute(typeof(ReflectorArrayAttribute), property) ||
                            this.HasAttribute(typeof(ReflectorCollectionAttribute), property) ||
                            this.HasAttribute(typeof(ReflectorHashAttribute), property))
                        {
                            failedTypes.Add(type.FullName);
                            break;
                        }
                    }
                }
            }

            if (failedTypes.Count > 0)
            {
                var message = "The following " + 
                    failedTypes.Count.ToString() + 
                    " type(s) use an obsolete NetReflector attribute " +
                    "(ReflectorArray, ReflectorCollection or ReflectorHash)" +
                    Environment.NewLine +
                    "* " + 
                    string.Join(Environment.NewLine + "* ", failedTypes.ToArray());
                Assert.Fail(message);
            }
        }
        #endregion

        #region CheckForFieldAsReflectorProperty()
        /// <summary>
        /// Checks an assembly and makes sure there are no fields marked for reflection.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        private void CheckForFieldAsReflectorProperty(Assembly assembly)
        {
            var failedTypes = new List<string>();
            foreach (var type in assembly.GetTypes().OrderBy(t => t.FullName))
            {
                if (this.HasAttribute(typeof(ReflectorTypeAttribute), type))
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(p => p.Name);
                    foreach (var field in fields)
                    {
                        if (this.HasAttribute(typeof(ReflectorArrayAttribute), field) ||
                            this.HasAttribute(typeof(ReflectorCollectionAttribute), field) ||
                            this.HasAttribute(typeof(ReflectorPropertyAttribute), field) ||
                            this.HasAttribute(typeof(ReflectorHashAttribute), field))
                        {
                            failedTypes.Add(type.FullName);
                            break;
                        }
                    }
                }
            }

            if (failedTypes.Count > 0)
            {
                var message = "The following " +
                    failedTypes.Count.ToString() +
                    " type(s) expose fields for reflection (should be properties)" +
                    Environment.NewLine +
                    "* " +
                    string.Join(Environment.NewLine + "* ", failedTypes.ToArray());
                Assert.Fail(message);
            }
        }
        #endregion

        #region HasAttribute()
        /// <summary>
        /// Determines whether the specified type has an attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the specified type has the attribute; otherwise, <c>false</c>.
        /// </returns>
        private bool HasAttribute(Type attribute, Type type)
        {
            var hasAttrib = type.GetCustomAttributes(attribute, true).Length > 0;
            return hasAttrib;
        }

        /// <summary>
        /// Determines whether the specified property has an attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// <c>true</c> if the specified property has the attribute; otherwise, <c>false</c>.
        /// </returns>
        private bool HasAttribute(Type attribute, PropertyInfo property)
        {
            var hasAttrib = property.GetCustomAttributes(attribute, true).Length > 0;
            return hasAttrib;
        }

        /// <summary>
        /// Determines whether the specified field has an attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        /// <c>true</c> if the specified field has the attribute; otherwise, <c>false</c>.
        /// </returns>
        private bool HasAttribute(Type attribute, FieldInfo field)
        {
            var hasAttrib = field.GetCustomAttributes(attribute, true).Length > 0;
            return hasAttrib;
        }
        #endregion
        #endregion
    }
}
