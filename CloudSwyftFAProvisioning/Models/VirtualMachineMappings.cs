using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class VirtualMachineMappings
    {
        [Key]
        public string RoleName { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int VEProfileID { get; set; }
        [Required]
        public int CourseID { get; set; }
        public int MachineInstance { get; set; }
        public int IsProvisioned { get; set; }
        public int IsLaasProvisioned { get; set; }
    }
}
