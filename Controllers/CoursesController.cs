using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VathmologioMVC.Models;
using VathmologioMVC.Models.MetaData;
using X.PagedList;

namespace VathmologioMVC.Controllers
{
    public class CoursesController : Controller
    {
        private readonly VathmologioDbContext _context;

        public CoursesController(VathmologioDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index( string? searchString, int? page = 1)
        {
            if (page != null && page < 1)
            {
                page = 1;
            }

            var pageSize = 4;

            var courses = _context.Courses.OrderBy(c => c.CourseTitle)
                                          .Include(c => c.ProfessorsAfmNavigation);

            if (!String.IsNullOrEmpty(searchString))
            {
                ViewData["CurrentFilter"] = searchString;

                courses = courses.Where(c => c.CourseTitle.Contains(searchString))
                                 .OrderBy(c => c.CourseTitle)
                                 .Include(c => c.ProfessorsAfmNavigation);
            }

            ViewData["Page"] = page;
            //courses.ToPagedList(page ?? 1, pageSize);
            return View(courses.ToPagedList(page ?? 1, pageSize));

            // Πριν ήταν ο μοναδικός κώδικας που υπήρχε απο εδώ και κάτω
            //var vathmologioDbContext = _context.Courses.Include(c => c.ProfessorsAfmNavigation);
            //return View(await vathmologioDbContext.ToListAsync());
            }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.ProfessorsAfmNavigation)
                .FirstOrDefaultAsync(m => m.IdCourse == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors, "Afm", "Surname");
            return View();
        }

        public void ProcessCreation(CourseCreation courseCreation)
        {
            Course course = new Course();

            course.CourseTitle = courseCreation.CourseTitle;
            course.CourseSemester = courseCreation.CourseSemester;
            course.ProfessorsAfm = courseCreation.ProfessorsAfm;
            course.ProfessorsAfmNavigation = _context.Professors.Find(courseCreation.ProfessorsAfm);

            _context.Add(course);
            _context.SaveChangesAsync();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreation courseCreation)
        {
            if (ModelState.IsValid)
            {
                ProcessCreation(courseCreation);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors, "Afm", "Surname", courseCreation.ProfessorsAfm);
            return View(courseCreation);
        }

        public void ProcessEdit(CourseEdit courseEdit)
        {
            Course course = new Course();

            course.IdCourse= courseEdit.IdCourse;
            course.CourseTitle = courseEdit.CourseTitle;
            course.CourseSemester = courseEdit.CourseSemester;
            course.ProfessorsAfm = courseEdit.ProfessorsAfm;
            course.ProfessorsAfmNavigation = _context.Professors.Find(courseEdit.ProfessorsAfm);

            _context.Update(course);
            _context.SaveChangesAsync();
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors, "Afm", "Surname", course.ProfessorsAfm);
            CourseEdit courseEdit = new CourseEdit();
            //courseEdit = _context.Courses.Find(id);
            courseEdit.IdCourse = course.IdCourse;
            courseEdit.CourseTitle = course.CourseTitle;
            courseEdit.CourseSemester = course.CourseSemester;
            courseEdit.ProfessorsAfm = course.ProfessorsAfm;
            return View(courseEdit);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseEdit courseEdit)
        {
            if (id != courseEdit.IdCourse)
            {
                return NotFound();
            }
            //if(course.ProfessorsAfmNavigation == null) 
            //{
            //    course.ProfessorsAfmNavigation = _context.Professors.Find(course.ProfessorsAfm);
            //}
            
            if (ModelState.IsValid)
            {
                try
                {
                    ProcessEdit(courseEdit);
                    //_context.Update(course);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(courseEdit.IdCourse))
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
            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors, "Afm", "Surname", courseEdit.ProfessorsAfm);
            return View(courseEdit);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.ProfessorsAfmNavigation)
                .FirstOrDefaultAsync(m => m.IdCourse == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Courses == null)
            {
                return Problem("Entity set 'VathmologioDbContext.Courses'  is null.");
            }
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses
                    .Remove(course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
          return _context.Courses.Any(e => e.IdCourse == id);
        }
    }
}
