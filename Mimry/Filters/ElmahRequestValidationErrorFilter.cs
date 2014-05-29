using Elmah;
using System.Web;
using System.Web.Mvc;

namespace Mimry.Filters
{
    public class ElmahRequestValidationErrorFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Elmah currently doesn't log this type of exception due to a breaking change in request validation in ASP.NET 4
            // It appears it still isn't fixed as of now https://code.google.com/p/elmah/issues/detail?id=217 so it needs to be done
            // manually via global filters
            if (context.Exception is HttpRequestValidationException)
            {
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Error(context.Exception));
            }
        }
    }
}