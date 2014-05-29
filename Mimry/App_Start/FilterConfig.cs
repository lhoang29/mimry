using System.Web;
using System.Web.Mvc;
using Mimry.Filters;

namespace Mimry
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ElmahRequestValidationErrorFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
