using System;
using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PayerAccount.BusinessLogic;

namespace PayerAccount.Utils
{
    public static class Extensions
    {
        public static T GetFieldFromReader<T>(this DbDataReader dataReader, string columnName, T defaultValue = default(T))
        {
            int ordinal = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(ordinal))
                return defaultValue;

            return dataReader.GetFieldValue<T>(ordinal);
        }

        public static int GetFinancialPeriod(this DateTime dateTime)
        {
            return dateTime.Year * 12 + dateTime.Month;
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null
                ? default(T)
                : JsonConvert.DeserializeObject<T>(value);
        }

        public static UserSessionState GetUserSessionState(this HttpContext httpContext)
        {
            return httpContext.Session.Get<UserSessionState>("user");
        }

        public static void SetUserSessionState(this HttpContext httpContext, UserSessionState sessionState)
        {
            httpContext.Session.Set<UserSessionState>("user", sessionState);
        }
    }
}