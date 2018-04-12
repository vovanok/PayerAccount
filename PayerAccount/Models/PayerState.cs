using System;

namespace PayerAccount.Models
{
    internal class PayerState
    {
        public string Address { get; private set; }
        public decimal Balance { get; private set; }
        public int DayValue { get; private set; }
        public int NightValue { get; private set; }
        public string CounterName { get; private set; }
        public DateTime CounterCheckDate { get; private set; }
        public DateTime CounterMountDate { get; private set; }

        public PayerState(
            string address, decimal balance, int dayValue,
            int nightValue, string counterName,
            DateTime counterCheckDate, DateTime counterMountDate)
        {
            Address = address;
            Balance = balance;
            DayValue = dayValue;
            NightValue = nightValue;
            CounterName = counterName;
            CounterCheckDate = counterCheckDate;
            CounterMountDate = counterMountDate;
        }
    }
}
