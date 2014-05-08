using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Mimry.Attributes
{
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        private string m_RedirectUrl;

        public AjaxAuthorizeAttribute(string redirectUrl)
        {
            m_RedirectUrl = redirectUrl;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = Convert.ToInt32(HttpStatusCode.Unauthorized);
                filterContext.HttpContext.Response.Write(m_RedirectUrl);
                filterContext.HttpContext.Response.End();
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}