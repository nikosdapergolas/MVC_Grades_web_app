using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VathmologioMVC.Models;
using VathmologioMVC.Services;

namespace VathmologioMVC.Controllers
{
    public class LoginController : Controller
    {
        public static string Authenticated { get; private set; }
        public static string AuthenticatedUsername { get; private set; }

        private readonly VathmologioDbContext _context;

        public LoginController(VathmologioDbContext context)
        {
            _context = context;
            Authenticated= null;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [CustomAuthorization]
        public IActionResult PrivateSectionMustBeLoggedIn()
        {
            // To be removed
            return Content("I am a private method");
        }

        public IActionResult ProcessLogin(User user)
        {
            User? logged_in_user = _context.Users.Find(user.Username);
            SecurityService securityService = new SecurityService(_context);

            if (securityService.isValid(user))
            {
                Authenticated = logged_in_user.Role;
                AuthenticatedUsername = user.Username;
                HttpContext.Session.SetString("role", logged_in_user.Role);
                //return View("LoginSuccess", logged_in_user);
                return View("Views/Home/Index.cshtml", logged_in_user);
            }
            else
            {
                HttpContext.Session.Remove("role");
                return View("LoginFailure", user);
            }
        }
    }
}
