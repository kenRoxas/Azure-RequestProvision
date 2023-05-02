using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using FAProvisioning.DBContext;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FAProvisioning
{
    public static class CheckAvailProvision
    {
        private static DBCloudLabsContext _db = new DBCloudLabsContext();
        [FunctionName("CheckAvailProvision")]
        public static async Task<int> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                string courseCode = req.GetQueryNameValuePairs()
                   .FirstOrDefault(q => string.Compare(q.Key, "CourseCode", true) == 0)
                   .Value;

                string email = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "email", true) == 0)
                    .Value;

                var userGroup = _db.CloudLabUsers.Where(x => x.Email == email).Select(q => q.UserGroup).FirstOrDefault();

                var userId = _db.CloudLabUsers.Where(x => x.Email == email).Select(q => q.UserId).FirstOrDefault();
                log.Info(userGroup.ToString() + " -Usergroup");
                log.Info(userId.ToString() + " - Userid");

                var veProfilesMapping = _db.VEProfileLabCreditMappings.Where(x => x.GroupID == userGroup).ToList();

                var vep = _db.VEProfileLabCreditMappings.Join(_db.VEProfiles,
                    a => a.VEProfileID,
                    b => b.VEProfileID,
                    (a, b) => new { a, b })
                    .Join(_db.VirtualEnvironments,
                    c => c.b.VirtualEnvironmentID,
                    d => d.VirtualEnvironmentID,
                    (c, d) => new { c, d })
                    .Where(q => q.d.Description.ToUpper() == courseCode.ToUpper() && q.c.a.GroupID == userGroup).Select(x => x.c.a.VEProfileID).ToList();



                var virtualEnvironmentID = _db.VirtualEnvironments.Where(x => x.Description == courseCode).Select(q => q.VirtualEnvironmentID).FirstOrDefault();
                log.Info(virtualEnvironmentID.ToString() + " -virtualEnvironmentID");

                var veprofiles = _db.VEProfiles.Where(x => x.VirtualEnvironmentID == virtualEnvironmentID).ToList();

                log.Info(veprofiles.ToString() + " -veprofiles");

                foreach (var veprofileid in vep)
                {
                    log.Info(veprofileid.ToString() + " -veprofileid");
                    

                    if (_db.VEProfileLabCreditMappings.Where(x=> x.GroupID == userGroup && x.VEProfileID == veprofileid).Count() > 0)
                        if (_db.VirtualMachineMappings.Where(x => x.UserID == userId && x.VEProfileID == veprofileid).Count() == 0 &&
                            _db.CloudLabGroups.Where(q => q.CloudLabsGroupID == userGroup).Select(s => new { s.SubscriptionRemainingHourCredits }).FirstOrDefault().SubscriptionRemainingHourCredits != 0)
                            return 1; 
                        
                    
                    //if (_db.VEProfileLabCreditMappings.Where(x => x.VEProfileID == veprofileid.VEProfileID && x.GroupID == userGroup).Count() > 0)
                    //{
                    //    var isNoAvailHours = _db.CloudLabGroups.Where(x => x.CloudLabsGroupID == userGroup && x.SubscriptionRemainingHourCredits != 0).Select(q => new { q.SubscriptionRemainingHourCredits }).FirstOrDefault();
                    //    //var isNoAvailHours = _db.VEProfileLabCreditMappings.Where(x => x.VEProfileID == veprofileid.VEProfileID && x.GroupID == userGroup).Select(q => new { q.TotalRemainingCourseHours }).FirstOrDefault();

                    //    if (isNoAvailHours == null)
                    //        return 0;
                    //    else if (isNoAvailHours.SubscriptionRemainingHourCredits == 0)
                    //        return 0;//return req.CreateResponse(HttpStatusCode.OK, 0, JsonMediaTypeFormatter.DefaultMediaType);
                    //    else
                    //        return 1;//return req.CreateResponse(HttpStatusCode.OK, 1, JsonMediaTypeFormatter.DefaultMediaType);
                    //}
                    //else
                    //    return 0;
                }
                return 0;
                //var virtualEnvironmentID = _db.VirtualEnvironments.Where(x => x.Title == courseCode).Select(q => q.VirtualEnvironmentID).FirstOrDefault();

                //var veProfileId = _db.VEProfiles.Where(x => x.VirtualEnvironmentID == virtualEnvironmentID).Select(q => q.VEProfileID).FirstOrDefault();

                //var userId = _db.CloudLabUsers.Where(x => x.Email == email).Select(q => q.UserId).FirstOrDefault();

                //var userGroup = _db.CloudLabUsers.Where(x => x.Email == email && x.UserId == userId).Select(q => q.UserGroup).FirstOrDefault();

                //if (_db.VirtualMachineMappings.Where(x => x.UserID == userId && x.VEProfileID == veProfileId).Select(f => f.VEProfileID).Count() > 0)
                //{

                //    var isNoAvailHours = _db.VEProfileLabCreditMappings.Where(x => x.VEProfileID == veProfileId && x.GroupID == userGroup).Select(q => new { q.TotalRemainingCourseHours }).FirstOrDefault();

                //    if (isNoAvailHours == null)
                //        return 0;
                //    else if (isNoAvailHours.TotalRemainingCourseHours == 0)
                //        return 0;//return req.CreateResponse(HttpStatusCode.OK, 0, JsonMediaTypeFormatter.DefaultMediaType);
                //    else
                //        return 1;//return req.CreateResponse(HttpStatusCode.OK, 1, JsonMediaTypeFormatter.DefaultMediaType);
                //}
                //else
                //    return 0;//return req.CreateResponse(HttpStatusCode.OK, 0, JsonMediaTypeFormatter.DefaultMediaType);

            }
            catch (Exception e)
            {
                return 0;
//                return req.CreateResponse(HttpStatusCode.OK, e.Message, JsonMediaTypeFormatter.DefaultMediaType);

            }
        }
    }
}
