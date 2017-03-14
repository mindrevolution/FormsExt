using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace FormsExt.Nodes
{
    public static class Media
    {

        public static List<IMedia> GetDownloads(string key, string t)
        {
            List<IMedia> list = new List<IMedia>();

            if (!string.IsNullOrWhiteSpace(t) && !string.IsNullOrWhiteSpace(key))
            {
                UmbracoHelper umbracoHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
                IMediaService mediaService = ApplicationContext.Current.Services.MediaService;
                IMedia media = null;

                // - find download item
                foreach (var rm in umbracoHelper.TypedMediaAtRoot())
                {
                    foreach (IMedia m in mediaService.GetDescendants(rm.Id))
                    {
                        if (m.HasProperty("downloadKey") && m.GetValue<string>("downloadKey").Equals(key, System.StringComparison.InvariantCultureIgnoreCase))
                        {
                            // - found the matching media/folder
                            media = m;
                            var children = mediaService.GetDescendants(m.Id);
                            if (children.Count() > 0)
                            {
                                // - has children (= build list)
                                foreach (IMedia child in children)
                                {
                                    list.Add(child);
                                }
                            }
                            else
                            {
                                // - only add the single item
                                list.Add(m);
                            }
                            break;
                        }
                    }
                    if (media != null)
                        break;
                }
            }

            return list;
        }
    }
}
