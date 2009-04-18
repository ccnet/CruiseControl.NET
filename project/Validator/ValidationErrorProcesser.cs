using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Config;
using System.Xml;
using System.Windows.Forms;

namespace Validator
{
    public class ValidationErrorProcesser
        : IConfigurationErrorProcesser
    {
        private WebBrowser browser;
        private HtmlElement bodyEl;

        public ValidationErrorProcesser(WebBrowser browser)
        {
            this.browser = browser;
            this.bodyEl = browser.Document.Body;
            Passed = true;
        }

        public bool Passed { get; private set; }
        public string ItemName { get; set; }

        public void ProcessError(string message)
        {
            bodyEl.AppendChild(
                GenerateElement("div",
                new HtmlAttribute("class", "error"),
                GenerateElement("div", 
                    string.Format("ERROR ({0}): {1}", 
                        ItemName,
                        message))));
            Passed = false;
        }

        public void ProcessError(Exception error)
        {
            bodyEl.AppendChild(
                GenerateElement("div",
                new HtmlAttribute("class", "error"),
                GenerateElement("div", 
                    string.Format("ERROR ({0}): {1}",
                        ItemName,
                        error.Message))));
            Passed = false;
        }

        public void ProcessWarning(string message)
        {
            bodyEl.AppendChild(
                GenerateElement("div",
                new HtmlAttribute("class", "warning"),
                GenerateElement("div", 
                    string.Format("Warning ({0}): {1}",
                        ItemName,
                        message))));
            Passed = false;
        }

        public bool ProcessUnhandledNode(XmlNode node)
        {
            return false;
        }

        private HtmlElement GenerateElement(string tagName, params object[] contents)
        {
            HtmlElement element = browser.Document.CreateElement(tagName);
            foreach (object contentEl in contents)
            {
                if (contentEl is HtmlElement)
                {
                    element.AppendChild(contentEl as HtmlElement);
                }
                else if (contentEl is HtmlAttribute)
                {
                    HtmlAttribute attrbite = contentEl as HtmlAttribute;
                    element.SetAttribute(attrbite.Name, attrbite.Value);
                }
                else
                {
                    HtmlElement spanEl = browser.Document.CreateElement("span");
                    spanEl.InnerText = contentEl.ToString();
                    element.AppendChild(spanEl);
                }
            }
            return element;
        }
    }
}
