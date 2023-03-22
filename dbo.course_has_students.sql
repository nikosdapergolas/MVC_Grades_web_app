CREATE TABLE [dbo].[course_has_students] (
    [COURSE_idCOURSE]             INT NOT NULL,
    [STUDENTS_RegistrationNumber] INT NOT NULL,
    [GradeCourseStudent]          INT NOT NULL,
    [Id] INT NOT NULL PRIMARY KEY, 
    CONSTRAINT [FK_courseHasStudents_course_idCOURSE] FOREIGN KEY ([COURSE_idCOURSE]) REFERENCES [dbo].[course] ([idCOURSE]),
    CONSTRAINT [FK_courseHasStudents_students_RegistrationNumber] FOREIGN KEY ([STUDENTS_RegistrationNumber]) REFERENCES [dbo].[students] ([RegistrationNumber])
);

