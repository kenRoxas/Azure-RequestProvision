using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class Tenants
    {
        [Key]
        public int TenantId { get; set; }
        public string ApiUrl { get; set; }
        public string AuthConnectionString { get; set; }
        public string Code { get; set; }
        public string EdxUrl { get; set; }
        public string TenantName { get; set; }
        public string GuacConnection { get; set; }
        public string GuacamoleURL { get; set; }
        public int SubscriptionMinutes { get; set; }
        public string AzurePort { get; set; }
        public string SubscriptionId { get; set; }
    }
    public class DataOps
    {
        public string ApiUrl { get; set; }
        public string TenantPrefix { get; set; }
        public string TenantName { get; set; }
    }

}
