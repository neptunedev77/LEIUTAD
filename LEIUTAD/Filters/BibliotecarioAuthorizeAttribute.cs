using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LEIUTAD.Filters
{
    public class BibliotecarioAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.Session.GetString("UserRole");

            // Verifica se o utilizador tem o papel de Bibliotecário
            if (userRole != "Bibliotecario")
            {
                // Redirecionar para página de acesso negado
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}
