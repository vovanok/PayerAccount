using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using PayerAccount.Utils;
using PayerAccount.Common;
using PayerAccount.Models;
using PayerAccount.Dal.Remote.Data;
using PayerAccount.Dal.Remote;
using PayerAccount.Dal.Local;
using PayerAccount.Dal.Local.Data;

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
                UserAddress = userSessionState.PayerState.Address,
                UserNumber = userSessionState.User.Number,
                UserBalance = userSessionState.PayerState.Balance,
                UserDayCounterValue = userSessionState.PayerState.DayValue,
                UserNightCounterValue = userSessionState.PayerState.NightValue,
                UserCounterName = userSessionState.PayerState.CounterName,
                UserCounterMountDate = userSessionState.PayerState.CounterMountDate,
                UserCounterCheckDate = userSessionState.PayerState.CounterCheckDate,
                UserPayerCounterValues = userSessionState.PayerState.PayerCounterValues,
                UserPayerPaymentExtracharges = userSessionState.PayerState.PayerPaymentExtracharges
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

        public string GetPaymentReceiptPath(HttpContext httpContext)
        {
            var sessionState = GetSessionState(httpContext);
            if (sessionState == null)
                return null;

            var templateFullpath = Path.Combine(Environment.CurrentDirectory, Config.PaymentReceiptTemplateFilename);
            if (!File.Exists(templateFullpath))
                throw new Exception($"Template is not exist.");

            var receiptDocumentWorker = new ReceiptDocumentWorker(templateFullpath);
            receiptDocumentWorker.PutToPlaceholder(11, sessionState.PayerState.ZipCode.ToString());
            receiptDocumentWorker.PutToPlaceholder(1, sessionState.User.Number.ToString());
            receiptDocumentWorker.PutToPlaceholder(2, "???");
            receiptDocumentWorker.PutToPlaceholder(4, sessionState.User.Name);
            receiptDocumentWorker.PutToPlaceholder(5, sessionState.PayerState.Address);
            receiptDocumentWorker.PutToPlaceholder(8, sessionState.PayerState.TotalFloorSpace.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(9, sessionState.PayerState.RegistratedCount.ToString());
            receiptDocumentWorker.PutToPlaceholder(10, sessionState.PayerState.RoomCount.ToString());
            receiptDocumentWorker.PutToPlaceholder(32, sessionState.PayerState.EndBalance.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(13, sessionState.PayerState.BeginBalance.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(14, sessionState.PayerState.DefaultDeltaVolume.ToString());
            receiptDocumentWorker.PutToPlaceholder(16, sessionState.PayerState.DayDeltaVolume.ToString());
            receiptDocumentWorker.PutToPlaceholder(18, sessionState.PayerState.NightDeltaVolume.ToString());
            receiptDocumentWorker.PutToPlaceholder(15, sessionState.PayerState.DefaultTariff.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(17, sessionState.PayerState.DayTariff.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(19, sessionState.PayerState.NightTariff.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(20, sessionState.PayerState.DefaultPublicspaceVolume.ToString());
            receiptDocumentWorker.PutToPlaceholder(22, sessionState.PayerState.DayPublicspaceVolume.ToString());
            receiptDocumentWorker.PutToPlaceholder(24, sessionState.PayerState.NightPublicspaceVolume.ToString());
            receiptDocumentWorker.PutToPlaceholder(21, sessionState.PayerState.DefaultPublicspaceTariff.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(23, sessionState.PayerState.DayPublicspaceTariff.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(25, sessionState.PayerState.NightPublicspaceTariff.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(27, sessionState.PayerState.EstimateTotal.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(28, sessionState.PayerState.EstimatePublicspaceTotal.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(30, sessionState.PayerState.AdjustmentTotal.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(31, sessionState.PayerState.PaymentTotal.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(46, sessionState.PayerState.GroupTotalFloorSpace.ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(12, sessionState.PayerState.RateVolume.ToString());
            receiptDocumentWorker.PutToPlaceholder(37, (sessionState.PayerState.DayEnergyTariff / sessionState.PayerState.NightEnergyTariff).ToString("0.00"));
            receiptDocumentWorker.PutToPlaceholder(38, (sessionState.PayerState.DayTransferTariff / sessionState.PayerState.NightTransferTariff).ToString("0.00"));

            var receiptFolder = Path.Combine("PaymentReceipts", sessionState.User.Id.ToString());
            if (!Directory.Exists(receiptFolder))
                Directory.CreateDirectory(receiptFolder);

            var filenameForSave = Path.Combine(receiptFolder, Guid.NewGuid().ToString() + ".htm");
            if (File.Exists(filenameForSave))
                File.Delete(filenameForSave);

            receiptDocumentWorker.Save(filenameForSave);

            return filenameForSave;
        }

        public void SaveCounterValues(int dayValue, int nightValue, HttpContext httpContext)
        {
            var sessionState = GetSessionState(httpContext);
            if (sessionState == null)
                throw new Exception("User isn't login");

            var department = localDb.Departments.FirstOrDefault(item => item.Id == sessionState.User.DepartmentId);
            if (department == null)
                throw new Exception("User department is not found");

            var payerRepository = GetPayerRepository(department.Url, department.Path);
            if (sessionState.PayerState.CustomerCounterId ==0)
            {
                throw new Exception("User has no counter");
            }

            var isCounterValueInserted = false;

            /*if (payerRepository.IsCounterValueValid(sessionState.User.Number, dayValue,nightValue))
            {
                throw new Exception("На этот день уже есть контрольные показания.");
            }*/

            if ((dayValue < sessionState.PayerState.DayValue) || (nightValue < sessionState.PayerState.NightValue))
            {
                throw new Exception("Новые показания не могут быть меньше предыдущих");
            }


            isCounterValueInserted = payerRepository.InsertCounterValue(sessionState.User.Number, dayValue, nightValue, sessionState.PayerState.CustomerCounterId);
            
            if (!isCounterValueInserted)
                throw new Exception("Возникли технические проблемы. Попробуйте указать контрольные показания позже.");


            


            // TODO: сдеалть валидацию котнрольных показаний

            localDb.CounterValues.Add(new CounterValues
            {
                UserId = sessionState.User.Id,
                Date = DateTime.Now,
                DayValue = dayValue,
                NightValue = nightValue
            });

            localDb.SaveChanges();
           

        }
    }
}
