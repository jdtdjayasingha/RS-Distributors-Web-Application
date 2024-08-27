using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RsDistributors.Pages.Seller.Besto
{
    public class BestoPage1Model : PageModel
    {
        public void OnGet()
        {
        }

        public required string DropDownShop { get; set; }
    }
}
