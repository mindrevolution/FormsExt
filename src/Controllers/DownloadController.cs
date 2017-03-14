using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using System.Text;
using System.Web;
using System.IO;

namespace FormsExt.Controllers
{
    [PluginController("FormsExt")]
    public class DownloadController : Umbraco.Web.Mvc.SurfaceController
    {
        private IMediaService mediaservice = ApplicationContext.Current.Services.MediaService;
        private IMedia mediaMatch = null;

        public ActionResult MediaById(int id, string t)
        {
            IMemberService members = ApplicationContext.Current.Services.MemberService;
            IMember member = null;

            foreach (IMember m in members.GetMembersByPropertyValue("trackingToken", t))
            {
                // - this one matches the tracking code
                member = m;

                // - stream file (if exists)
                IPublishedContent media = Umbraco.TypedMedia(id);
                if (media != null)
                {
                    string mediaFile = Server.MapPath(Umbraco.TypedMedia(media.Id).Url);
                    return File(mediaFile, Helpers.Net.GetMIMEType(Path.GetFileName(mediaFile)));
                }
            }

            return null;
        }

        public ActionResult Media(string key, string t)
        {
            if (string.IsNullOrWhiteSpace(t) || string.IsNullOrWhiteSpace(key))
            {
                // - return nothing if token and/or key missing
                return null;
            }

            // - find download item
            foreach (var rm in Umbraco.TypedMediaAtRoot())
            {
                foreach (IMedia m in mediaservice.GetDescendants(rm.Id))
                {
                    if (m.HasProperty("downloadKey") && m.GetValue<string>("downloadKey").Equals(key, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        // - found the matching media
                        return MediaById(m.Id, t);
                    }
                }
            }

            // - nothing found
            return null;
        }
    }
}
