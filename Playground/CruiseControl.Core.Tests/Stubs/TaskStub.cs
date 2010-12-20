namespace CruiseControl.Core.Tests.Stubs
{
    using System;
    using System.Collections.Generic;

    public class TaskStub
        : Task
    {
        public Action OnValidateAction { get; set; }
        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.OnValidateAction != null)
            {
                this.OnValidateAction();
            }
        }

        public Action OnInitialiseAction { get; set; }
        protected override void OnInitialise()
        {
            base.OnInitialise();
            if (this.OnInitialiseAction != null)
            {
                this.OnInitialiseAction();
            }
        }

        public Func<TaskExecutionContext, IEnumerable<Task>> OnRunAction { get; set; }
        protected override IEnumerable<Task> OnRun(TaskExecutionContext context)
        {
            if (this.OnRunAction != null)
            {
                return this.OnRunAction(context);
            }

            throw new NotImplementedException();
        }

        public Action OnCleanUpAction { get; set; }
        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            if (this.OnCleanUpAction != null)
            {
                this.OnCleanUpAction();
            }
        }
    }
}
