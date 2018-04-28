using PayerAccount.Models.Local;
using System.Collections.Generic;

namespace PayerAccount.Models
{
    public class LoginViewModel
    {
        public IEnumerable<Region> Regions { get; }

        public LoginViewModel(IEnumerable<Region> regions)
        {
            Regions = regions;
        }
    }
}
