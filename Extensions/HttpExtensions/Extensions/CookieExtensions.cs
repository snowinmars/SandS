using System;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Common.SandS
{
    public static class CookieExtensions
    {
            public static string GetCookie(this HttpContextBase currentHttpContext, string key)
            {
                // first see if cookie was changed - look in response
                if (currentHttpContext.Response.Cookies.AllKeys.Contains(key))
                {
                    var responseCookie = currentHttpContext.Response.Cookies.Get(key);
                    return responseCookie == null ? null : responseCookie.Value;
                }

                var cookie = currentHttpContext.Request.Cookies.Get(key);
                return cookie != null ? cookie.Value : null;
            }

            public static void SaveCookie(this HttpContextBase currentHttpContext, string key, string value)
            {
                currentHttpContext.SaveCookieWithExpiration(key, value, null, null);
            }

            public static void SaveCookie(this HttpContextBase currentHttpContext, string key, string value, DateTime expires)
            {
                currentHttpContext.SaveCookieWithExpiration(key, value, expires, null);
            }

            public static void SaveCookie(this HttpContextBase currentHttpContext, string key, string value, DateTime expires, bool httpOnly)
            {
                currentHttpContext.SaveCookieWithExpiration(key, value, expires, httpOnly);
            }

            private static void SaveCookieWithExpiration(this HttpContextBase currentHttpContext, string key, string value, DateTime? expires, bool? httpOnly)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                // if cookie didn't exist it will be created
                var cookie = currentHttpContext.Response.Cookies.Get(key);
                cookie.Value = value;
                if (expires.HasValue)
                {
                    cookie.Expires = expires.Value;
                }
                if (httpOnly.HasValue)
                {
                    cookie.HttpOnly = httpOnly.Value;
                }

                currentHttpContext.Response.Cookies.Add(cookie);
            }

            public static void RemoveCookie(this HttpContextBase currentHttpContext, string key)
            {
                var cookie = currentHttpContext.Request.Cookies.Get(key);
                if (cookie != null)
                {
                    var expiredCookie = new HttpCookie(key);
                    expiredCookie.Expires = DateTime.Now.AddDays(-1);
                    currentHttpContext.Response.Cookies.Add(expiredCookie);
                }
            }

            public static string[] GetArrayCookie(this HttpContextBase currentHttpContext, string key)
            {
                var cookie = currentHttpContext.Request.Cookies.Get(key);
                return cookie != null ? cookie.Value.TrimEnd('/').Split('/') : null;
            }
        }
}