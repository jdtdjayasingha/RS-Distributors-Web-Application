using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

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