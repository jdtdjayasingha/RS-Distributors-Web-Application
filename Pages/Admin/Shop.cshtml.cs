using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace RsDistributors.Pages.Admin
{
    public class ShopModel : PageModel
    {
        private readonly string _connectionString;

        [BindProperty]
        public Shop Shop { get; set; } = new Shop();

        public List<Shop> Shops { get; set; } = new List<Shop>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public string ErrMsg { get; set; } = string.Empty;

        public ShopModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration));
            
            Categories = new List<SelectListItem>
            {
                new SelectListItem { Value = "Besto", Text = "Besto" },
                new SelectListItem { Value = "Delmo", Text = "Delmo" }
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadShopsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveShopAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadShopsAsync();
                return Page();
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "INSERT INTO ShopTB (Category, Name, Area, Phone) VALUES (@Category, @Name, @Area, @Phone)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Category", Shop.Category);
                        cmd.Parameters.AddWithValue("@Name", Shop.Name);
                        cmd.Parameters.AddWithValue("@Area", Shop.Area);
                        cmd.Parameters.AddWithValue("@Phone", Shop.Phone);
                        await cmd.ExecuteNonQueryAsync();
                        ErrMsg = "Product Added!";
                    }
                }
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrMsg = $"Error: {ex.Message}";
                await LoadShopsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnGetEditShopAsync(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "SELECT * FROM ShopTB WHERE ID = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                Shop = new Shop
                                {
                                    ID = (int)reader["ID"],
                                    Category = reader["Category"]?.ToString() ?? string.Empty,
                                    Name = reader["Name"]?.ToString() ?? string.Empty,
                                    Area = reader["Area"]?.ToString() ?? string.Empty,
                                    Phone = reader["Phone"]?.ToString() ?? string.Empty,
                                };
                            }
                        }
                    }
                }
                await LoadShopsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrMsg = $"Error: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnGetDeleteShopAsync(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "DELETE FROM ShopTB WHERE ID = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        await cmd.ExecuteNonQueryAsync();
                        ErrMsg = "Shop Deleted!";
                    }
                }
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrMsg = $"Error: {ex.Message}";
                return Page();
            }
        }

        private async Task LoadShopsAsync()
        {
            Shops.Clear();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "SELECT * FROM ShopTB";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Shops.Add(new Shop
                                {
                                    ID = (int)reader["ID"],
                                    Category = reader["Category"]?.ToString() ?? string.Empty,
                                    Name = reader["Name"]?.ToString() ?? string.Empty,
                                    Area = reader["Area"]?.ToString() ?? string.Empty,
                                    Phone = reader["Phone"]?.ToString() ?? string.Empty,
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = $"Error: {ex.Message}";
            }
        }
    }

    public class Shop
    {
        public int ID { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}