using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace PayerAccount.Models
{
    public class CounterValueViewModel
    {
        public int day_value { get; set; }
        public int night_value { get; set; }
        public DateTime date { get; set; }
    }
}
