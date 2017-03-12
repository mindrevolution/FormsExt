using System;
using System.Collections.Generic;

using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace FormsExt.Workflows
{
    public class Redirect : WorkflowType
    {
        [Setting("Target", prevalues = "", description = "Target page to redirect user to", view = "pickers.content")]
        public dynamic TargetNode { get; set; }
        [Setting("Append querystring values", prevalues = "", description = "Copies all querystring parameters of the original page to the redirect target page", view = "checkbox")]
        public string AppendQuerystringParams { get; set; }
        [Setting("Append hidden field values", prevalues = "", description = "Appends the values of all hidden fields to the querystring of the target page", view = "checkbox")]
        public string AppendHiddenFields { get; set; }

        public Redirect()
        {
            // - GUID, name et al setup
            this.Id = new Guid("ae5f66c3-b537-443d-a447-8ae293f1f583");
            this.Name = "Redirect";
            this.Description = "Redirects the current user to the selected page";
            this.Icon = "icon-directions-alt";
        }

        public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
        {
            // - I'm just a kind of placeholder workflow ...
            return WorkflowExecutionStatus.Completed;
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptions = new List<Exception>();

            if (TargetNode == null)
            {
                exceptions.Add(new Exception("Please select a target page."));
            }
            if (!AppendQuerystringParams.Equals("True", StringComparison.InvariantCultureIgnoreCase))
            {
                AppendQuerystringParams = "False";
            }

            return exceptions;
        }
    }
}