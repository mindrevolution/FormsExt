using System;
using System.Collections.Generic;

using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace FormsExt.Workflows.Members
{
    public class AddMember : WorkflowType
    {
        [Setting("E-Mail Field Name", prevalues = "email", description = "The name of the field containing the e-mail address (i.e. 'E-Mail')", view = "textstring")]
        public string EmailFieldName { get; set; }

        [Setting("Member Type", prevalues = "", description = "The member typ's alias that is used to create the member (if the member already exists, the existing one's type is not changed)", view = "textstring")]
        public string MemberType { get; set; }

        [Setting("Member Group(s)", prevalues = "", description = "The group(s) to add the member to (i.e. 'Newsletter' or 'Newsletter;CustomerDiscounts;TrustedShops')", view = "textstring")]
        public string MemberGroups { get; set; }

        public AddMember()
        {
            // - GUID, name et al setup
            this.Id = new Guid("dafdce36-5393-4169-8aa3-43e71f5dd41e");
            this.Name = "Add member";
            this.Description = "Creates a new member (or updates existing) for this email and adds to group(s).";
            this.Icon = "icon-user";
        }

        public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
        {
            try
            {
                // - get the email value and subscribe
                string email = record.GetRecordField(EmailFieldName).ValuesAsString();

                if (Helpers.Net.IsValidEmail(email))
                {
                    IMemberService members = ApplicationContext.Current.Services.MemberService;
                    IMember member = null;

                    // -- create user (if not exists)
                    int foundrecords = 0;
                    members.FindByEmail(email, 1, 1, out foundrecords, Umbraco.Core.Persistence.Querying.StringPropertyMatchType.Exact);
                    if (foundrecords==0)
                    {
                        // - add member with random password!
                        member = members.CreateWithIdentity(email, email, Guid.NewGuid().ToString(), MemberType);

                        // - generate tracking token if there is a prop on the member for it
                        if (member.HasProperty("trackingToken"))
                        {
                            member.SetValue("trackingToken", Helpers.Tokens.GenerateTrackingToken(email));
                            members.Save(member);
                        }
                    }

                    // - add to group
                    member = members.GetByEmail(email);
                    MemberGroups = MemberGroups.TrimEnd(";") + ";";
                    foreach (string group in MemberGroups.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(group))
                        {
                            //Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("ASSIGN ROLE (member.Id: '{0}', group: '{1}')", member.Id, group));
                            members.AssignRole(member.Id, group);
                            members.Save(member);
                        }
                    }
                }

                // - all good
                return WorkflowExecutionStatus.Completed;
            }
            catch (Exception ex)
            {
                // - that didn't work out: log it and bubble up failed execution
                Umbraco.Core.Logging.LogHelper.WarnWithException(this.GetType(), string.Format("Unable add member (EmailFieldName: '{1}', MemberType: '{2}', MemberGroups: '{3}'): {0}", ex.Message, EmailFieldName, MemberType, MemberGroups), ex);
                return WorkflowExecutionStatus.Failed;
            }
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptions = new List<Exception>();

            if (string.IsNullOrWhiteSpace(EmailFieldName))
            {
                exceptions.Add(new Exception("Email field name is required"));
            }

            if (string.IsNullOrWhiteSpace(MemberType))
            {
                exceptions.Add(new Exception("Member type is required"));
            }

            return exceptions;
        }
    }
}