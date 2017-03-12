using System;
using System.Collections.Generic;

using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

using NewsletterStudio;

namespace FormsExt.Workflows.NewsletterStudio
{
    public class Subscribe : WorkflowType
    {
        [Setting("Mailing List Id", prevalues = "", description = "Numeric mailing list id (i.e. '2')", view = "textstring")]
        public string MailingListId { get; set; }

        [Setting("E-Mail Field Name", prevalues = "email", description = "The name of the field containing the e-mail address (i.e. 'E-Mail')", view = "textstring")]
        public string EmailFieldName { get; set; }

        public Subscribe()
        {
            // - GUID, name et al setup
            this.Id = new Guid("57c9b164-c255-4724-a384-29fb6de73d0c");
            this.Name = "Subscribe to Newsletter Studio mailing list";
            this.Description = "Adds an E-Mail to a mailing list within Newsletter Studio";
            this.Icon = "icon-inbox";
        }

        public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
        {
            try
            {
                // - get the email value and subscribe
                string email = record.GetRecordField(EmailFieldName).ValuesAsString();

                if (Api.IsValidEmail(email))
                {
                    Api.Subscribe(email, int.Parse(this.MailingListId));
                }

                // - all good
                return WorkflowExecutionStatus.Completed;
            }
            catch (Exception ex)
            {
                // - that didn't work out: log it and bubble up failed execution
                Umbraco.Core.Logging.LogHelper.WarnWithException(this.GetType(), string.Format("Unable to subscribe e-mail (EmailFieldName: '{1}', MailingListId: '{2}'): {0}", ex.Message, EmailFieldName, MailingListId), ex);
                return WorkflowExecutionStatus.Failed;
            }
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptions = new List<Exception>();

            int listid = -1;
            int.TryParse(this.MailingListId, out listid);
            if (listid == -1)
            {
                exceptions.Add(new Exception("Mailing list id is expected to be numeric (int)"));
            }

            return exceptions;
        }
    }
}