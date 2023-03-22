namespace VathmologioMVC.Models.MetaData
{
    public class StudentEdit
    {
        public int RegistrationNumber { get; set; }

        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Department { get; set; } = null!;

        public string UsersUsername { get; set; } = null!;
    }
}
