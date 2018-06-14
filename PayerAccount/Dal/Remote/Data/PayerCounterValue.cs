using System;

namespace PayerAccount.Dal.Remote.Data
{
    public class PayerCounterValue
    {
        public int DayValue { get; private set; }
        public int NightValue { get; private set; }
        public DateTime RecieveDate { get; private set; }

        public PayerCounterValue(int dayValue, int nightValue, DateTime recieveDate)
        {
            DayValue = dayValue;
            NightValue = nightValue;
            RecieveDate = recieveDate;
        }
    }
}
