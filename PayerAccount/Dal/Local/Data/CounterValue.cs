using System;

namespace PayerAccount.Dal.Local.Data
{
    public class CounterValues
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int DayValue { get; set; }
        public int NightValue { get; set; }
    }
}
