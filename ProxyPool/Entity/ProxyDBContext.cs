namespace ProxyPool
{
    using System;
    using Pomelo.EntityFrameworkCore.MySql;
    using Pomelo.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ProxyDBContext : DbContext
    {
        public ProxyDBContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(ConfigHelper.GetConnectionString());
            }
        }

        public virtual DbSet<Proxy> Proxy { get; set; }

    }
}
