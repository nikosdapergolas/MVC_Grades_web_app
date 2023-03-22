using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VathmologioMVC.Models;
using VathmologioMVC.Models.MetaData;

namespace VathmologioMVC.Controllers
{
    public class CourseHasStudentsController : Controller
    {
        private readonly VathmologioDbContext _context;

        public CourseHasStudentsController(VathmologioDbContext context)
        {
            _context = context;
        }

        // GET: CourseHasStudents
        public async Task<IActionResult> Index()
        {
            var vathmologioDbContext = _context.CourseHasStudents.Include(c => c.CourseIdCourseNavigation).Include(c => c.StudentsRegistrationNumberNavigation);
            return View(await vathmologioDbContext.ToListAsync());
        }

        // GET: CourseHasStudents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CourseHasStudents == null)
            {
                return NotFound();
            }

            var courseHasStudent = await _context.CourseHasStudents
                .Include(c => c.CourseIdCourseNavigation)
                .Include(c => c.StudentsRegistrationNumberNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseHasStudent == null)
            {
                return NotFound();
            }

            return View(courseHasStudent);
        }

        // GET: CourseHasStudents/Create
        public IActionResult Create(int id)
        {
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "CourseTitle");

            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students
                                                                                .Where(s => s.RegistrationNumber
                                                                                .Equals(id)), "RegistrationNumber", "Surname");

            //ViewData["StudentsRegistrationNumber"] = _context.Students.Where(s => s.RegistrationNumber.Equals(id));
            return View();
        }

        public void processCreation(CourseAssign courseAssign)
        {
            CourseHasStudent courseHasStudent = new CourseHasStudent();
            courseHasStudent.CourseIdCourse = courseAssign.CourseIdCourse;
            courseHasStudent.StudentsRegistrationNumber = courseAssign.StudentsRegistrationNumber;
            courseHasStudent.Id = courseAssign.CourseIdCourse + courseAssign.StudentsRegistrationNumber;
            courseHasStudent.GradeCourseStudent = -1;

            courseHasStudent.StudentsRegistrationNumberNavigation = _context.Students.Find(courseAssign.StudentsRegistrationNumber);
            courseHasStudent.CourseIdCourseNavigation = _context.Courses.Find(courseAssign.CourseIdCourse);

            if(CourseHasStudentExists(courseHasStudent.Id) == false)
            {
                _context.Add(courseHasStudent);
                _context.SaveChangesAsync();
            }            
        }

        // POST: CourseHasStudents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseAssign courseAssign)
        {
            if (ModelState.IsValid)
            {
                processCreation(courseAssign);
                //StudentsController studentsController = new StudentsController(_context);
                return RedirectToAction(nameof(Index));
                //return View("Views/Students/Index.cshtml");
            }
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "IdCourse", courseAssign.CourseIdCourse);
            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "RegistrationNumber", courseAssign.StudentsRegistrationNumber);
            return View(courseAssign);
        }

        // GET: CourseHasStudents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CourseHasStudents == null)
            {
                return NotFound();
            }

            var courseHasStudent = await _context.CourseHasStudents.FindAsync(id);
            if (courseHasStudent == null)
            {
                return NotFound();
            }
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "CourseTitle", courseHasStudent.CourseIdCourse);
            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "Surname", courseHasStudent.StudentsRegistrationNumber);
            
            RegisterGrade registerGradeForStudent = new RegisterGrade();
            registerGradeForStudent.Id = courseHasStudent.Id;
            registerGradeForStudent.CourseIdCourse = courseHasStudent.CourseIdCourse;
            registerGradeForStudent.StudentsRegistrationNumber = courseHasStudent.StudentsRegistrationNumber;
            registerGradeForStudent.GradeCourseStudent = courseHasStudent.GradeCourseStudent;

            return View(registerGradeForStudent);
        }

        public void proccessEdit(RegisterGrade registerGradeForStudent)
        {
            CourseHasStudent courseHasStudent = new CourseHasStudent();
            courseHasStudent.Id= registerGradeForStudent.Id;
            courseHasStudent.CourseIdCourse = registerGradeForStudent.CourseIdCourse;
            courseHasStudent.GradeCourseStudent = registerGradeForStudent.GradeCourseStudent;
            courseHasStudent.StudentsRegistrationNumber = registerGradeForStudent.StudentsRegistrationNumber;

            courseHasStudent.StudentsRegistrationNumberNavigation = _context.Students
                                                        .Find(registerGradeForStudent.StudentsRegistrationNumber);
            courseHasStudent.CourseIdCourseNavigation = _context.Courses
                                                        .Find(registerGradeForStudent.CourseIdCourse);

            _context.CourseHasStudents.Update(courseHasStudent);
            _context.SaveChangesAsync();
        }

        // POST: CourseHasStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegisterGrade registerGradeForStudent)
        {
            if (id != registerGradeForStudent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    proccessEdit(registerGradeForStudent);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseHasStudentExists(registerGradeForStudent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ProfessorsController professorsController = new ProfessorsController(_context);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "IdCourse", registerGradeForStudent.CourseIdCourse);
            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "RegistrationNumber", registerGradeForStudent.StudentsRegistrationNumber);
            return View(registerGradeForStudent);
        }

        // GET: CourseHasStudents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CourseHasStudents == null)
            {
                return NotFound();
            }

            var courseHasStudent = await _context.CourseHasStudents
                .Include(c => c.CourseIdCourseNavigation)
                .Include(c => c.StudentsRegistrationNumberNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseHasStudent == null)
            {
                return NotFound();
            }

            return View(courseHasStudent);
        }

        // POST: CourseHasStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CourseHasStudents == null)
            {
                return Problem("Entity set 'VathmologioDbContext.CourseHasStudents'  is null.");
            }
            var courseHasStudent = await _context.CourseHasStudents.FindAsync(id);
            if (courseHasStudent != null)
            {
                _context.CourseHasStudents.Remove(courseHasStudent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseHasStudentExists(int id)
        {
          return _context.CourseHasStudents.Any(e => e.Id == id);
        }
    }
}
