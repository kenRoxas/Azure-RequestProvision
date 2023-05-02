using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class VEProfiles
    {
        [Key]
        public int VEProfileID { get; set; }
        public int CourseID { get; set; }

        [ForeignKey("VirtualEnvironment")]
        public int VirtualEnvironmentID { get; set; }
        public VirtualEnvironments VirtualEnvironment { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int ConnectionLimit { get; set; }
        public string ThumbnailURL { get; set; }
        public int IsEnabled { get; set; }
        public DateTime DateProvisionTrigger { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
        public int IsEmailEnabled { get; set; }
        public int PassingRate { get; set; }
        public int ExamPassingRate { get; set; }
        //public bool ShowExamPassingRate { get; set; }
    }
}
