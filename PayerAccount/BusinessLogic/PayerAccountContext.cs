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

namespace PayerAccount.BusinessLogic
{
    public class PayerAccountContext : IPayerAccountContext
    {
        private readonly PayerAccountDbContext localDb;
        private readonly IPasswordHasher<User> passwordHasher = new PasswordHasher<User>();

        public UserSessionState GetSessionState(HttpContext httpContext)
        {
            return httpContext.GetUserSessionState();
        }

        public PayerAccountContext(PayerAccountDbContext localDb)
        {
            this.localDb = localDb;
        }

        public MainViewModel GetCurrentMainViewModel(HttpContext httpContext)
        {
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
            var regions = localDb.Regions;

            return new LoginViewModel
            {
                PayerPassword = string.Empty,
                Regions = regions.Select(region => new SelectListItem { Value = region.Id.ToString(), Text = region.Name }),
                RegionId = regions.FirstOrDefault()?.Id ?? default(int)
            };
        }

        public RegistrateViewModel GetEmptyRegistrateModel()
        {
            var regions = localDb.Regions;

            return new RegistrateViewModel
            {
                UserPassword = string.Empty,
                UserConfirmPassword = string.Empty,
                Regions = regions.Select(region => new SelectListItem { Value = region.Id.ToString(), Text = region.Name }),
                UserRegionId = regions.FirstOrDefault()?.Id ?? default(int)
            };
        }

        public void Login(LoginViewModel loginModel, HttpContext httpContext)
        {
            if (GetSessionState(httpContext) != null)
                throw new Exception("User already login");

            var localUser = FindUser(loginModel.PayerNumber, loginModel.RegionId);
            if (localUser == null)
                throw new Exception("User is not registered");

            var passwordCheckResult = passwordHasher.VerifyHashedPassword(localUser, localUser.PasswordHash, loginModel.PayerPassword);
            if (passwordCheckResult != PasswordVerificationResult.Success)
                throw new Exception("Password is not valid");

            var department = localDb.Departments.FirstOrDefault(item => item.Id == localUser.DepartmentId);
            if (department == null)
                throw new Exception("User department is not found");

            var payerRepository = GetPayerRepository(department.Url, department.Path);
            var payerState = payerRepository.Get(localUser.Number);
            if (payerState == null)
                throw new Exception("Remote user is not exist");

            httpContext.SetUserSessionState(new UserSessionState(localUser, payerState));
        }

        public void Logout(HttpContext httpContext)
        {
            if (GetSessionState(httpContext) == null)
                throw new Exception("Already logout");

            httpContext?.SetUserSessionState(null);
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

            var departments = localDb.Departments.Where(department => department.RegionId == targetRegion.Id);
            
            PayerState targetPayerState = null;
            Department targetDepartment = null;
            foreach (var department in departments)
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
            var departments = localDb.Departments
                .Where(item => item.RegionId == regionId)
                .Select(item => item.Id);

            return localDb.Users.FirstOrDefault(user =>
                user.Number == userNumber &&
                departments.Contains(user.DepartmentId));
        }
    }
}
