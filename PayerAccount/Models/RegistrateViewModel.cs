using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PayerAccount.Models
{
    public class RegistrateViewModel
    {
        public int UserNumber { get; set; }
        public int UserRegionId { get; set; }
        public string UserPassword { get; set; }
        public string UserConfirmPassword { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; }
    }
}
