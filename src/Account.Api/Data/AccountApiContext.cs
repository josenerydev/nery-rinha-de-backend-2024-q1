using Account.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace Account.Api.Data
{
    public class AccountApiContext : DbContext
    {
        public AccountApiContext(DbContextOptions<AccountApiContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } = default!;
    }
}