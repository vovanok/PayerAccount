using Microsoft.AspNetCore.Identity;

namespace PayerAccount.Models.Local
{
    public class User
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
