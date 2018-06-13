using System;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using FirebirdSql.Data.FirebirdClient;
using PayerAccount.Utils;
using PayerAccount.Dal.Remote.Data;
using System.Collections.Generic;

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
                string name = string.Empty;
                string address = string.Empty;
                decimal totalFloorSpace;
                int registratedCount;
                int roomCount;
                string zipCode= string.Empty;
                int rateVolume;
                decimal beginBalance;
                decimal endBalance;
                int defaultDeltaVolume;
                int dayDeltaVolume;
                int nightDeltaVolume;
                decimal defaultTariff;
                decimal dayTariff;
                decimal nightTariff;
                int defaultPublicspaceVolume;
                int dayPublicspaceVolume;
                int nightPublicspaceVolume;
                decimal defaultPublicspaceTariff;
                decimal dayPublicspaceTariff;
                decimal nightPublicspaceTariff;
                decimal estimateTotal;
                decimal estimatePublicspaceTotal;
                decimal adjustmentTotal;
                decimal paymentTotal;
                decimal groupTotalFloorSpace;
                decimal defaultEnergyTariff;
                decimal dayEnergyTariff;
                decimal nightEnergyTariff;
                decimal defaultTransferTariff;
                decimal dayTransferTariff;
                decimal nightTransferTariff;
                List<PayerCounterValue> payerCounterValues = new List<PayerCounterValue>();
                List<PayerPaymentExtracharge> payerPaymentExtracharges = new List<PayerPaymentExtracharge>();

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

                        name = reader.GetFieldFromReader<string>("customer_name");
                        address = reader.GetFieldFromReader<string>("customer_address");
                        totalFloorSpace = reader.GetFieldFromReader<decimal>("total_area");
                        registratedCount = reader.GetFieldFromReader<int>("registered_count");
                        roomCount = reader.GetFieldFromReader<int>("room_count");
                        zipCode = reader.GetFieldFromReader<string>("zip_code");
                        rateVolume = reader.GetFieldFromReader<int>("rate_volume");
                        beginBalance = reader.GetFieldFromReader<decimal>("begin_balance");
                        endBalance = reader.GetFieldFromReader<decimal>("end_balance");
                        defaultDeltaVolume = reader.GetFieldFromReader<int>("default_delta_kwh");
                        dayDeltaVolume = reader.GetFieldFromReader<int>("day_delta_kwh");
                        nightDeltaVolume = reader.GetFieldFromReader<int>("night_delta_kwh");
                        defaultTariff = reader.GetFieldFromReader<decimal>("default_tariff_value");
                        dayTariff = reader.GetFieldFromReader<decimal>("day_tariff_value");
                        nightTariff = reader.GetFieldFromReader<decimal>("night_tariff_value");
                        defaultPublicspaceVolume = reader.GetFieldFromReader<int>("publicspace_default_kwh");
                        dayPublicspaceVolume = reader.GetFieldFromReader<int>("publicspace_day_kwh");
                        nightPublicspaceVolume = reader.GetFieldFromReader<int>("publicspace_night_kwh");
                        defaultPublicspaceTariff = reader.GetFieldFromReader<decimal>("publicspace_default_tariff");
                        dayPublicspaceTariff = reader.GetFieldFromReader<decimal>("publicspace_day_tariff");
                        nightPublicspaceTariff = reader.GetFieldFromReader<decimal>("publicspace_night_tariff");
                        estimateTotal = reader.GetFieldFromReader<decimal>("estimate_value");
                        estimatePublicspaceTotal = reader.GetFieldFromReader<decimal>("publicspace_estimate");
                        adjustmentTotal = reader.GetFieldFromReader<decimal>("adjustment_value");
                        paymentTotal = reader.GetFieldFromReader<decimal>("PAYMENT_value");
                        groupTotalFloorSpace = reader.GetFieldFromReader<decimal>("GROUP_TOTAL_AREA");
                        defaultEnergyTariff = reader.GetFieldFromReader<decimal>("default_ENERGY_TARIFF");
                        dayEnergyTariff = reader.GetFieldFromReader<decimal>("DAY_ENERGY_TARIFF");
                        nightEnergyTariff = reader.GetFieldFromReader<decimal>("NIGHT_ENERGY_TARIFF");
                        defaultTransferTariff = reader.GetFieldFromReader<decimal>("default_TRANSFER_TARIFF");
                        dayTransferTariff = reader.GetFieldFromReader<decimal>("DAY_TRANSFER_TARIFF");
                        nightTransferTariff = reader.GetFieldFromReader<decimal>("NIGHT_TRANSFER_TARIFF");
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
                    $@"select first 10 countervalues.dayvalue dayValue, countervalues.nightvalue nightValue, countervalues.receiveddate receiveddate
                         from countervalues
                         join customercounter on
                           customercounter.id = countervalues.customercounterid
                           and customercounter.unmountdate is null
                           and customercounter.customer_id = {payerNumber}
                         order by countervalues.receiveddate desc"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payerCounterValues.Add(
                                new PayerCounterValue(
                                    reader.GetFieldFromReader<int>("dayValue"), 
                                    reader.GetFieldFromReader<int>("nightValue"), 
                                    reader.GetFieldFromReader<DateTime>("receiveddate")));
                        }
                    }
                    command.Transaction.Commit();
                }

                using (var command = GetDbCommandByQuery(
                     $@"select ENTRY_DATE,PERIOD_AS_STRING,DAY_VALUE,DAY_KWH,NIGHT_VALUE,NIGHT_KWH,ESTIMATE_VALUE, FACILITY_VALUE,PAYMENT_VALUE,BALANCE from get_operations_list({payerNumber}, '01.01.2018')"))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           payerPaymentExtracharges.Add(
                                new PayerPaymentExtracharge(
                                    reader.GetFieldFromReader<DateTime>("ENTRY_DATE"),
                                    reader.GetFieldFromReader<string>("PERIOD_AS_STRING"),
                                    reader.GetFieldFromReader<int>("DAY_VALUE"),
                                    reader.GetFieldFromReader<int>("DAY_KWH"),
                                    reader.GetFieldFromReader<int>("NIGHT_VALUE"),
                                    reader.GetFieldFromReader<int>("NIGHT_KWH"),
                                    reader.GetFieldFromReader<decimal>("ESTIMATE_VALUE"),
                                    reader.GetFieldFromReader<decimal>("FACILITY_VALUE"),
                                    reader.GetFieldFromReader<decimal>("PAYMENT_VALUE"),
                                    reader.GetFieldFromReader<decimal>("BALANCE")
                                    ));
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

                return new PayerState(
                    balance, dayValue, nightValue, counterName, counterCheckDate, counterMountDate,
                    name, address, totalFloorSpace, registratedCount, roomCount, zipCode, rateVolume,
                    beginBalance, endBalance, defaultDeltaVolume, dayDeltaVolume, nightDeltaVolume,
                    defaultTariff, dayTariff, nightTariff, defaultPublicspaceVolume, dayPublicspaceVolume,
                    nightPublicspaceVolume, defaultPublicspaceTariff, dayPublicspaceTariff, nightPublicspaceTariff,
                    estimateTotal, estimatePublicspaceTotal, adjustmentTotal, paymentTotal, groupTotalFloorSpace,
                    defaultEnergyTariff, dayEnergyTariff, nightEnergyTariff, defaultTransferTariff,
                    dayTransferTariff, nightTransferTariff, payerCounterValues, payerPaymentExtracharges);
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
