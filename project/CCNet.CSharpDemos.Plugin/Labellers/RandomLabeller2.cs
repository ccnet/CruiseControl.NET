namespace CCNet.CSharpDemos.Plugin.Labellers
{
    using System;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Label;

    [ReflectorType("randomLabeller2")]
    public class RandomLabeller2
        : LabellerBase, IConfigurationValidation
    {
        public RandomLabeller2()
        {
            this.MaximumValue = int.MaxValue;
        }

        [ReflectorProperty("max", Required = false)]
        public int MaximumValue { get; set; }

        public override string Generate(IIntegrationResult integrationResult)
        {
            var rand = new Random();
            var label = rand.Next(this.MaximumValue).ToString();
            return label;
        }

        public void Validate(IConfiguration configuration,
            ConfigurationTrace parent,
            IConfigurationErrorProcesser errorProcesser)
        {
            if (this.MaximumValue <= 0)
            {
                errorProcesser.ProcessError(
                    "The maximum value must be greater than zero");
            }
        }
    }
}
