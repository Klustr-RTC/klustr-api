using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Models;
using Microsoft.EntityFrameworkCore;

namespace Klustr_api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}