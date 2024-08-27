using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace RsDistributors.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string? _connectionString;
        public IndexModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [BindProperty]
        public required string emailLogin { get; set; }
        [BindProperty]
        public required string passwordLogin { get; set; }
        public required string infoMsg { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string query = "SELECT * FROM SellerTB WHERE Email=@Email AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", emailLogin);
                cmd.Parameters.AddWithValue("@Password", passwordLogin);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    string passStatus = dt.Rows[0]["Category"]?.ToString() ?? string.Empty;

                    if (passStatus == "besto")
                    {
                        return RedirectToPage("/Seller/Besto/BestoPage1");
                    }
                    else if (passStatus == "delmo")
                    {
                        return RedirectToPage("/Seller/Delmo/DelmoPage1");
                    }
                    else
                    {
                        infoMsg = "Invalid Role!";
                    }
                }
                else
                {
                    if (emailLogin == "admin@gmail.com" && passwordLogin == "admin")
                    {
                        return RedirectToPage("Admin/Dashboard");
                    }
                    else
                    {
                        infoMsg = "Invalid username or password!";
                    }
                }
            }

            return Page();
        }
    }
}


