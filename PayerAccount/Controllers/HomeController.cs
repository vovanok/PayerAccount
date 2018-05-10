using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PayerAccount.Models;
using PayerAccount.BusinessLogic;
using System;

namespace PayerAccount.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPayerAccountContext context;

        public HomeController(IPayerAccountContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            if (context.IsLogin)
                return Main();

            return Login();
        }

        [AcceptVerbs("get")]
        public IActionResult Login()
        {
            return View(context.GetEmptyLoginModel());
        }

        [AcceptVerbs("post")]
        public IActionResult Login(LoginViewModel loginModel)
        {
            try
            {
                context.Login(loginModel, HttpContext);
                return Main();
            }
            catch (Exception ex)
            {
                return Message($"Login failed: {ex.Message}");
            }
        }

        [AcceptVerbs("get")]
        public IActionResult Registrate()
        {
            return View(context.GetEmptyRegistrateModel());
        }

        [AcceptVerbs("post")]
        public IActionResult Registrate(RegistrateViewModel registrateModel)
        {
            try
            {
                context.Registrate(registrateModel);
                return Message($"Registration success");
            }
            catch (Exception ex)
            {
                return Message($"Registration failed: {ex.Message}");
            }
        }

        public IActionResult Main()
        {
            if (!context.IsLogin)
                return Login();

            var mainModel = context.GetCurrentMainViewModel(HttpContext);
            if (mainModel == null)
                return Login();

            return View(mainModel);
        }

        public IActionResult Message(string message)
        {
            return View(message ?? string.Empty);
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
