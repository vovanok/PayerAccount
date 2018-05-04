using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PayerAccount.Models;
using PayerAccount.BusinessLogic;

namespace PayerAccount.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPayerAccountContext context;

        public IActionResult Index()
        {
            if (context.IsLogin)
                return Main();

            return Login();
        }

        [AcceptVerbs("get")]
        public IActionResult Login()
        {
            return View();
        }

        [AcceptVerbs("post")]
        public IActionResult Login(string payerNumber, int regionId, string password)
        {
            return View();
        }

        public IActionResult Main()
        {
            return View();
        }

        //
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
