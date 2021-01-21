using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// 	
    /// </summary>
	public class XslFilesSectionHandler : ConfigurationSection
	{
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(XslFilesCollection), AddItemName = "file")]
        public XslFilesCollection XslFiles
        {
            get
            {
                return  (XslFilesCollection)base[""];
            }
        }
        
        public List<string> FileNames 
        {
            get
            {
                var result = new List<string>();
    			foreach (var file in XslFiles) 
            		result.Add(((XslFileElement)file).Name);
                    
                return result;
            }
        }
	}

    public class XslFilesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new XslFileElement();
        }
    
        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((XslFileElement)element).Name;
        }
    
        public XslFileElement this[int index]
        {
            get
            {
                return (XslFileElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }
    
    public class XslFileElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
    }
}

