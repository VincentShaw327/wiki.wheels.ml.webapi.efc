using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;


namespace WebApi.DataAccess.Base
{
    public class AlanContext : DbContext
    {
        public AlanContext(DbContextOptions<AlanContext> options)
    :   
        base(options){ }

        public DbSet<User> User { get; set; }
    }
}
