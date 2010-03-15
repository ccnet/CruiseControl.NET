using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    public partial class BuildParameters : Form
    {
        private MainFormController myController;
        private Dictionary<string, ParameterBase> myParameters;
        private object myParameterHolder;

        public BuildParameters(MainFormController controller, List<ParameterBase> buildParameters)
        {
            InitializeComponent();
            myController = controller;
            myParameters = new Dictionary<string, ParameterBase>();
            foreach (ParameterBase parameter in buildParameters)
            {
                myParameters.Add(parameter.Name, parameter);
            }
            myParameterHolder = GenerateParametersClass();
            ApplyParameters();
            parameters.SelectedObject = myParameterHolder;
        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            myController.ForceBuild(RetrieveParameters());
            this.Close();
        }

        private void ApplyParameters()
        {
            Type holderType = myParameterHolder.GetType();
            foreach (ParameterBase parameter in myParameters.Values)
            {
                if (parameter.ClientDefaultValue != null)
                {
                    PropertyInfo property = holderType.GetProperty(parameter.Name);
                    object dataValue;
                    if (property.PropertyType.IsEnum)
                    {
                        dataValue = Enum.Parse(property.PropertyType, parameter.ClientDefaultValue);
                    }
                    else
                    {
                        dataValue = Convert.ChangeType(parameter.ClientDefaultValue, parameter.DataType);
                    }
                    property.SetValue(myParameterHolder,
                        dataValue,
                        new object[0]);
                }
            }
        }

        /// <summary>
        /// Converts the values in the holder instance to a string dictionary for transmission to the server.
        /// </summary>
        /// <returns>A dictionary of strings containing the non-null values.</returns>
        private Dictionary<string, string> RetrieveParameters()
        {
            var results = new Dictionary<string, string>();
            var holderType = myParameterHolder.GetType();
            foreach (var parameter in myParameters.Values)
            {
                // Retrieve the property value from the holder
                var property = holderType.GetProperty(parameter.Name);
                var propertyValue = property.GetValue(myParameterHolder, new object[0]);

                // Convert the value to a string for transmission
                if (propertyValue != null)
                {
                    var propertyValueString = propertyValue.ToString();

                    // Check if the parameter performs any internal conversion
                    var convertedValue = parameter.Convert(propertyValueString);
                    if (convertedValue != null)
                    {
                        propertyValueString = convertedValue.ToString();
                    }

                    // Finally add to the results
                    results.Add(parameter.Name, propertyValueString); 
                }
            }

            return results;
        }

        private object GenerateParametersClass()
        {
            AssemblyName tempAssembly = new AssemblyName("param" + Guid.NewGuid().ToString());
            AssemblyBuilder assemblybuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                tempAssembly,
                AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemblybuilder.DefineDynamicModule(tempAssembly.Name);
            TypeBuilder typeBuilder = moduleBuilder.DefineType("ProjectParameters", TypeAttributes.Public);
            foreach (ParameterBase parameter in myParameters.Values)
            {
                GenerateProperty(typeBuilder, parameter, moduleBuilder);
            }
            Type resultType = typeBuilder.CreateType();
            object result = Activator.CreateInstance(resultType);
            return result;
        }

        private void GenerateProperty(TypeBuilder typeBuilder, ParameterBase parameter, ModuleBuilder moduleBuilder)
        {
            Type dataType = parameter.DataType;
            if ((parameter.AllowedValues != null) && (parameter.AllowedValues.Length > 0))
            {
                dataType = GenerateEnumeration(parameter.Name + "Enum", parameter.AllowedValues, moduleBuilder);
            }
            FieldBuilder fieldBuilder = typeBuilder.DefineField(parameter.Name + "Field", dataType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(parameter.Name, PropertyAttributes.HasDefault, dataType, null);
            MethodBuilder propertyGetBuilder = typeBuilder.DefineMethod("get_" + parameter.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                dataType,
                null);
            ILGenerator getGenerator = propertyGetBuilder.GetILGenerator();
            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getGenerator.Emit(OpCodes.Ret);
            MethodBuilder propertySetBuilder = typeBuilder.DefineMethod("get_" + parameter.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                new Type[] { dataType });
            ILGenerator setGenerator = propertySetBuilder.GetILGenerator();
            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            setGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            setGenerator.Emit(OpCodes.Ret);
            AssociateAttribute(propertyBuilder, typeof(DescriptionAttribute), parameter.Description);
            AssociateAttribute(propertyBuilder, typeof(DisplayNameAttribute), parameter.DisplayName);
            propertyBuilder.SetGetMethod(propertyGetBuilder);
            propertyBuilder.SetSetMethod(propertySetBuilder);
        }

        private Type GenerateEnumeration(string name, string[] values, ModuleBuilder moduleBuilder)
        {
            EnumBuilder enumBuilder = moduleBuilder.DefineEnum(name, TypeAttributes.Public, typeof(int));
            int position = 0;
            foreach (string value in values)
            {
                enumBuilder.DefineLiteral(value, position++);
            }
            return enumBuilder.CreateType();
        }

        private void AssociateAttribute(PropertyBuilder propertyBuilder, Type attributeType, string value)
        {
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(
                attributeType.GetConstructor(new Type[] { typeof(string) }),
                new object[] { value });
            propertyBuilder.SetCustomAttribute(attributeBuilder);
        }

        private void parameters_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string item = e.ChangedItem.PropertyDescriptor.Name;
            Exception[] errors = myParameters[item].Validate(e.ChangedItem.Value.ToString());
            if (errors.Length > 0)
            {
                StringBuilder message = new StringBuilder();
                message.Append("The following errors occurred with this value:");
                foreach (Exception error in errors)
                {
                    message.Append(Environment.NewLine + error.Message);
                }
                MessageBox.Show(message.ToString(), "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.ChangedItem.PropertyDescriptor.SetValue(null, e.OldValue);
            }
        }
    }
}
