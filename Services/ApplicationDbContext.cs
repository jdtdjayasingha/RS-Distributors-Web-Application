using Microsoft.EntityFrameworkCore;

namespace RsDistributors.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
