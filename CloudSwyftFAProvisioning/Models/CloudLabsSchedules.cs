using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class CloudLabsSchedules
    {
        [Key]
        public int CloudLabsScheduleId { get; set; }

        public int VEProfileID { get; set; }
        public int UserId { get; set; }
        public string ScheduledBy { get; set; }
        public DateTime DateCreated { get; set; }
        public double LabHoursRemaining { get; set; }
        public double LabHoursTotal { get; set; }
        public DateTime? StartLabTriggerTime { get; set; }
        public DateTime? RenderPageTriggerTime { get; set; }

        public double? InstructorLabHours { get; set; }
        public DateTime? InstructorLastAccess { get; set; }
    }
}
