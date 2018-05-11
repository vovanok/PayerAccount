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
            if (context.GetSessionState(HttpContext) != null)
                return RedirectToAction("Main");

            return RedirectToAction("Login");
        }

        [AcceptVerbs("get")]
        public IActionResult Login()
        {
            var loginModel = context.GetEmptyLoginModel();
            return View(loginModel);
        }

        [AcceptVerbs("post")]
        public IActionResult Login(LoginViewModel loginModel)
        {
            try
            {
                context.Login(loginModel, HttpContext);
                return RedirectToAction("Main");
            }
            catch (Exception ex)
            {
                return OpenMessagePage(
                    new MessageViewModel { Message = $"Login failed: {ex.Message}", IsError = true });
            }
        }

        public IActionResult Logout()
        {
            try
            {
                context.Logout(HttpContext);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return OpenMessagePage(
                    new MessageViewModel { Message = $"Logout failed: {ex.Message}", IsError = true });
            }
        }

        [AcceptVerbs("get")]
        public IActionResult Registrate()
        {
            var registrateModel = context.GetEmptyRegistrateModel();
            return View(registrateModel);
        }

        [AcceptVerbs("post")]
        public IActionResult Registrate(RegistrateViewModel registrateModel)
        {
            try
            {
                context.Registrate(registrateModel);
                return OpenMessagePage(new MessageViewModel { Message = $"Registration success" });
            }
            catch (Exception ex)
            {
                return OpenMessagePage(new MessageViewModel { Message = $"Registration failed: {ex.Message}", IsError = true });
            }
        }

        public IActionResult Main()
        {
            var mainModel = context.GetCurrentMainViewModel(HttpContext);
            if (mainModel == null)
                return RedirectToAction("Login");

            return View(mainModel);
        }

        public IActionResult Message(MessageViewModel messageModel)
        {
            return View(messageModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult OpenMessagePage(MessageViewModel messageModel)
        {
            return RedirectToAction("Message", "Home", messageModel);
        }
    }
}
