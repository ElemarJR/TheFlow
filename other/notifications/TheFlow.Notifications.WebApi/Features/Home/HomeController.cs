using Microsoft.AspNetCore.Mvc;

namespace TheFlow.Notifications.WebApi.Features.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}
