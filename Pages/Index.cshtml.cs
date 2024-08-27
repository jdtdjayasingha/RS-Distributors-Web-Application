using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RsDistributors.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostBtnSaveLoginClick()
        {
            //return RedirectToPage("/Admin/Product");
            return RedirectToPage("/Seller/Besto/BestoPage1");
        }
    }
}
