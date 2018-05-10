using System;

namespace PayerAccount.Models.Remote
{
    public class PayerState
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public decimal Balance { get; private set; }
        public int DayValue { get; private set; }
        public int NightValue { get; private set; }
        public string CounterName { get; private set; }
        public DateTime CounterCheckDate { get; private set; }
        public DateTime CounterMountDate { get; private set; }

        public PayerState(
            string name, string address, decimal balance,
            int dayValue, int nightValue, string counterName,
            DateTime counterCheckDate, DateTime counterMountDate)
        {
            Name = name;
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
