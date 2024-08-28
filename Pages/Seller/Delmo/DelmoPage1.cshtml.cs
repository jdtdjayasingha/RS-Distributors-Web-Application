using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RsDistributors.Pages.Seller.Delmo
{
    public class DelmoPage1Model : PageModel
    {
        private readonly string? _connectionString;
        public DelmoPage1Model(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<SelectListItem> Shops { get; set; } = new List<SelectListItem>();
        [BindProperty]
        public required string SelectedShop { get; set; }
        public required string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadShopsAsync();
        }

        public async Task<IActionResult> OnPostNextAsync()
        {
            try
            {
                await DelBillAsync();
                
                return RedirectToPage("DelmoPage2");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("DelmoPage2");
            }
        }

        

        private async Task DelBillAsync()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand("DELETE FROM BillTB", con))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}