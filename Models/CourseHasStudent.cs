﻿using System;
using System.Collections.Generic;

namespace VathmologioMVC.Models;

public partial class CourseHasStudent
{
    public int CourseIdCourse { get; set; }

    public int StudentsRegistrationNumber { get; set; }

    public int GradeCourseStudent { get; set; }

    public int Id { get; set; }

    public virtual Course CourseIdCourseNavigation { get; set; } = null!;

    public virtual Student StudentsRegistrationNumberNavigation { get; set; } = null!;
}
