using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace RsDistributors.Pages.Seller.Delmo
{
    public class DelmoPage2Model : PageModel
    {
        private readonly string? _connectionString;

        public DelmoPage2Model(IConfiguration configuration)
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

            LoadDropDownLists();
        }

        public IActionResult OnPostPrintBill()
        {
            try
            {
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
            return Page();
        }

        public IActionResult OnPostAddToBill()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedProductId) || string.IsNullOrEmpty(Quantity) || string.IsNullOrEmpty(Price))
                {
                    ErrMsg = "Missing Data";
                    return Page();
                }

                if (!decimal.TryParse(Quantity, out decimal quantity) || !decimal.TryParse(Price, out decimal price))
                {
                    ErrMsg = "Invalid Quantity or Price";
                    return Page();
                }

                decimal total = quantity * price;
                Total = total.ToString("F2");

                string productName = string.Empty;

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

                if (string.IsNullOrEmpty(productName))
                {
                    ErrMsg = "Product not found";
                    return Page();
                }

                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    var query = "INSERT INTO BillTB (Name, Quantity, Price, Total) VALUES (@Name, @Quantity, @Price, @Total)";
                    var cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", productName);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Total", total);

                    cmd.ExecuteNonQuery();
                    ErrMsg = "Product Saved!";
                }

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

            return RedirectToPage();
        }

        private void LoadDropDownLists()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM ProductTB WHERE Category='Delmo'", con);
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
