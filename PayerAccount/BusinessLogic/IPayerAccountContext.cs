using Microsoft.AspNetCore.Http;
using PayerAccount.Models;

namespace PayerAccount.BusinessLogic
{
    public interface IPayerAccountContext
    {
        bool IsLogin { get; }
        LoginViewModel GetEmptyLoginModel();
        RegistrateViewModel GetEmptyRegistrateModel();
        void Login(LoginViewModel loginModel, HttpContext httpContext);
        void Registrate(RegistrateViewModel registrateModel);
        MainViewModel GetCurrentMainViewModel(HttpContext httpContext);
    }
}
