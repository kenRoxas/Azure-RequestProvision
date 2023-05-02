using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class GuacamoleInstances
    {
        [Key]
        public int GuacamoleInstanceID { get; set; }
        public string Hostname { get; set; }
        public string Connection_Name { get; set; }
        public string Url { get; set; }
        [DefaultValue(1)]
        public int GuacLinkType { get; set; }

        [ForeignKey("VirtualMachine")]
        public int VirtualMachineID { get; set; }
        public VirtualMachines VirtualMachine { get; set; }
    }

}
