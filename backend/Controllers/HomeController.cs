using Microsoft.AspNetCore.Mvc;

namespace ScholarshipCMGroups.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
