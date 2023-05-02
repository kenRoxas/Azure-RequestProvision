using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class VirtualMachineLogs
    {
        [Key]
        public int VirtualMachineLogID { get; set; }
        [Required]
        public string RoleName { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int VEProfileID { get; set; }
        public int VirtualMachineID { get; set; }
        [Required]
        public string TimeStamp { get; set; }
        public string Comment { get; set; }
    }
}
