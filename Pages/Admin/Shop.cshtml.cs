using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RsDistributors.Pages.Admin
{
    public class ShopModel : PageModel
    {
        public void OnGet()
        {
        }

        public required string droCategoryShop { get; set; }
    }
}
