using EasyTagProject.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace EasyTagProject.Infrastructure
{
    public static class UserAccountExtensions
    {
        /// <summary>
        /// Determines if the id beloongs to the current logged user or an admin
        /// </summary>
        /// <param name="viewContext">Extended object</param>
        /// <param name="userId">userId to compare with the logged user</param>
        /// <returns></returns>
        public static bool IsAccessibleForUserOrAdmin(this ViewContext viewContext, string userId)
        {
            if (viewContext.HttpContext.User == null)
            {
                return false;
            }
            if (viewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return false;
            }
            if (String.IsNullOrEmpty(userId))
            {
                return false;
            }

            return viewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value == userId ||
                viewContext.HttpContext.User.IsInRole(nameof(UserRoles.Admin));
        }

        public static bool IsAccessibleForUserOrAdmin(this HttpContext httpContext, string userId)
        {
            if (httpContext.User == null)
            {
                return false;
            }
            if (httpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return false;
            }
            if (String.IsNullOrEmpty(userId))
            {
                return false;
            }

            return httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value == userId ||
                httpContext.User.IsInRole(nameof(UserRoles.Admin));
        }

        /// <summary>
        /// Identify if the id belongs to the current logged user
        /// </summary>
        /// <param name="viewContext"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsAccessibleForUser(this ViewContext viewContext, string userId)
        {
            if (viewContext.HttpContext.User == null)
            {
                return false;
            }
            if (viewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return false;
            }

            return viewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value == userId;
        }

        /// <summary>
        /// Determines if the user belongs to the specified role
        /// </summary>
        /// <param name="viewContext">Extended object</param>
        /// <param name="roleName">Name of the specified role</param>
        /// <returns></returns>
        public static bool IsUserInRool(this ViewContext viewContext, string roleName) =>
            viewContext.HttpContext.User.IsInRole(roleName);

        /// <summary>
        /// Gets the Id of the logged user. Returns an empty string if there is no logged user
        /// </summary>
        /// <param name="viewContext">Extended object</param>
        /// <returns></returns>
        public static string GetLoggedUserId(this ViewContext viewContext)
        {
            if (viewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return "";
            }

            return viewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // 
        public static string GetLoggedUserId(this HttpContext httpContext)
        {
            if (httpContext.User.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return "";
            }

            return httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewContext"></param>
        /// <returns></returns>
        public static string GetLoggedUserName(this ViewContext viewContext)
        {
            if (viewContext.HttpContext.User == null)
            {
                return "";
            }

            return viewContext.HttpContext.User.Identity.Name;
        }

        //
        public static string GetLoggedUserName(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return "";
            }

            return httpContext.User.Identity.Name;
        }

        public static string GetHostUrl(this ViewContext viewContext)
        {
            return HttpUtility.UrlEncode(viewContext.HttpContext.Request.Host.ToString());
        }
    }
}
