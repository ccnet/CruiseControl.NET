namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class IfNDefProcessor : IfDefProcessor
    {
        public IfNDefProcessor(PreprocessorEnvironment env)
            : base(env._Settings.Namespace.GetName("ifndef"), env)
        {
        }

        protected override bool _TestCondition(string name)
        {
            return !base._TestCondition( name );
        }
    }
}
