using Exortech.NetReflector;
using System;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    [ReflectorType("defaultIssueTracker")]
    public class DefaultIssueTrackerUrlBuilder : IModificationUrlBuilder
    {
        private string _url;

        [ReflectorProperty("url")]
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public void SetupModification(Modification[] modifications)
        {
            if (modifications == null) throw new ArgumentNullException("modifications");

            foreach (Modification mod in modifications)
            {
                //split the comment on a space, take the first part
                //this must be the issue ID
                //from the last position of this part, go back while the characters are numeric                

                if ((mod != null) && !string.IsNullOrEmpty(mod.Comment))
                {
                    string SearchingComment = mod.Comment.Split(' ')[0];
                    int EndPosition = SearchingComment.Length - 1;
                    char CurrentChar = SearchingComment[EndPosition];
                    string Result = string.Empty;
                    bool NumericPartFound = false;

                    //eliminate non numeric characters at the end (ex type  [ccnet-1500])
                    while (EndPosition > 0 && !char.IsNumber(CurrentChar))
                    {
                        EndPosition--;
                        CurrentChar = SearchingComment[EndPosition];
                    }


                    //while last position is numeric add to result
                    while (EndPosition >= 0 && char.IsNumber(CurrentChar))
                    {
                        Result = Result.Insert(0, CurrentChar.ToString());
                        EndPosition--;
                        if (EndPosition >= 0 ) CurrentChar = SearchingComment[EndPosition];
                        
                        NumericPartFound = true;
                    }




                    if (NumericPartFound)
                    {
                        mod.IssueUrl = string.Format(_url, Result);    
                    }
                }                
            }
        }

    }
}
