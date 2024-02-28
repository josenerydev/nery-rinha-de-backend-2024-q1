using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Account.Api;

namespace Account.Api.Data
{
    public class AccountApiContext : DbContext
    {
        public AccountApiContext (DbContextOptions<AccountApiContext> options)
            : base(options)
        {
        }

        public DbSet<Account.Api.Cliente> Cliente { get; set; } = default!;
    }
}
