using VathmologioMVC.Models;

namespace VathmologioMVC.Services
{
    public class SecurityService
    {
        private readonly VathmologioDbContext _context;
        public SecurityService(VathmologioDbContext context) 
        {
            _context = context;
        }

        public bool isValid(User user)
        {
            return _context.Users.Any(e => e.Username == user.Username 
                                        && e.Password == user.Password);
        }
    }
}
