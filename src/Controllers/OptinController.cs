using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco;
using Umbraco.Core;
using Umbraco.Forms.Core;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.UI;
using Umbraco.Forms.Core.Enums;
using Umbraco.Core.Logging;

namespace FormsExt.Controllers
{
    [PluginController("FormsExt")]
    public class NewsletterOptinController : Umbraco.Web.Mvc.SurfaceController
    {
        private RecordField BuildDataField(Field f, Guid key, string alias, string caption, int recordId, string value)
        {
            f.Id = Guid.NewGuid();
            f.Alias = alias;
            f.Caption = caption;

            return new RecordField()
            {
                Alias = f.Alias,
                FieldId = f.Id,
                Field = f,
                Key = key,
                Record = recordId,
                Values = new List<object>() { value }
            }; ;
        }

        public ActionResult Activate(string optin)
        {
            bool fail = false;
            string redirect = string.Empty;

            if (!string.IsNullOrWhiteSpace(optin))
            {
                try
                {
                    Guid recordGuid = new Guid(optin);
                    if (recordGuid != null)
                    {
                        RecordStorage recordStorage = new RecordStorage();
                        FormStorage formStorage = new FormStorage();

                        Record record = recordStorage.GetRecordByUniqueId(recordGuid);
                        // get corresponding form for this record
                        Umbraco.Forms.Core.Form form = formStorage.GetForm(record.Form);

                        // - approve this record, so that form approval workflow kicks in!
                        RecordService.Instance.Approve(record, form);

                        // - redirect to target page (if configured)
                        WorkflowStorage workflowStorage = new WorkflowStorage();
                        foreach (var workflow in workflowStorage.GetAllWorkFlows(form))
                        {
                            // - find the FormsExt.Workflows.Redirect workflow item (if added/configured)
                            if (workflow.ExecutesOn==FormState.Approved && workflow.Active && workflow.Type.ToString().Equals("FormsExt.Workflows.Redirect", StringComparison.InvariantCultureIgnoreCase))
                            {
                                // - only act on this workflow step if these settings exist!
                                if (!string.IsNullOrEmpty(workflow.Settings["TargetNode"]))
                                {
                                    int targetnodeId = -1;
                                    int.TryParse(workflow.Settings["TargetNode"], out targetnodeId);

                                    if (targetnodeId!=-1)
                                    {
                                        // - read settings
                                        bool appendQuerystringParams = false;
                                        if (workflow.Settings.ContainsKey("AppendQuerystringParams"))
                                        {
                                            bool.TryParse(workflow.Settings["AppendQuerystringParams"], out appendQuerystringParams);
                                        }
                                        bool appendHiddenFields = false;
                                        if (workflow.Settings.ContainsKey("AppendHiddenFields"))
                                        {
                                            bool.TryParse(workflow.Settings["AppendHiddenFields"], out appendHiddenFields);
                                        }

                                        redirect += "?";

                                        // - attach querystring params?
                                        if (appendQuerystringParams)
                                        {
                                            foreach (string param in Request.QueryString.Keys)
                                            {
                                                redirect += param + "=" + Server.UrlEncode(Request.QueryString[param]) + "&";
                                            }
                                        }

                                        // - attach hidden fields?
                                        if (appendHiddenFields)
                                        {
                                            foreach (Guid key in record.RecordFields.Keys)
                                            {
                                                RecordField recfield;
                                                record.RecordFields.TryGetValue(key, out recfield);
                                                if (recfield!=null)
                                                {
                                                    foreach (var val in recfield.Values)
                                                    {
                                                        if (recfield.Field.FieldType.Name.Equals("Hidden", StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            // - this is a hidden field, add to params
                                                            redirect += recfield.Field.Alias + "=" + Server.UrlEncode(recfield.ValuesAsString()) + "&";
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        // - get page url ("nice url")
                                        string pageurl = Umbraco.TypedContent(targetnodeId).UrlAbsolute();

                                        redirect = redirect.Trim("&");
                                        redirect = pageurl + redirect;

                                        fail = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        fail = true;
                    }

                }
                catch (Exception ex)
                {
                    fail = true;
                    LogHelper.WarnWithException(this.GetType(), string.Format("Unable opt-in via '{1}': {0}", ex.Message, Request.QueryString.ToString()), ex);
                }
            }
            else
            {
                fail = true;
                LogHelper.Warn(this.GetType(), string.Format("Unable opt-in via '{1}': No opt-in token given", Request.QueryString.ToString()));
            }

            if (fail)
            {
                string subscribeErrorUrl = System.Configuration.ConfigurationManager.AppSettings["FormsExt:NewsletterSubscribeError"];
                if (!string.IsNullOrWhiteSpace(subscribeErrorUrl))
                {
                    System.Web.HttpContext.Current.Response.Redirect(subscribeErrorUrl, false);
                    return null;
                }
                else
                {
                    return Content("<h1>Sorry, that didn't work out.</h1>");
                }
            }
            else
            {
                // - redirect if URL provided
                if (!string.IsNullOrWhiteSpace(redirect))
                {
                    Response.Redirect(redirect, false);
                    return null;
                }
                else
                {
                    return Content("<h1>👍</h1>");
                }
            }

        }
    }
}
