using System;
using System.Data.Common;

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
    }
}