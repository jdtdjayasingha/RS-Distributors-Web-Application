using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace RsDistributors.View.Seller.Besto
{
    public class BestoPage1Model : PageModel
    {
        private readonly string? _connectionString;
        public BestoPage1Model(IConfiguration configuration)
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
                
                return RedirectToPage("BestoPage2");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("BestoPage2");
            }
        }

        private async Task LoadShopsAsync()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand("SELECT ID, Name FROM ShopTB WHERE Category='Besto'", con))
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Shops.Add(new SelectListItem
                        {
                            Value = reader["ID"].ToString(),
                            Text = reader["Name"].ToString()
                        });
                    }
                }
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