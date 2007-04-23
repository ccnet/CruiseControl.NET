using System;
using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
    // ToDo - test untested bits!
    public class CruiseActionFactory : IActionFactory
    {
        private readonly ObjectSource objectSource;

        public CruiseActionFactory(ObjectSource objectSource)
        {
            this.objectSource = objectSource;
        }

        public IAction Create(IRequest request)
        {
            string actionName = request.FileNameWithoutExtension;

            IAction action = CreateHandler(actionName) as IAction;
            if (action == null)
            {
                return new UnknownActionAction();
            }
            return action;
        }


        public IConditionalGetFingerprintProvider CreateFingerprintProvider(IRequest request)
        {
            try
            {
                IConditionalGetFingerprintProvider fingerprintProvider =
                    CreateHandler(request.FileNameWithoutExtension + "_CONDITIONAL_GET_FINGERPRINT_CHAIN") as
                    IConditionalGetFingerprintProvider;
                return fingerprintProvider;
            }
            catch (ApplicationException)
            {
                return null;
            }
        }

        private object CreateHandler(string actionName)
        {
            // Can probably do something clever with this in CruiseObjectSourceInitialiser
            if (actionName == string.Empty || actionName.ToLower() == "default")
            {
                return objectSource.GetByType(typeof (DefaultAction));
            }

            object action = objectSource.GetByName(actionName);

            return action;
        }
    }
}