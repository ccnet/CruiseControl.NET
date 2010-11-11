using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Defines the status on an item.
    /// </summary>
    [Serializable]
    [XmlRoot("itemStatus")]
    public class ItemStatus
    {
        #region Private fields
        private Guid identifier = Guid.NewGuid();
        private string name;
        private ItemBuildStatus status = ItemBuildStatus.Unknown;
        private DateTime? timeStarted;
        private DateTime? timeCompleted;
        private DateTime? timeOfEstimatedCompletion;
        private string description;
        private List<ItemStatus> childItems = new List<ItemStatus>();
        private ItemStatus parent;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new blank <see cref="ItemStatus"/>.
        /// </summary>
        public ItemStatus()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="ItemStatus"/> with a name.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        public ItemStatus(string name)
        {
            this.name = name;
        }
        #endregion

        #region Public properties
        #region Identifier
        /// <summary>
        /// The unique identifier of the item.
        /// </summary>
        [XmlAttribute("identifier")]
        public Guid Identifier
        {
            get { return identifier; }
        }
        #endregion

        #region Name
        /// <summary>
        /// The name of the item.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region Description
        /// <summary>
        /// The name of the item.
        /// </summary>
        [XmlElement("description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        #endregion

        #region Error
        /// <summary>
        /// Any error message for why the item failed.
        /// </summary>
        [XmlElement("error")]
        public string Error { get; set; }
        #endregion

        #region Status
        /// <summary>
        /// The status of the item.
        /// </summary>
        [XmlAttribute("status")]
        public ItemBuildStatus Status
        {
            get { return status; }
            set { status = value; }
        }
        #endregion

        #region TimeStarted
        /// <summary>
        /// The date and time the item started building.
        /// </summary>
        [XmlElement("timeStarted")]
        public DateTime? TimeStarted
        {
            get { return timeStarted; }
            set { timeStarted = value; }
        }
        #endregion

        #region TimeCompleted
        /// <summary>
        /// The date and time the item completed building.
        /// </summary>
        [XmlElement("timeCompleted")]
        public DateTime? TimeCompleted
        {
            get { return timeCompleted; }
            set { timeCompleted = value; }
        }
        #endregion

        #region TimeOfEstimatedCompletion
        /// <summary>
        /// The date and time the item is estimated to complete building.
        /// </summary>
        [XmlElement("timeOfEstimatedCompletion")]
        public DateTime? TimeOfEstimatedCompletion
        {
            get { return timeOfEstimatedCompletion; }
            set { timeOfEstimatedCompletion = value; }
        }
        #endregion

        #region ChildItems
        /// <summary>
        /// Any child status items.
        /// </summary>
        [XmlArray("childItems")]
        [XmlArrayItem("childItem")]
        public List<ItemStatus> ChildItems
        {
            get { return childItems; }
        }
        #endregion

        #region Parent
        /// <summary>
        /// The parent of this item.
        /// </summary>
        [XmlIgnore()]
        public ItemStatus Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region AddChild()
        /// <summary>
        /// Adds a child and correctly links it.
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(ItemStatus child)
        {
            child.parent = this;
            childItems.Add(child);
        }
        #endregion

        #region Clone()
        /// <summary>
        /// Generates a clone of this status.
        /// </summary>
        /// <returns></returns>
        public virtual ItemStatus Clone()
        {
            ItemStatus clone = new ItemStatus();
            CopyTo(clone);
            return clone;
        }
        #endregion

        #region CopyTo()
        /// <summary>
        /// Copies this item to another.
        /// </summary>
        /// <param name="value"></param>
        public virtual void CopyTo(ItemStatus value)
        {
            value.identifier = this.identifier;
            value.description = this.description;
            value.name = this.name;
            value.status = this.status;
            value.Error = this.Error;
            value.timeCompleted = this.timeCompleted;
            value.timeOfEstimatedCompletion = this.timeOfEstimatedCompletion;
            value.timeStarted = this.timeStarted;
            foreach (ItemStatus item in this.childItems)
            {
                value.AddChild(item.Clone());
            };
        }
        #endregion

        #region GetHashCode()
        /// <summary>
        /// Gets the hash code for the status.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return identifier.GetHashCode();
        }
        #endregion

        #region Equals()
        /// <summary>
        /// Checks if two statuses are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var dummy = obj as ItemStatus;

            if (dummy != null)
            {
                bool areEqual = identifier.Equals((dummy).identifier);
                return areEqual;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region ToString()
        /// <summary>
        /// Returns an XML representation of the status.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            XmlSerializer serialiser = new XmlSerializer(this.GetType());
            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = UTF8Encoding.UTF8;
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(builder, settings);
            serialiser.Serialize(writer, this);
            return builder.ToString();
        }
        #endregion
        #endregion
    }
}
