using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using VathmologioMVC.Models;
using VathmologioMVC.Models.MetaData;
using X.PagedList;

namespace VathmologioMVC.Controllers
{
    public class ProfessorsController : Controller
    {
        private readonly VathmologioDbContext _context;
        private readonly string currentUserRole;
        private readonly string currentUserUsername;

        public ProfessorsController(VathmologioDbContext context)
        {
            _context = context;
            currentUserRole = LoginController.Authenticated;
            currentUserUsername = LoginController.AuthenticatedUsername;
        }

        // GET: Professors
        public async Task<IActionResult> Index(string? searchString, int? page = 1)
        {
            // Pagination
            if (page != null && page < 1)
            {
                page = 1;
            }

            var pageSize = 4;

            var professors = _context.Professors.OrderBy(n => n.Name)
                                                .Include(p => p.UsersUsernameNavigation);

            if (!String.IsNullOrEmpty(searchString))
            {
                ViewData["CurrentFilter"] = searchString;

                professors = professors.Where(p => p.Name.Contains(searchString) || p.Surname.Contains(searchString))
                                       .OrderBy(n => n.Name)
                                       .Include(p => p.UsersUsernameNavigation);
            }            

            ViewData["Page"] = page;
            //await professors.ToListAsync()
            return View(professors.ToPagedList(page ?? 1, pageSize));

            // Πριν ήταν ο μοναδικός κώδικας που υπήρχε απο εδώ και κάτω
            //var vathmologioDbContext = _context.Professors.Include(p => p.UsersUsernameNavigation);
            //return View(await vathmologioDbContext.ToListAsync());
        }

        // GET: Professors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .Include(p => p.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Afm == id);

            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // GET: Professors/Create
        public IActionResult Create()
        {
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username");
            return View();
        }

        public void ProcessCreation(ProfessorUser professorUser)
        {
            User user = new User();
            Professor professor = new Professor();

            user.Username = professorUser.Username;
            user.Password = professorUser.Password;
            user.Role = professorUser.Role;

            professor.Name = professorUser.Name;
            professor.Surname = professorUser.Surname;
            professor.Department = professorUser.Department;
            professor.UsersUsername = user.Username;
            professor.UsersUsernameNavigation = _context.Users.Find(user.Username);

            _context.Add(user);
            _context.Add(professor);
            _context.SaveChangesAsync();
        }

        // POST: Professors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfessorUser professorUser)
        {
            if (ModelState.IsValid)
            {
                ProcessCreation(professorUser);
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", professorUser.Username);
            return View(professorUser);
        }

        // GET: Professors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors.FindAsync(id);
            if (professor == null)
            {
                return NotFound();
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", professor.UsersUsername);
            ProfessorEdit professorEdit = new ProfessorEdit();
            professorEdit.Afm = professor.Afm;
            professorEdit.Name = professor.Name;
            professorEdit.Surname = professor.Surname;
            professorEdit.Department = professor.Department;
            professorEdit.UsersUsername = professor.UsersUsername;
            return View(professorEdit);
        }

        public void ProcessEdit(ProfessorEdit professorEdit)
        {
            Professor professor = new Professor();

            professor.Afm = professorEdit.Afm;
            professor.Name = professorEdit.Name;
            professor.Surname = professorEdit.Surname;
            professor.Department = professorEdit.Department;
            professor.UsersUsername = professorEdit.UsersUsername;
            professor.UsersUsernameNavigation = _context.Users.Find(professorEdit.UsersUsername);

            _context.Update(professor);
            _context.SaveChangesAsync();
        }

        // POST: Professors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProfessorEdit professorEdit)
        {
            if (id != professorEdit.Afm)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ProcessEdit(professorEdit);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfessorExists(professorEdit.Afm))
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
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", professorEdit.UsersUsername);
            return View(professorEdit);
        }

        // GET: Professors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .Include(p => p.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Afm == id);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // POST: Professors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Professors == null)
            {
                return Problem("Entity set 'VathmologioDbContext.Professors'  is null.");
            }
            var professor = await _context.Professors.FindAsync(id);
            if (professor != null)
            {
                _context.Professors
                    .Remove(professor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfessorExists(int id)
        {
          return _context.Professors.Any(e => e.Afm == id);
        }

        // GET: Professor
        public IActionResult Authenticated_Profile()
        {
            var professor = _context.Professors.Where(p => p.UsersUsername.Equals(currentUserUsername))
                                                .Include(p => p.UsersUsernameNavigation);
            return View(professor.ToList());
        }

        // Get: Professor's Courses
        public IActionResult Authenticated_Professor_Courses() 
        {
            //Professor professor = new Professor();
            //var professor = _context.Professors.Where(p => p.UsersUsername.Equals(currentUserUsername))
            //                                    .Include(p => p.UsersUsernameNavigation);

            var courses = _context.Courses
                .Where(p => p.ProfessorsAfmNavigation.UsersUsername.Equals(currentUserUsername));
            return View(courses.ToList());
        }

        public IActionResult Show_Scores(string course)
        {
            var courses = _context.CourseHasStudents.Where(p => p.CourseIdCourseNavigation.ProfessorsAfmNavigation.UsersUsername.Equals(currentUserUsername)
                                                             && p.GradeCourseStudent >= 0
                                                             && p.CourseIdCourseNavigation.CourseTitle.Equals(course))
                                                    .OrderBy(o => o.CourseIdCourse)
                                                    .Include(n => n.CourseIdCourseNavigation)
                                                    .Include(n => n.StudentsRegistrationNumberNavigation);
            return View(courses.ToList());
        }

        public IActionResult Register_Scores(string course)
        {
            var courses = _context.CourseHasStudents.Where(p => p.CourseIdCourseNavigation.ProfessorsAfmNavigation.UsersUsername.Equals(currentUserUsername)
                                                             && p.GradeCourseStudent < 0
                                                             && p.CourseIdCourseNavigation.CourseTitle.Equals(course))
                                                    .OrderBy(o => o.CourseIdCourse)
                                                    .Include(n => n.CourseIdCourseNavigation)
                                                    .Include(n => n.StudentsRegistrationNumberNavigation);
            return View(courses.ToList());
        }

        public IActionResult Update_score(CourseHasStudent courseHasStudent) 
        {
            //var student = _context.CourseHasStudents.Find(id);
            //student.GradeCourseStudent = Request.Form[""];
            //_context.CourseHasStudents.Update(student);

            var courses = _context.CourseHasStudents.Where(p => p.CourseIdCourseNavigation.ProfessorsAfmNavigation.UsersUsername.Equals(currentUserUsername)
                                                             && p.GradeCourseStudent < 0)
                                                    .OrderBy(o => o.CourseIdCourse)
                                                    .Include(n => n.CourseIdCourseNavigation)
                                                    .Include(n => n.StudentsRegistrationNumberNavigation);

            return View("Views/Professors/Register_Scores.cshtml", courses);
        }
    }
}
