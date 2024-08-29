using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace RsDistributors.Pages.Admin
{
    public class ProductModel : PageModel
    {
        private readonly string _connectionString;

        [BindProperty]
        public Product Product { get; set; } = new Product();

        public List<Product> Products { get; set; } = new List<Product>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public string ErrMsg { get; set; } = string.Empty;

        public ProductModel(IConfiguration configuration)
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
            await LoadProductsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveProductAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadProductsAsync();
                return Page();
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "INSERT INTO ProductTB (Category, Name, Price) VALUES (@Category, @Name, @Price)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Category", Product.Category);
                        cmd.Parameters.AddWithValue("@Name", Product.Name);
                        cmd.Parameters.AddWithValue("@Price", Product.Price);
                        await cmd.ExecuteNonQueryAsync();
                        ErrMsg = "Product Added!";
                    }
                }
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrMsg = $"Error: {ex.Message}";
                await LoadProductsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnGetEditProductAsync(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "SELECT * FROM ProductTB WHERE ID = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                Product = new Product
                                {
                                    ID = (int)reader["ID"],
                                    Category = reader["Category"]?.ToString() ?? string.Empty,
                                    Name = reader["Name"]?.ToString() ?? string.Empty,
                                    Price = reader["Price"]?.ToString() ?? string.Empty,
                                };
                            }
                        }
                    }
                }
                await LoadProductsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrMsg = $"Error: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnGetDeleteProductAsync(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "DELETE FROM ProductTB WHERE ID = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        await cmd.ExecuteNonQueryAsync();
                        ErrMsg = "Product Deleted!";
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

        private async Task LoadProductsAsync()
        {
            Products.Clear();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    string query = "SELECT * FROM ProductTB";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Products.Add(new Product
                                {
                                    ID = (int)reader["ID"],
                                    Category = reader["Category"]?.ToString() ?? string.Empty,
                                    Name = reader["Name"]?.ToString() ?? string.Empty,
                                    Price = reader["Price"]?.ToString() ?? string.Empty,
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

    public class Product
    {
        public int ID { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
    }
}