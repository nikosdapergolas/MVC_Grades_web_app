using System;
using System.Collections.Generic;

namespace VathmologioMVC.Models;

public partial class Student
{
    public int RegistrationNumber { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string UsersUsername { get; set; } = null!;

    public virtual ICollection<CourseHasStudent> CourseHasStudents { get; } = new List<CourseHasStudent>();

    public virtual User UsersUsernameNavigation { get; set; } = null!;
}
