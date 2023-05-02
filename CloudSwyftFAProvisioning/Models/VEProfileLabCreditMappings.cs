using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class VEProfileLabCreditMappings
    {
        [Key]
        [Column(Order = 1)]
        public int VEProfileID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int GroupID { get; set; }

        public Int64 CourseHours { get; set; }
        public Int64 NumberOfUsers { get; set; }
        public Int64 TotalCourseHours { get; set; }
        public Int64 TotalRemainingCourseHours { get; set; }
    }
}
