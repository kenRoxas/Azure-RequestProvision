using FAProvisioning.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.DBContext
{
    public class DBTenantContext : DbContext
    {
        public DBTenantContext() : base("name=DBTenantContext")
        {
        }

        public DbSet<Tenants> Tenants { get; set; }

    }
}