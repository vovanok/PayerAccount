using System;

namespace PayerAccount.Dal.Remote.Data
{
    public class PayerPaymentExtracharge
    {
        public DateTime EntryDate { get; private set; }
        public string PeriodAsString { get; private set; }
        public int DayValue { get; private set; }
        public int DayKwh { get; private set; }
        public int NightValue { get; private set; }
        public int NightKwh { get; private set; }
        public decimal EstimateValue { get; private set; }
        public decimal FacilityValue { get; private set; }
        public decimal PaymentValue { get; private set; }
        public decimal Balance { get; private set; }

        public PayerPaymentExtracharge(DateTime entryDate, string periodAsString, int dayValue, int dayKwh, int nightValue, int nightKwh, decimal estimateValue, decimal facilityValue, decimal paymentValue, decimal balance)
        {
            EntryDate = entryDate;
            PeriodAsString = periodAsString;
            DayValue = dayValue;
            DayKwh = dayKwh;
            NightValue = nightValue;
            NightKwh = nightKwh;
            EstimateValue = estimateValue;
            FacilityValue = facilityValue;
            PaymentValue = paymentValue;
            Balance = balance;
        }
    }
}
