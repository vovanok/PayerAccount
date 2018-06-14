using System;
using System.Collections.Generic;
using PayerAccount.Dal.Remote.Data;

namespace PayerAccount.Models
{
    public class MainViewModel
    {
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public int UserNumber { get; set; }
        public decimal UserBalance { get; set; }
        public int UserDayCounterValue { get; set; }
        public int UserNightCounterValue { get; set; }
        public string UserCounterName { get; set; }
        public DateTime UserCounterMountDate { get; set; }
        public DateTime UserCounterCheckDate { get; set; }
        public List<PayerCounterValue> UserPayerCounterValues { get; set; } 
        public List<PayerPaymentExtracharge> UserPayerPaymentExtracharges { get; set; }
    }
}
