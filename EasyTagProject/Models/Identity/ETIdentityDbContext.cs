using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.Identity
{
    public class ETIdentityDbContext: IdentityDbContext<EasyTagUser>
    {
        public ETIdentityDbContext(DbContextOptions<ETIdentityDbContext> options): base(options)
        {

        }
    }
}
