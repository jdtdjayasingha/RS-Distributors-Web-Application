using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RsDistributors.Pages.Admin
{
    public class SellerModel : PageModel
    {
        private readonly string _connectionString;

        [BindProperty]
        public Seller Seller { get; set; } = new Seller();

        public List<Seller> Sellers { get; set; } = new List<Seller>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public string ErrMsg { get; set; } = string.Empty;

        public SellerModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
            Categories = new List<SelectListItem>
            {
                new SelectListItem { Value = "Besto", Text = "Besto" },
                new SelectListItem { Value = "Delmo", Text = "Delmo" }
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSellersAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveSellerAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSellersAsync();
                return Page();
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "INSERT INTO SellerTB (Category, Name, Email, Phone, NIC, Address, Password) VALUES (@Category, @Name, @Email, @Phone, @NIC, @Address, @Password)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Category", Seller.Category);
                        cmd.Parameters.AddWithValue("@Name", Seller.Name);
                        cmd.Parameters.AddWithValue("@Email", Seller.Email);
                        cmd.Parameters.AddWithValue("@Phone", Seller.Phone);
                        cmd.Parameters.AddWithValue("@NIC", Seller.NIC);
                        cmd.Parameters.AddWithValue("@Address", Seller.Address);
                        cmd.Parameters.AddWithValue("@Password", Seller.Password);
                        await cmd.ExecuteNonQueryAsync();
                        ErrMsg = "Seller Added!";
                    }
                }
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrMsg = $"Error: {ex.Message}";
                await LoadSellersAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnGetEditSellerAsync(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "SELECT * FROM SellerTB WHERE ID = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                Seller = new Seller
                                {
                                    ID = (int)reader["ID"],
                                    Category = reader["Category"]?.ToString() ?? string.Empty,
                                    Name = reader["Name"]?.ToString() ?? string.Empty,
                                    Email = reader["Email"]?.ToString() ?? string.Empty,
                                    Phone = reader["Phone"]?.ToString() ?? string.Empty,
                                    NIC = reader["NIC"]?.ToString() ?? string.Empty,
                                    Address = reader["Address"]?.ToString() ?? string.Empty,
                                    Password = reader["Password"]?.ToString() ?? string.Empty,
                                };
                            }
                        }
                    }
                }
                await LoadSellersAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrMsg = $"Error: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnGetDeleteSellerAsync(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "DELETE FROM SellerTB WHERE ID = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        await cmd.ExecuteNonQueryAsync();
                        ErrMsg = "Seller Deleted!";
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

        private async Task LoadSellersAsync()
        {
            Sellers.Clear();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "SELECT * FROM SellerTB";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Sellers.Add(new Seller
                                {
                                    ID = (int)reader["ID"],
                                    Category = reader["Category"]?.ToString() ?? string.Empty,
                                    Name = reader["Name"]?.ToString() ?? string.Empty,
                                    Email = reader["Email"]?.ToString() ?? string.Empty,
                                    Phone = reader["Phone"]?.ToString() ?? string.Empty,
                                    NIC = reader["NIC"]?.ToString() ?? string.Empty,
                                    Address = reader["Address"]?.ToString() ?? string.Empty,
                                    Password = reader["Password"]?.ToString() ?? string.Empty,
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

    public class Seller
    {
        public int ID { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string NIC { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}