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
    public class SecretariesController : Controller
    {
        private readonly VathmologioDbContext _context;

        public SecretariesController(VathmologioDbContext context)
        {
            _context = context;
        }

        // GET: Secretaries
        public async Task<IActionResult> Index(string? searchString, int? page = 1)
        {
            if (page != null && page < 1)
            {
                page = 1;
            }

            var pageSize = 4;

            var secretaries = _context.Secretaries.OrderBy(n => n.Name)
                                                  .Include(s => s.UsersUsernameNavigation);

            if (!String.IsNullOrEmpty(searchString))
            {
                ViewData["CurrentFilter"] = searchString;

                secretaries = secretaries.Where(s => s.Name.Contains(searchString) || s.Surname.Contains(searchString))
                                         .OrderBy(n => n.Name)
                                         .Include(s => s.UsersUsernameNavigation);
            }

            ViewData["Page"] = page;
            // await secretaries.ToListAsync()
            return View(secretaries.ToPagedList(page ?? 1, pageSize));

            // Πριν ήταν ο μοναδικός κώδικας που υπήρχε απο εδώ και κάτω
            //var vathmologioDbContext = _context.Secretaries.Include(s => s.UsersUsernameNavigation);
            //return View(await vathmologioDbContext.ToListAsync());
        }

        // GET: Secretaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Secretaries == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Phonenumber == id);

            if (secretary == null)
            {
                return NotFound();
            }

            return View(secretary);
        }

        // GET: Secretaries/Create
        public IActionResult Create()
        {
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username");
            return View();
        }

        public void ProcessCreation(SecretaryUser secretaryUser)
        {
            User user = new User();
            Secretary secretary = new Secretary();

            user.Username = secretaryUser.Username;
            user.Password = secretaryUser.Password;
            user.Role = secretaryUser.Role;

            secretary.Phonenumber = secretaryUser.Phonenumber;
            secretary.Name = secretaryUser.Name;
            secretary.Surname = secretaryUser.Surname;
            secretary.Department = secretaryUser.Department;
            secretary.UsersUsername = user.Username;
            secretary.UsersUsernameNavigation = _context.Users.Find(user.Username);

            _context.Add(user);
            _context.Add(secretary);
            _context.SaveChangesAsync();
        }

        // POST: Secretaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SecretaryUser secretaryUser)
        {
            if (ModelState.IsValid)
            {
                ProcessCreation(secretaryUser);
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", secretaryUser.Username);
            return View(secretaryUser);
        }

        // GET: Secretaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Secretaries == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries.FindAsync(id);
            //secretary.UsersUsernameNavigation = _context.Users.Find(secretary.UsersUsername);
            if (secretary == null)
            {
                return NotFound();
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", secretary.UsersUsername);
            SecretaryEdit secretaryEdit = new SecretaryEdit();
            secretaryEdit.Phonenumber = secretary.Phonenumber;
            secretaryEdit.Name = secretary.Name;
            secretaryEdit.Surname = secretary.Surname;
            secretaryEdit.Department = secretary.Department;
            secretaryEdit.UsersUsername = secretary.UsersUsername;
            return View(secretaryEdit);
        }

        public void ProcessEdit(SecretaryEdit secretaryEdit)
        {
            Secretary secretary = new Secretary();

            secretary.Phonenumber = secretaryEdit.Phonenumber;
            secretary.Name = secretaryEdit.Name;
            secretary.Surname = secretaryEdit.Surname;
            secretary.Department = secretaryEdit.Department;
            secretary.UsersUsername = secretaryEdit.UsersUsername;
            secretary.UsersUsernameNavigation = _context.Users.Find(secretaryEdit.UsersUsername);

            _context.Update(secretary);
            _context.SaveChangesAsync();
        }

        // POST: Secretaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SecretaryEdit secretaryEdit)
        {
            if (id != secretaryEdit.Phonenumber)
            {
                return NotFound();
            }

            if(ModelState.IsValid) 
            {
                try
                {
                    ProcessEdit(secretaryEdit);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecretaryExists(secretaryEdit.Phonenumber))
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
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", secretaryEdit.UsersUsername);
            return View(secretaryEdit);
        }

        // GET: Secretaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Secretaries == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Phonenumber == id);
            if (secretary == null)
            {
                return NotFound();
            }

            return View(secretary);
        }

        // POST: Secretaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Secretaries == null)
            {
                return Problem("Entity set 'VathmologioDbContext.Secretaries'  is null.");
            }
            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary != null)
            {
                _context.Secretaries
                    .Remove(secretary);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SecretaryExists(int id)
        {
          return _context.Secretaries.Any(e => e.Phonenumber == id);
        }

        // GET: Assigned Courses
        public IActionResult Assign_Course(int id)
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "IdCourse", "CourseTitle");
            var student = _context.Students.Find(id);

            var courseHasStudent = _context.CourseHasStudents.Where(s => s.StudentsRegistrationNumber.Equals(student.RegistrationNumber));

            ViewData["StudentRegistrationNumber"] = student.RegistrationNumber;

            return View(courseHasStudent.ToList());
        }

        // POST: Assign a Course
        public IActionResult Proccess_course_assignment(int CourseId)
        {
            return View("Views/Secretaries/Assign_Course.cshtml");
        }


//        <h1>Assign a Course to this Student</h1>
//<div class="row">
//    <div class="col-md-4">
//        <form asp-controller="Secretaries" asp-action="Proccess_course_assignment">
//            <div class="form-group">
//                <label asp-for="CourseIdCourse" class="control-label"></label>
//                <select asp-for="CourseIdCourse" class ="form-control" asp-items="ViewBag.CourseId"></select>
//            </div>
//            <div class="form-group">
//                <input type = "submit" value="Assign" class="btn btn-primary" />
//            </div>
//        </form>
//    </div>
//</div>

        //@*<p>
        //    <a asp-action="Create" class="btn btn-secondary">Assign</a>
        //</p>*@

    }
}
