using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
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
}


