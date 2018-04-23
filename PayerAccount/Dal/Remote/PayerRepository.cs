using System;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using FirebirdSql.Data.FirebirdClient;
using PayerAccount.Utils;
using PayerAccount.Models.Remote;

[assembly: InternalsVisibleTo("PayerAccount.Test")]
namespace PayerAccount.Dal.Remote
{
    internal class PayerRepository
    {
        private const string CONNECTION_STRING_FORMAT = "DataSource={0};Database={1};User={2};Password={3};Charset=UTF8";
        private DbConnection connection;
        
        public PayerRepository(string url, string path, string user, string password)
        {
            connection = new FbConnection(string.Format(CONNECTION_STRING_FORMAT, url, path, user, password));
        }

        public PayerState Get(int payerNumber)
        {
            try
            {
                connection.Open();
                var currentDate = DateTime.Now;

                // Check account open
                using (var command = GetDbCommandByQuery(
                    $"select id from customer where customer.id = {payerNumber} and customer.AGGR$STATE_ID <> 4"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;
                    }

                    command.Transaction.Commit();
                }

                // Common payer information
                string address = string.Empty;
                using (var command = GetDbCommandByQuery(
                    $"select * from r090$print({payerNumber}, {currentDate.GetFinancialPeriod()});"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        var payerState = reader.GetFieldFromReader<int>("customer_state_id");
                        if (payerState == 4) // Closed status
                            return null;

                        address = reader.GetFieldFromReader<string>("customer_address");
                        
                        // TODO: get other payer information
                    }

                    command.Transaction.Commit();
                }

                // Payer balance
                decimal balance = 0;
                using (var command = GetDbCommandByQuery(
                    $"select balance from get_customer_balance({payerNumber}, '{currentDate.ToString("dd.MM.yyyy")}');"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            balance = reader.GetFieldFromReader<decimal>("balance");
                    }

                    command.Transaction.Commit();
                }

                // Day and night payer's counter values
                int dayValue = 0;
                int nightValue = 0;
                using (var command = GetDbCommandByQuery(
                    $@"select first 1 countervalues.dayvalue dayValue, countervalues.nightvalue nightValue
                         from countervalues
                         join customercounter on
                           customercounter.id = countervalues.customercounterid
                           and customercounter.unmountdate is null
                           and customercounter.customer_id = {payerNumber}
                         order by countervalues.receiveddate desc"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dayValue = reader.GetFieldFromReader<int>("dayValue");
                            nightValue = reader.GetFieldFromReader<int>("nightValue");
                        }
                    }

                    command.Transaction.Commit();
                }

                // First day of previous month
                var prevMonth = currentDate.AddMonths(-1);
                var prevMonthFirstDay = new DateTime(prevMonth.Year, prevMonth.Month, 1);

                // Counter name
                string counterName = string.Empty;
                using (var command = GetDbCommandByQuery(
                    $@"select COUNTER_NAME from R090$GET_COUNTER_VALUES(
                         {payerNumber}, '{prevMonthFirstDay.ToString("dd.MM.yyyy")}', '{currentDate.ToString("dd.MM.yyyy")}');"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            counterName = reader.GetFieldFromReader<string>("COUNTER_NAME");
                    }

                    command.Transaction.Commit();
                }

                // Counter check and mount dates
                DateTime counterCheckDate = default(DateTime);
                DateTime counterMountDate = default(DateTime);
                using (var command = GetDbCommandByQuery(
                    $@"select checkDate, mountDate, id from customercounter
                         where customer_id = {payerNumber} and customercounter.unmountdate is null"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            counterCheckDate = reader.GetFieldFromReader<DateTime>("checkDate");
                            counterMountDate = reader.GetFieldFromReader<DateTime>("mountDate");
                        }
                    }

                    command.Transaction.Commit();
                }

                return new PayerState(address, balance, dayValue, nightValue, counterName, counterCheckDate, counterMountDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection?.Close();
            }
        }

        private FbCommand GetDbCommandByQuery(string query)
        {
            if (connection.State != ConnectionState.Open)
                throw new SystemException("DB connection is't open");

            var command = connection.CreateCommand();
            command.Transaction = connection.BeginTransaction();
            command.CommandText = query;
            return command as FbCommand;
        }
    }
}
