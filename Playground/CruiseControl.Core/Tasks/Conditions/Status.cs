namespace CruiseControl.Core.Tasks.Conditions
{
    using System;
    using System.Windows.Markup;

    [ContentProperty("Value")]
    public class Status
        : TaskCondition
    {
        #region Public properties
        #region Value
        public IntegrationStatus Value { get; set; }
        #endregion
        #endregion

        public override bool Evaluate(TaskExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
