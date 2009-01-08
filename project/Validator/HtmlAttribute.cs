using System;

namespace Validator
{
    public class HtmlAttribute
    {
        public HtmlAttribute() { }
        public HtmlAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name = string.Empty;
        public string Value = string.Empty;
    }
}
