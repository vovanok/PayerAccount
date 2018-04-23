using System.Collections.Generic;

namespace PayerAccount.Models.Local
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Department> Departments { get; set; }
    }
}
