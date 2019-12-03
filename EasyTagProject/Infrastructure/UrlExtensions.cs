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

            if (url.Contains("Login"))
            {
                url = "/";
            }

            return url;
        }
    }
}
