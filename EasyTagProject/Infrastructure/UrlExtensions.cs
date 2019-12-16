using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EasyTagProject.Infrastructure
{
    public static class UrlExtensions
    {
        /// <summary>
        /// Returns the actual path with query string as string
        /// </summary>
        /// <param name="request">Extended object</param>
        /// <returns></returns>
        public static string PathAndQuery(this HttpRequest request)
        {
            string url = request.QueryString.HasValue
                            ? $"{request.Path}{request.QueryString}"
                            : request.Path.ToString();

            // avoid self redirection
            if (url.Contains("Login") || url.Contains("ChangePassword"))
            {
                url = "/";
            }
            // Encode all characters
            else if (url.Contains("SearchList"))
            {
                url = HttpUtility.UrlEncode(url);
            }
            // Replace / to + to avoid redirection issues
            else if (url[0].Equals('/') && url[1..].Contains("/"))
            {
                url = url.Replace('/', '+');
            }

            return url;
        }
    }
}
