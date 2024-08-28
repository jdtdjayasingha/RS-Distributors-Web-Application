using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RsDistributors.Pages.Seller.Besto
{
    public class BestoPage2Model : PageModel
    {
        private readonly string? _connectionString;

        public BestoPage2Model(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [BindProperty]
        public string SelectedProductId { get; set; } = string.Empty;

        [BindProperty]
        public string Price { get; set; } = string.Empty;

        [BindProperty]
        public string Quantity { get; set; } = string.Empty;

        [BindProperty]
        public string Total { get; set; } = string.Empty;

        public string ErrMsg { get; set; } = string.Empty;

        public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();
        public List<BillItem> BillItems { get; set; } = new List<BillItem>();
        public string ShopName { get; set; } = string.Empty;
        public double GrandTotal { get; set; }

        public void OnGet()
        {
            // Retrieve the shop name from the session
            //ShopName = HttpContext.Session.GetString("shopName") ?? string.Empty;

            LoadDropDownLists();
            ShowProducts();
            CalculateTotal();
        }

        public void OnPost()
        {
            if (!string.IsNullOrEmpty(SelectedProductId))
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    var cmd = new SqlCommand("SELECT Price FROM ProductTB WHERE ID=@ProductID", con);
                    cmd.Parameters.AddWithValue("@ProductID", SelectedProductId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            Price = dr["Price"]?.ToString() ?? string.Empty;
                        }
                    }
                }
            }

            LoadDropDownLists(); // Reload dropdown lists
        }

        public IActionResult OnPostPrintBill()
        {
            try
            {
                // Insert GrandTotal into the TotalDB table
                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    var query = "INSERT INTO TotalDB (Total) VALUES (@GrandTotal)";
                    var cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@GrandTotal", GrandTotal);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        ErrMsg = "GrandTotal Saved!";
                    }
                    else
                    {
                        ErrMsg = "Failed to save GrandTotal.";
                    }
                }
            }
            catch (SqlException ex)
            {
                ErrMsg = $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrMsg = $"General Error: {ex.Message}";
            }

            // Stay on the same page to display the message
            return Page();
        }

        public IActionResult OnPostAddToBill()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedProductId) || string.IsNullOrEmpty(Quantity) || string.IsNullOrEmpty(Price))
                {
                    ErrMsg = "Missing Data";
                    return Page(); // Stay on the same page
                }

                // Convert Quantity and Price to appropriate types (decimal)
                if (!decimal.TryParse(Quantity, out decimal quantity) || !decimal.TryParse(Price, out decimal price))
                {
                    ErrMsg = "Invalid Quantity or Price";
                    return Page(); // Stay on the same page
                }

                // Calculate Total
                decimal total = quantity * price;
                Total = total.ToString("F2"); // Format total to 2 decimal places

                string productName = string.Empty;

                // Fetch product name based on the selected product ID
                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    var cmd = new SqlCommand("SELECT Name FROM ProductTB WHERE ID=@ProductID", con);
                    cmd.Parameters.AddWithValue("@ProductID", SelectedProductId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            productName = dr["Name"]?.ToString() ?? string.Empty;
                        }
                    }
                }

                // Ensure that a product name was retrieved
                if (string.IsNullOrEmpty(productName))
                {
                    ErrMsg = "Product not found";
                    return Page(); // Stay on the same page
                }

                // Insert data into the BillTB table
                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    var query = "INSERT INTO BillTB (Name, Quantity, Price, Total) VALUES (@Name, @Quantity, @Price, @Total)";
                    var cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", productName); // Use the product name
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Total", total);

                    cmd.ExecuteNonQuery();
                    ErrMsg = "Product Saved!";
                }

                // Reload the bill items and recalculate the total
                ShowProducts();
                CalculateTotal();
            }
            catch (SqlException ex)
            {
                ErrMsg = ex.Message;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }

            return RedirectToPage(); // Refresh the page
        }

        private void LoadDropDownLists()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM ProductTB WHERE Category='Besto'", con);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Products.Add(new SelectListItem
                        {
                            Value = dr["ID"].ToString(),
                            Text = dr["Name"].ToString()
                        });
                    }
                }
            }
        }

        private void ShowProducts()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM BillTB", con);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        BillItems.Add(new BillItem
                        {
                            Name = dr["Name"]?.ToString() ?? string.Empty,
                            Quantity = dr["Quantity"]?.ToString() ?? string.Empty,
                            Price = dr["Price"]?.ToString() ?? string.Empty,
                            Total = dr["Total"]?.ToString() ?? string.Empty,
                        });
                    }
                }
            }
        }

        private void CalculateTotal()
        {
            GrandTotal = BillItems.Sum(item => double.Parse(item.Total));
        }

        public class BillItem
        {
            public string Name { get; set; } = string.Empty;
            public string Quantity { get; set; } = string.Empty;
            public string Price { get; set; } = string.Empty;
            public string Total { get; set; } = string.Empty;
        }

        public async Task<JsonResult> OnGetProductPrice(string productId)
        {
            string price = string.Empty;

            if (!string.IsNullOrEmpty(productId))
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    await con.OpenAsync();
                    var cmd = new SqlCommand("SELECT Price FROM ProductTB WHERE ID=@ProductID", con);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            price = dr["Price"]?.ToString() ?? string.Empty;
                        }
                    }
                }
            }

            return new JsonResult(new { Price = price });
        }
    }
}