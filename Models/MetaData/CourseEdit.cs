namespace VathmologioMVC.Models.MetaData
{
    public class CourseEdit
    {
        public int IdCourse { get; set; }

        public string CourseTitle { get; set; } = null!;

        public string CourseSemester { get; set; } = null!;

        public int ProfessorsAfm { get; set; }
    }
}
