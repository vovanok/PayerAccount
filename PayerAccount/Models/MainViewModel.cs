using System;

namespace PayerAccount.Models
{
    public class MainViewModel
    {
        public string UserName { get; set; }
        public int UserNumber { get; set; }
        public decimal UserBalance { get; set; }
        public int UserDayCounterValue { get; set; }
        public int UserNightCounterValue { get; set; }
        public string UserCounterName { get; set; }
        public DateTime UserCounterMountDate { get; set; }
        public DateTime UserCounterCheckDate { get; set; }
    }
}
