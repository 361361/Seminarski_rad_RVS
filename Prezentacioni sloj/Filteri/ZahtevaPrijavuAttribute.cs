using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RVS_Aplikacija.Filteri
{
    // Action filter - primenjuje se na kontrolere/akcije kojima je potrebna prijava.
    // Ako u sesiji nema "KorisnikId", korisnik se preusmerava na formu za prijavu.
    public class ZahtevaPrijavuAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sesija = context.HttpContext.Session;
            if (sesija.GetInt32("KorisnikId") == null)
            {
                context.Result = new RedirectToActionResult("Prijava", "Nalog", null);
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
