using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using PayerAccount.Utils;
using PayerAccount.Common;
using PayerAccount.Models;
using PayerAccount.Models.Remote;
using PayerAccount.Dal.Remote;
using PayerAccount.Dal.Local;
using PayerAccount.Dal.Local.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PayerAccount.BusinessLogic
{
    public class PayerAccountContext : IPayerAccountContext
    {
        private readonly PayerAccountDbContext localDb = new PayerAccountDbContext();
        private readonly IPasswordHasher<User> passwordHasher = new PasswordHasher<User>();

        public bool IsLogin { get; private set; }

        public MainViewModel GetCurrentMainViewModel(HttpContext httpContext)
        {
            if (!IsLogin)
                return null;

            var userSessionState = httpContext.GetUserSessionState();
            if (userSessionState == null)
                return null;

            return new MainViewModel
            {
                UserName = userSessionState.User.Name,
                UserNumber = userSessionState.User.Number,
                UserBalance = userSessionState.PayerState.Balance,
                UserDayCounterValue = userSessionState.PayerState.DayValue,
                UserNightCounterValue = userSessionState.PayerState.NightValue,
                UserCounterName = userSessionState.PayerState.CounterName,
                UserCounterMountDate = userSessionState.PayerState.CounterMountDate,
                UserCounterCheckDate = userSessionState.PayerState.CounterCheckDate
            };
        }

        public LoginViewModel GetEmptyLoginModel()
        {
            return new LoginViewModel
            {
                Regions = GetRegionsSelectItems()
            };
        }

        public RegistrateViewModel GetEmptyRegistrateModel()
        {
            return new RegistrateViewModel
            {
                Regions = GetRegionsSelectItems()
            };
        }

        public void Login(LoginViewModel loginModel, HttpContext httpContext)
        {
            if (IsLogin)
                throw new Exception("User already login");

            var localUser = localDb.Users.FirstOrDefault(
                user =>
                    user.Number == loginModel.PayerNumber &&
                    user.Department.RegionId == loginModel.RegionId);

            if (localUser == null)
                throw new Exception("User is not registered");

            var passwordCheckResult = passwordHasher.VerifyHashedPassword(localUser, localUser.PasswordHash, loginModel.PayerPassword);
            if (passwordCheckResult != PasswordVerificationResult.Success)
                throw new Exception("Password is not valid");

            var payerRepository = GetPayerRepository(localUser.Department.Url, localUser.Department.Path);
            var payerState = payerRepository.Get(localUser.Number);
            if (payerState == null)
                throw new Exception("Remote user is not exist");

            httpContext.SetUserSessionState(new UserSessionState(localUser, payerState));
        }

        public void Registrate(RegistrateViewModel registrateModel)
        {
            if (registrateModel.UserNumber <= 0)
                throw new ArgumentException("Payer number");

            if (string.IsNullOrEmpty(registrateModel.UserPassword)
                || string.IsNullOrEmpty(registrateModel.UserConfirmPassword))
                throw new ArgumentException("Password or confirm password");

            if (registrateModel.UserPassword != registrateModel.UserConfirmPassword)
                throw new ArgumentException("Passwords are not same");

            var targetRegion = localDb.Regions.FirstOrDefault(region => region.Id == registrateModel.UserRegionId);
            if (targetRegion == null)
                throw new Exception("Region not found");

            var existUser = FindUser(registrateModel.UserNumber, registrateModel.UserRegionId);
            if (existUser != null)
                throw new Exception("User already exist");

            PayerState targetPayerState = null;
            Department targetDepartment = null;
            foreach (var department in targetRegion.Departments)
            {
                var payerRepository = GetPayerRepository(department.Url, department.Path);
                targetPayerState = payerRepository.Get(registrateModel.UserNumber);
                if (targetPayerState != null)
                {
                    targetDepartment = department;
                    break;
                }
            }

            if (targetPayerState == null || targetDepartment == null)
                throw new Exception($"Payer {registrateModel.UserNumber} in region {targetRegion.Name} is not found");

            var newUser = new User
            {
                Number = registrateModel.UserNumber,
                Name = targetPayerState.Name,
                DepartmentId = targetDepartment.Id
            };

            newUser.PasswordHash = passwordHasher.HashPassword(newUser, registrateModel.UserPassword);

            localDb.Users.Add(newUser);
            localDb.SaveChanges();
        }

        private PayerRepository GetPayerRepository(string dbUrl, string dbPath)
        {
            return new PayerRepository(dbUrl, dbPath, Config.RemoteDbUser, Config.RemoteDbPassword);
        }

        private User FindUser(int userNumber, int regionId)
        {
            return localDb.Users.FirstOrDefault(user =>
                user.Number == userNumber &&
                user.Department.RegionId == regionId);
        }

        private IEnumerable<SelectListItem> GetRegionsSelectItems()
        {
            return localDb.Regions.Select(
                region => new SelectListItem { Value = region.Id.ToString(), Text = region.Name });
        }
    }
}
