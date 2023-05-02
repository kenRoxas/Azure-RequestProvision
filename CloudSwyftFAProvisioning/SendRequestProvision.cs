using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using FAProvisioning.Helper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FAProvisioning
{
    public static class SendRequestProvision
    {
        [FunctionName("SendRequestProvision")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            string email = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "email", true) == 0)
                .Value;
            string course_name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "course_name", true) == 0)
                .Value;
            string org = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "org", true) == 0)
                .Value;
            string username = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "username", true) == 0)
                .Value;
            string course_code = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "course_code", true) == 0)
                .Value;

            MailInfo mailInfo = new MailInfo();
            mailInfo.SendTo = "support@cloudswyft.com";
            string htmlBody = string.Empty;

            htmlBody = "Username: " + username + "</br>"
                +"Email: " + email + "</br>"
                + "Course Code: " + course_code + "</br>"
                + "Course Name: " + course_name + "</br>"
                + "Date of Request: " + DateTime.UtcNow;


            mailInfo.Subject = "Request for Hands on Labs";

            mailInfo.HtmlBody = htmlBody;

            MailHelper.SendMail(mailInfo);

            return req.CreateResponse(HttpStatusCode.OK, 1, JsonMediaTypeFormatter.DefaultMediaType);
        }

    }
}
