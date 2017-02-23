using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Data.Storage;

namespace FormsExt.Fieldtypes
{
    public class Consent : FieldType
    {
        [Setting("Text", prevalues = "", description = "Geben Sie hier Hinweis- und Erklärungstexte an.", view = "textarea")]
        public string Text { get; set; }

        [Setting("Datenschutzerklärung anfügen", prevalues = "", description = "Fügt einen Standardhinweis zur Datenschutzerklärung hinzu.", view = "checkbox")]
        public string PrivacyStatement { get; set; }

        public Consent()
        {
            this.Id = new Guid("57696a35-f1e6-4fab-af4a-ad7f7ad2c235");
            this.Name = "Consent";
            this.Description = "Informiert über Bedingungen und Zustimmung.";
            this.Icon = "icon-legal";
            this.DataType = FieldDataType.Bit;
            this.SortOrder = 10;
        }

        // - custom value
        public override IEnumerable<object> ProcessSubmittedValue(Field field, IEnumerable<object> postedValues, HttpContextBase context)
        {
            List<object> returnValue = new List<object>();
            if (postedValues.ToList().Count > 0)
            {
                returnValue.Add("true");
            }
            else
            {
                returnValue.Add("false");
            }
            return returnValue;

        }
    }
}