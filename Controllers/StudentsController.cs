using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VathmologioMVC.Models;
using VathmologioMVC.Models.MetaData;
using X.PagedList;

namespace VathmologioMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly VathmologioDbContext _context;
        private readonly string currentUserRole;
        private readonly string currentUserUsername;

        public StudentsController(VathmologioDbContext context)
        {
            _context = context;
            currentUserRole = LoginController.Authenticated;
            currentUserUsername = LoginController.AuthenticatedUsername;
        }

        // GET: Students
        public async Task<IActionResult> Index(string? searchString, int? page = 1)
        {
            if (page != null && page < 1)
            {
                page = 1;
            }

            var pageSize = 4;

            var students = _context.Students.OrderBy(n => n.Name)
                                            .Include(s => s.UsersUsernameNavigation);

            if (!String.IsNullOrEmpty(searchString))
            {
                ViewData["CurrentFilter"] = searchString;

                students = students.Where(s => s.Name.Contains(searchString) || s.Surname.Contains(searchString))
                                   .OrderBy(n => n.Name)
                                   .Include(s => s.UsersUsernameNavigation);
            }


            ViewData["Page"] = page;
            //await students.ToListAsync()
            return View(students.ToPagedList(page ?? 1, pageSize));

            // Πριν ήταν ο μοναδικός κώδικας που υπήρχε απο εδώ και κάτω
            //var vathmologioDbContext = _context.Students.Include(s => s.UsersUsernameNavigation);
            //return View(await vathmologioDbContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);

            //var student = await _context.CourseHasStudents
            //    .Where(s => s.StudentsRegistrationNumber.Equals(id))
            //    .Include(s => s.StudentsRegistrationNumberNavigation.UsersUsernameNavigation);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username");
            return View();
            //StudentUserController studentUserController = new StudentUserController(_context);
            //studentUserController.Create(studentUser);
        }

        public IActionResult Create2(StudentUser studentUser)
        {
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username");
            //ProcessCreation(studentUser);
            return View();

        }

        public void ProcessCreation(StudentUser studentUser)
        {
            User user= new User();
            Student student= new Student();

            user.Username=studentUser.Username;
            user.Password=studentUser.Password;
            user.Role=studentUser.Role;

            student.Name=studentUser.Name;
            student.Surname=studentUser.Surname;
            student.Department=studentUser.Department;
            student.UsersUsername=user.Username;

            student.UsersUsernameNavigation = _context.Users
                .Find(user.Username);

            _context.Add(user);
            _context.Add(student);
            _context.SaveChangesAsync();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("RegistrationNumber,Name,Surname,Department,UsersUsername")] Student student)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(student);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", student.UsersUsername);
        //    return View(student);
        //}

        // My version of this create method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentUser studentUser)
        {
            if (ModelState.IsValid)
            {
                ProcessCreation(studentUser);
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", studentUser.Username);
            return View(studentUser);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", student.UsersUsername);
            StudentEdit studentEdit = new StudentEdit();
            studentEdit.RegistrationNumber = student.RegistrationNumber;
            studentEdit.Name = student.Name;
            studentEdit.Surname = student.Surname;
            studentEdit.Department = student.Department;
            studentEdit.UsersUsername = student.UsersUsername;
            return View(studentEdit);
        }

        public void ProcessEdit(StudentEdit studentEdit)
        {
            Student student = new Student();

            student.RegistrationNumber = studentEdit.RegistrationNumber;
            student.Name = studentEdit.Name;
            student.Surname = studentEdit.Surname;
            student.Department = studentEdit.Department;
            student.UsersUsername = studentEdit.UsersUsername;
            student.UsersUsernameNavigation = _context.Users.Find(studentEdit.UsersUsername);

            _context.Update(student);
            _context.SaveChangesAsync();

        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentEdit studentEdit)
        {
            if (id != studentEdit.RegistrationNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ProcessEdit(studentEdit);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(studentEdit.RegistrationNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", studentEdit.UsersUsername);
            return View(studentEdit);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'VathmologioDbContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students
                    .Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
          return _context.Students.Any(e => e.RegistrationNumber == id);
        }

        public IActionResult Authenticated_Profile()
        {
            var student = _context.Students.Where(p => p.UsersUsername.Equals(currentUserUsername))
                                                .Include(p => p.UsersUsernameNavigation);
            return View(student.ToList());
        }

        // Get: Student's Courses
        public IActionResult Authenticated_Student_Courses_By_Semester()
        {
            var courses = _context.CourseHasStudents
                .Where(p => p.StudentsRegistrationNumberNavigation.UsersUsername.Equals(currentUserUsername))
                .OrderBy(p => p.CourseIdCourseNavigation.CourseSemester)
                .Include(p => p.CourseIdCourseNavigation)
                .Include(p => p.StudentsRegistrationNumberNavigation);
            return View(courses.ToList());
        }

        // Get: Student's Courses
        public IActionResult Authenticated_Student_Courses_By_Course_Title()
        {
            var courses = _context.CourseHasStudents
                .Where(p => p.StudentsRegistrationNumberNavigation.UsersUsername.Equals(currentUserUsername))
                .OrderBy(p => p.CourseIdCourseNavigation.CourseTitle)
                .Include(p => p.CourseIdCourseNavigation)
                .Include(p => p.StudentsRegistrationNumberNavigation);
            return View(courses.ToList());
        }

        // Get: Student's Courses
        public IActionResult Authenticated_Student_Courses_By_Semester_excluding_non_graded()
        {
            var courses = _context.CourseHasStudents
                .Where(p => p.StudentsRegistrationNumberNavigation.UsersUsername.Equals(currentUserUsername)
                         && p.GradeCourseStudent >= 0)
                .OrderBy(p => p.CourseIdCourseNavigation.CourseSemester)
                .Include(p => p.CourseIdCourseNavigation)
                .Include(p => p.StudentsRegistrationNumberNavigation);
            return View(courses.ToList());
        }
    }
}
