using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace VathmologioMVC.Controllers
{

    internal class CustomAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //string? userName = context.HttpContext.Session.GetString("username");
            string? role = context.HttpContext.Session.GetString("role");

            if (role == null)
            {
                context.Result = new RedirectResult("/login");
            }
            else
            {
                // Nothing
            }
            //context.Result = new RedirectResult("/login");
        }
    }
}