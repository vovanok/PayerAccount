namespace PayerAccount.Dal.Local.Data
{
    public class User
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
