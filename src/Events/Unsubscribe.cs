using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

using NewsletterStudio;
using NewsletterStudio.Infrastucture.Events;

namespace FormsExt.Startup
{
    public class RegisterEvents : ApplicationEventHandler
    {

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            NewsletterStudio.Infrastucture.Services.TrackingService.Unsubscribing +=
            delegate (object sender, UnsubscribingEventArgs args)
            {
                try
                {
                    // - is it an unsubscribe process for a member group? we need to handle that manually!
                    if (args.SubscriptionAlias.StartsWith("UmbracoNewSubscriptionProvider_", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        IMemberService members = ApplicationContext.Current.Services.MemberService;
                        IMember member = members.GetByEmail(args.Email);

                        if (member != null)
                        {
                            Umbraco.Core.Logging.LogHelper.Info(this.GetType(), string.Format("Unsubscribe member from newsletter group (Email: '{0}', SubscriptionAlias: '{1}')", args.Email, args.SubscriptionAlias));
                            members.DissociateRole(member.Id, args.SubscriptionAlias.TrimStart("UmbracoNewSubscriptionProvider_"));
                            members.Save(member);

                            // - redirect if URL provided
                            string unsubscribedUrl = System.Configuration.ConfigurationManager.AppSettings["FormsExt:NewsletterUnsubscribedUrl"];
                            if (!string.IsNullOrWhiteSpace(unsubscribedUrl))
                            {
                                System.Web.HttpContext.Current.Response.Redirect(unsubscribedUrl, false);
                            }
                            else
                            {
                                System.Web.HttpContext.Current.Response.ClearContent();
                                System.Web.HttpContext.Current.Response.Write(string.Format("<h1>Successfully unsubscribed '{0}'.</h1>", System.Web.HttpContext.Current.Server.HtmlEncode(args.Email)));
                                
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Umbraco.Core.Logging.LogHelper.Error(this.GetType(), string.Format("Unsubscribe member from newsletter group (Email: '{0}', SubscriptionAlias: '{1}') failed: {2}", args.Email, args.SubscriptionAlias, ex.Message), ex);

                    string errorUrl = System.Configuration.ConfigurationManager.AppSettings["FormsExt:NewsletterUnsubscribeErrorUrl"];
                    if (!string.IsNullOrWhiteSpace(errorUrl))
                    {
                        System.Web.HttpContext.Current.Response.Redirect(errorUrl, false);
                    }
                    else
                    {
                        System.Web.HttpContext.Current.Response.ClearContent();
                        System.Web.HttpContext.Current.Response.Write("<h1>Error unsubscribing. This could be a configuration issue. Please contact the system administrator.</h1>");
                        System.Web.HttpContext.Current.Response.End();
                    }
                }
            };
        }

    }
}
