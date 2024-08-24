using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RsDistributors.Pages.Admin
{
    public class ProductModel : PageModel
    {
        public void OnGet()
        {
        }

        public required string droCategoryProduct { get; set; }
    }
}
