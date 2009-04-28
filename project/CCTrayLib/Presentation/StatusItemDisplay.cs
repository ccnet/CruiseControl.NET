using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    public class StatusItemDisplay
    {
        private ItemStatus item;

        public StatusItemDisplay(ItemStatus value)
        {
            this.item = value;
        }

        [Description("The name of the item")]
        public string Name
        {
            get { return item.Name; }
            set { }
        }

        [Description("The description of the item")]
        public string Description
        {
            get { return item.Description; }
            set { }
        }

        [Description("The current status of the item")]
        public string Status
        {
            get
            {
                switch (item.Status)
                {
                    case ItemBuildStatus.CompletedFailed:
                        return "Completed - failed!";
                    case ItemBuildStatus.CompletedSuccess:
                        return "Completed - Success!";
                    default:
                        return item.Status.ToString();
                }
            }
            set { }
        }

        [DisplayName("Time Started")]
        [Description("The date and time the item started running")]
        public string TimeStarted
        {
            get { return item.TimeStarted.HasValue ? item.TimeStarted.Value.ToString("F") : string.Empty; }
            set { }
        }

        [DisplayName("Time Completed")]
        [Description("The description of the item")]
        public string TimeCompleted
        {
            get { return item.TimeCompleted.HasValue ? item.TimeCompleted.Value.ToString("F") : string.Empty; }
            set { }
        }
    }
}
