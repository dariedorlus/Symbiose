using Microsoft.EntityFrameworkCore;
using Symbiose.Mail.Models;

namespace Symbiose.Mail.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Email> Emails { get; set; }
    }
}
