using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Umbraco.Core.Macros;

namespace FormsExt.Xslt
{
    [XsltExtension]
    public class Library
    { 
        public static string GenerateTrackingToken(string email)
        {
            return Helpers.Tokens.GenerateTrackingToken(email);
        }
    }
}
