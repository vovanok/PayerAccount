using Microsoft.AspNetCore.Http;
using PayerAccount.Models;

namespace PayerAccount.BusinessLogic
{
    public interface IPayerAccountContext
    {
        UserSessionState GetSessionState(HttpContext httpContext);
        LoginViewModel GetEmptyLoginModel();
        RegistrateViewModel GetEmptyRegistrateModel();
        void Login(LoginViewModel loginModel, HttpContext httpContext);
        void Logout(HttpContext httpContext);
        void Registrate(RegistrateViewModel registrateModel);
        MainViewModel GetCurrentMainViewModel(HttpContext httpContext);
    }
}
