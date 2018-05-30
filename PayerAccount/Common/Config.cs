using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

[assembly: InternalsVisibleTo("PayerAccount.Test")]
namespace PayerAccount.Common
{
    public static class Config
    {
        private static IConfiguration provider;
        public static IConfiguration Provider
        {
            get => provider;
            set
            {
                provider = value ?? throw new ArgumentNullException("Configuration provider");
            }
        }

        public static string RemoteDbUser => GetAppSettingByKey("remoteDbUser", string.Empty);
        public static string RemoteDbPassword => GetAppSettingByKey("remoteDbPassword", string.Empty);
        public static string PaymentReceiptTemplateFilename => GetAppSettingByKey("paymentReceiptTemplateFilename", string.Empty);

        private static string GetAppSettingByKey(string key, string defaultValue)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("Application settings key");

            return Provider?[key] ?? defaultValue;
        }

        private static int GetAppSettingByKeyAsInt(string key, int defaultValue)
        {
            var stringValue = GetAppSettingByKey(key, defaultValue.ToString());

            if (!int.TryParse(stringValue, out int result))
                result = defaultValue;

            return result;
        }

        private static bool GetAppSettingByKeyAsBool(string key, bool defaultValue)
        {
            var stringValue = GetAppSettingByKey(key, defaultValue.ToString());

            if (!bool.TryParse(stringValue, out bool result))
                result = defaultValue;

            return result;
        }
    }
}
