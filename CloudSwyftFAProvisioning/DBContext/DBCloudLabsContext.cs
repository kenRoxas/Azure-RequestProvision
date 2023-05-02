using FAProvisioning.Models;
using System.Data.Entity;

namespace FAProvisioning.DBContext
{
    public class DBCloudLabsContext : DbContext
    {
        public DBCloudLabsContext() : base("name=DBCloudlabsContext")
        {
        }

        public DbSet<VirtualMachines> VirtualMachines { get; set; }
        public DbSet<VirtualMachineMappings> VirtualMachineMappings { get; set; }
        public DbSet<CloudLabsGroups> CloudLabGroups { get; set; }
        public DbSet<CloudLabsSchedules> CloudLabsSchedules { get; set; }
        public DbSet<VirtualMachineLogs> VirtualMachineLogs { get; set; }
        public DbSet<VEProfileLabCreditMappings> VEProfileLabCreditMappings { get; set; }
        public DbSet<GuacamoleInstances> GuacamoleInstances { get; set; }
        public DbSet<CloudLabUsers> CloudLabUsers { get; set; }
        public DbSet<VirtualEnvironments> VirtualEnvironments { get; set; }
        public DbSet<VirtualEnvironmentImages> VirtualEnvironmentImages { get; set; }

        public DbSet<VEProfiles> VEProfiles { get; set; }

    }
}
