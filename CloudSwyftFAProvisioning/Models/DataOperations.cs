using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAProvisioning.Models
{
    public class DataOperations
    {
        public const string Windows = "Windows";
        public const string Linux = "Linux";
        public string Operation { get; set; }
        public string Prefix { get; set; }
        public int UserID { get; set; }
        public int CourseID { get; set; }
        public int VEProfileID { get; set; }
        public string ImageName { get; set; }
        public string ImageSize { get; set; }
        public string ImageType { get; set; }
        public string Protocol { get; set; }
        public string OSFamily { get; set; }
        public string Suffix { get; set; }
        public string AzurePort { get; set; }
        public string GuacConnection { get; set; }
        public string WebApiUrl { get; set; }
        public string GuacamoleUrl { get; set; }
        public string Region { get; set; }
        public string MachineName { get; set; }
        public string RoleName
        {
            get
            {
                if (string.IsNullOrEmpty(Prefix))
                {
                    return string.IsNullOrEmpty(Suffix)
                        ? string.Format("{0}-{1}-{2}", CourseID, VEProfileID, UserID)
                        : string.Format("{0}-{1}-{2}-{3}", CourseID, VEProfileID,
                            UserID, Suffix);
                }
                else
                {
                    return string.IsNullOrEmpty(Suffix)
                        ? string.Format("{0}-{1}-{2}-{3}", Prefix, CourseID, VEProfileID, UserID)
                        : string.Format("{0}-{1}-{2}-{3}-{4}", Prefix, CourseID, VEProfileID, UserID, Suffix);
                }

            }
        }
        public int GroupID { get; set; }
        public Int64 CourseHours { get; set; }
        public string ScheduledBy { get; set; }
        public string ApiUrl { get; set; }
        public string ResourceGroup { get; set; }
        public string TenantName { get; set; }
        public bool IsLaasProvision { get; set; }
    }

    public class VirtualMachineOperations
    {
        public int VirtualMachineID { get; set; }
        public int UserID { get; set; }
        public int VEProfileID { get; set; }
        public int CourseID { get; set; }
        public string ServiceName { get; set; }
        public string RoleName { get; set; }
        public List<GuacamoleInstanceOperations> GuacamoleInstances { get; set; }
        public string Port { get; set; }
        public int IsStarted { get; set; }
        public DateTime LastTimeStamp { get; set; }
        public DateTime DateStartedTrigger { get; set; }
        public string NetworkID { get; set; }
        public int MachineInstance { get; set; }
        public DateTime DateCreated { get; set; }
        public int LabHoursPerCourse { get; set; }
        public int GroupID { get; set; }
        public int TotalRemainingHours { get; set; }
    }

    public class GuacamoleInstanceOperations
    {
        public int GuacamoleInstanceID { get; set; }
        public string Hostname { get; set; }
        public string Connection_Name { get; set; }
        public string Url { get; set; }
        public int GuacLinkType { get; set; }
        public int VirtualMachineID { get; set; }
    }

}
