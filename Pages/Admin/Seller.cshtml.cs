using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RsDistributors.Pages.Admin
{
    public class SellerModel : PageModel
    {
        public void OnGet()
        {
        }

        public required string droCategorySeller { get; set; }
    }
}
