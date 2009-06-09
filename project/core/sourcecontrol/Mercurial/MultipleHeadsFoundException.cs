namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    public class MultipleHeadsFoundException : CruiseControlException
    {
        public MultipleHeadsFoundException() : base("Multiple Heads found in repository, couldn't choose one to update to.") { }
    }
}
