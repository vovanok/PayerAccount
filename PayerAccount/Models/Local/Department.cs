using System.Collections.Generic;

namespace PayerAccount.Models.Local
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }

        public List<User> Users { get; set; }
    }
}
