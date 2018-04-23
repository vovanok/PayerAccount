using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using PayerAccount.Common;
using PayerAccount.Dal.Remote;

namespace PayerAccount.Test
{
    [TestClass]
    public class PayerRepositoryTest
    {
        private const string TEST_FB_DB_URL = "localhost";
        private const string TEST_FB_DB_PATH = @"c:\TestData\testData.FDB";

        [TestInitialize]
        public void Initialize()
        {
            Config.Provider = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("dc7c1e72-414d-4264-8987-ac34edfca3c8")
                .Build();
        }

        [TestMethod]
        public void PayerRepositoryNullInit()
        {
            PayerRepository payerRepository = null;

            try
            {
                payerRepository = new PayerRepository(null, null, null, null);
            }
            catch(Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException, "Exception was not thrown after pass incorrect arguments");
            }

            Assert.IsNull(payerRepository, "Payer repository is't NULL after exception");
        }

        [TestMethod]
        public void PayerRepositoryIncorrectLocation()
        {
            PayerRepository payerRepository = null;

            try
            {
                payerRepository = new PayerRepository(string.Empty, string.Empty, Config.RemoteDbUser, Config.RemoteDbPassword);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException, "Exception was not thrown after pass incorrect arguments");
            }

            Assert.IsNull(payerRepository, "Payer repository is't NULL after exception");
        }

        [TestMethod]
        public void PayerRepositoryIncorrectCredentials()
        {
            var payerRepository = new PayerRepository(TEST_FB_DB_URL, TEST_FB_DB_PATH, string.Empty, string.Empty);

            try
            {
                var payer = payerRepository.Get(123);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is FbException, "Get payer from unautorise repository was not thrown exception");
            }
        }

        [TestMethod]
        public void PayerRepositoryGetCorrectPayer()
        {
            var payerRepository = new PayerRepository(TEST_FB_DB_URL, TEST_FB_DB_PATH, Config.RemoteDbUser, Config.RemoteDbPassword);
            var correctPayer = payerRepository.Get(21101);

            Assert.IsNotNull(correctPayer, "Player was not received");

            int expectedAddressLength = 32;
            decimal expectedBalance = 0;
            double expecedDayValue = 10113;
            double expectedNightValue = 0;
            string expectedCounterName = "жн-6807Я (5)";
            DateTime expectedCounterCheckDate = new DateTime(2008, 3, 1);
            DateTime expectedCounterMountDate = new DateTime(2009, 4, 7);

            Assert.IsTrue(correctPayer.Address.Length == expectedAddressLength,
                $"Address is incorrect: length actual {correctPayer.Address.Length}; lenght expected {expectedAddressLength}");
            Assert.IsTrue(correctPayer.Balance == expectedBalance,
                $"Balance is incorrect: actual {correctPayer.Balance}; expected {expectedBalance}");
            Assert.IsTrue(correctPayer.DayValue == expecedDayValue,
                $"Day value is incorrect: actual {correctPayer.DayValue}; expected {expecedDayValue}");
            Assert.IsTrue(correctPayer.NightValue == expectedNightValue,
                $"Night value is incorrect: actual {correctPayer.NightValue}; expected {expectedNightValue}");
            Assert.IsTrue(correctPayer.CounterName == expectedCounterName,
                $"Counter name is incorrect: actual '{correctPayer.CounterName}'; expected '{expectedCounterName}'");
            Assert.IsTrue(correctPayer.CounterCheckDate == expectedCounterCheckDate,
                $"Counter check date is incorrect: actual '{correctPayer.CounterCheckDate.ToString("dd.MM.yyyy")}'; expected '{expectedCounterCheckDate.ToString("dd.MM.yyyy")}'");
            Assert.IsTrue(correctPayer.CounterMountDate == expectedCounterMountDate,
                $"Counter mount date is incorrect: actual '{correctPayer.CounterMountDate.ToString("dd.MM.yyyy")}'; expected '{expectedCounterMountDate.ToString("dd.MM.yyyy")}'");
        }
    }
}
