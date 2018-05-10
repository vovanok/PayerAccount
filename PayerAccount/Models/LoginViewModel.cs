using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace PayerAccount.Models
{
    public class LoginViewModel
    {
        public int PayerNumber { get; set; }
        public int RegionId { get; set; }
        public string PayerPassword { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; }
    }
}
