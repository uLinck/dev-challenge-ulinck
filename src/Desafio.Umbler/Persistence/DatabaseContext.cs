using Desafio.Umbler.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Umbler.Persistence
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        public DbSet<Domain> Domains { get; set; }
    }
}