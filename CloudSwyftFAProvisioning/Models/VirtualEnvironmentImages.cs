using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class VirtualEnvironmentImages
    {
        [Key]
        public int VirtualEnvironmentImagesID { get; set; }
        public int VirtualEnvironmentID { get; set; }
        public VirtualEnvironments VirtualEnvironment { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Protocol { get; set; }
    }
}
