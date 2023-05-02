using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class VirtualMachines
    {
        [Key]
        public int VirtualMachineID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int VEProfileID { get; set; }
        [Required]
        public int CourseID { get; set; }
        public string ServiceName { get; set; }
        public string RoleName { get; set; }
        public string Port { get; set; }
        public int IsStarted { get; set; }
        public DateTime LastTimeStamp { get; set; }
        public string NetworkID { get; set; }
        public DateTime DateStartedTrigger { get; set; }
        public string VirtualMachineType { get; set; }
        public int MachineInstance { get; set; }
        public DateTime DateCreated { get; set; }
    }

}
