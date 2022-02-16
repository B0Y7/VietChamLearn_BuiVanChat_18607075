using System.Web;
using System.Web.Mvc;

namespace VietChamLearn_BuiVanChat_18607075
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
