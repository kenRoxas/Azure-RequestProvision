using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class CloudLabsGroups
    {
        [Key]
        public int CloudLabsGroupID { get; set; }
        public string GroupName { get; set; }
        public string EdxUrl { get; set; }
        public int TenantId { get; set; }
        public string CLUrl { get; set; }
        public Int64 SubscriptionHourCredits { get; set; }
        public Int64 SubscriptionRemainingHourCredits { get; set; }
        public string CLPrefix { get; set; }
    }
}
