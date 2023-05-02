using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FAProvisioning.DBContext;
using FAProvisioning.Helper;
using FAProvisioning.Models;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace FAProvisioning
{
    public static class MachineModule
    {
        private static DBCloudLabsContext _db = new DBCloudLabsContext();
        private static DBTenantContext _dbTenants = new DBTenantContext();
        private static string ProdID = ConfigurationManager.AppSettings["ProdID"];// "d2fd4772-79f4-476c-ac6d-9dc11365db22";
        private static string machineUsername = ConfigurationManager.AppSettings["MachineUserName"]; // "cloudswyft";
        private static string machinePassword = ConfigurationManager.AppSettings["MachinePassword"]; // "Pr0v3byd01n6";
        private static IVirtualMachine vm;
        private static VirtualMachineSizeTypes machineSize = VirtualMachineSizeTypes.StandardF4sV2;
        private static string clientID = ConfigurationManager.AppSettings["ClientID"];
        private static string clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
        private static string tenantID = ConfigurationManager.AppSettings["TenantID"];
        
        [FunctionName("LaasCourseProvision")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                
                string courseCode = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "CourseCode", true) == 0)
                    .Value;
                string email = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "email", true) == 0)
                    .Value;


                var azureCredentials = new AzureCredentialsFactory().FromServicePrincipal( clientID, clientSecret, tenantID, AzureEnvironment.AzureGlobalCloud);

                IAzure _azure = Azure.Authenticate(azureCredentials).WithSubscription(ProdID);
                var resourceGroup = "CS-AIZEL";
                var machineName = "BAM-Sample";
                var VEProfileId = 255;
                var imageName = "https://csvmlistdisk.blob.core.windows.net/vhds/ubuntu20200527101913.vhd";
                //var groupPrefix = _db.CloudLabUsers.Join(_db.CloudLabGroups, a => a.UserGroup, b => b.CloudLabsGroupID, (a, b) => new { a, b }).Where(q => q.a.Email == email).Select(x => new { x.b.CLPrefix }).FirstOrDefault().CLPrefix.ToUpper();
                //var resourceGroup = "CS-KEN-"+ groupPrefix;
                //var machineName = MachineNameGenerator(groupPrefix);
                //var imageName = _db.VirtualEnvironments.Join(_db.VirtualEnvironmentImages, a => a.VirtualEnvironmentID, b => b.VirtualEnvironmentID, (a, b) => new { a, b }).Where(x => x.a.Description == courseCode).FirstOrDefault().b.Name;

                //var courseInfo = _db.VEProfiles.Join(_db.VirtualEnvironments, a => a.VirtualEnvironmentID, b => b.VirtualEnvironmentID, (a, b) => new { a, b }).Where(x => x.b.Description.ToUpper() == courseCode.ToUpper())
                //    .Join(_db.VEProfileLabCreditMappings, c => c.a.VEProfileID, d => d.VEProfileID, (c, d) => new { c, d }).Select(y => new { VEProfileId = y.d.VEProfileID, CourseHours = y.d.CourseHours }).SingleOrDefault();

                //var userInfo = _db.CloudLabUsers.Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();

                //var guacDetails = _dbTenants.Tenants.Where(x => x.TenantId == userInfo.TenantId).Select(y => new { GuacConnection = y.GuacConnection, GuacamoleURL = y.GuacamoleURL }).FirstOrDefault();
                
                var t1 = DateTime.Now;

                var region = Region.AsiaSouthEast;

                var vm = _azure.Networks.GetByResourceGroup(resourceGroup, resourceGroup + "-vnet");

                //addMappings(machineName, userInfo.UserId, VEProfileId);

                //SendEmail(email);
                var subnet = string.Empty;
                var lastSub = vm.Subnets.LastOrDefault();

                subnet = lastSub.Value.Name;

                //if (lastSub.Value.NetworkInterfaceIPConfigurationCount == 240)
                //{
                //    var cidr = lastSub.Value.AddressPrefix.Split('.');
                //    if (Convert.ToInt32(cidr[2]) == 240)
                //    {
                //        cidr[3] = (Convert.ToInt32(cidr[3]) + 1).ToString();
                //    }
                //    else
                //    {
                //        cidr[2] = (Convert.ToInt32(cidr[2]) + 1).ToString();
                //    }
                //    var addPrefix = string.Join(".", cidr);

                //    var name = GenerateRandomSubnetName();

                //    vm.Update().WithAddressSpace(addPrefix).Apply();
                //    vm.Update().DefineSubnet(name).WithAddressPrefix(addPrefix).Attach().Apply();
                //}
                //else
                //{
                //    subnet = lastSub.Value.Name;
                //}

                var publicIpAddress = _azure.PublicIPAddresses.Define($"IP-{machineName}")
                   .WithRegion(region)
                   .WithExistingResourceGroup(resourceGroup).WithDynamicIP()
                   .WithLeafDomainLabel(machineName.ToLower())
                  .Create();

                var ipAdd = publicIpAddress.IPAddress;

                var networkInterface = _azure.NetworkInterfaces.Define($"{machineName}")
                    .WithRegion(region)
                    .WithExistingResourceGroup(resourceGroup)
                    .WithExistingPrimaryNetwork(vm)
                    .WithSubnet(subnet)
                    .WithPrimaryPrivateIPAddressDynamic()
                    .WithExistingPrimaryPublicIPAddress(publicIpAddress)
                    .Create();

                IVirtualMachine createVm = _azure.VirtualMachines.Define(machineName)
                    .WithRegion(region)
                    .WithExistingResourceGroup(resourceGroup)
                    .WithExistingPrimaryNetworkInterface(networkInterface)
                    .WithStoredLinuxImage(imageName)
                    .WithRootUsername(machineUsername)
                    .WithRootPassword(machinePassword)
                    //.WithExistingUnmanagedDataDisk("csvmlistdisk", "vhds", "ubuntu-cm-datadisk-u1.vhd")
                    .WithSize(machineSize)
                    .Create();

                    //.WithComputerName(machineName)
                    //.WithSize(machineSize)
                    
                    //.Create();

                //var newVirtualMachine = new VirtualMachineOperations
                //{
                //    ServiceName = resourceGroup + "-vnet",
                //    RoleName = machineName,
                //    Port = "0",
                //    //GuacamoleInstances = guacamoleInstance,
                //    CourseID = 1,
                //    UserID = userInfo.UserId,
                //    VEProfileID = VEProfileId,
                //    LastTimeStamp = DateTime.UtcNow,
                //    DateStartedTrigger = DateTime.UtcNow,
                //    DateCreated = DateTime.UtcNow.Date,
                //    IsStarted = 1,
                //    MachineInstance = 1,
                //    LabHoursPerCourse = 10,
                //    GroupID = Convert.ToInt32(userInfo.UserGroup)
                //};
                //var dataVirtualMachine = JsonConvert.SerializeObject(newVirtualMachine);

                //AddMachineToDatabase(machineName, machineName, ipAdd, newVirtualMachine, guacDetails.GuacConnection, guacDetails.GuacamoleURL, userInfo.UserId);

                //var urlContentList = new VirtualMachineLogs()
                //{
                //    RoleName = newVirtualMachine.RoleName,
                //    TimeStamp = DateTime.Now + "----CREATE;",
                //    UserID = newVirtualMachine.UserID,
                //    VEProfileID = newVirtualMachine.VEProfileID,
                //    Comment = "Created",
                //};

                //var Schedule = new CloudLabsSchedules()
                //{
                //    VEProfileID = newVirtualMachine.VEProfileID,
                //    UserId = newVirtualMachine.UserID,
                //    DateCreated = DateTime.UtcNow.Date,
                //    ScheduledBy = "",
                //    LabHoursRemaining = newVirtualMachine.LabHoursPerCourse,
                //    LabHoursTotal = newVirtualMachine.LabHoursPerCourse,
                //    StartLabTriggerTime = Convert.ToDateTime("1/1/1753 12:00:00 AM"),
                //    RenderPageTriggerTime = Convert.ToDateTime("1/1/1753 12:00:00 AM")
                //};

                //var groupID = newVirtualMachine.GroupID;

                //var VEProfileID = newVirtualMachine.VEProfileID;
                

                //addLogs(userInfo.UserId, VEProfileId, "CREATED");
                //addSchedules(Schedule);
                //editMappings(userInfo.UserId, VEProfileId, true);
                //deductLabHours(VEProfileID, groupID);

                //SendSuccessEmail(email);
                //ShutdownMachine(userInfo.UserId, courseInfo.VEProfileId, userInfo.UserGroup);

                return req.CreateResponse(HttpStatusCode.OK, "OK");

            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
        }
        public static string MachineNameGenerator(string groupCode)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string result = string.Empty;

            while (true)
            {
                var generatedValues = new string(
                    Enumerable.Repeat(chars, 7)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

                result = "CS-" + groupCode.ToUpper() + "-" + generatedValues;

                if (_db.VirtualMachineMappings.SingleOrDefault(x => x.RoleName == result) == null)
                    break;
            }

            return result;

        }
        public static void addLogs(int UserId, int VEProfileId, string comment)
        {
            var date = DateTime.Now;

            var virtualMachineMappings = _db.VirtualMachineMappings.Where(vmm => vmm.UserID == UserId && vmm.VEProfileID == VEProfileId).FirstOrDefault();
            var virtualMachineLogs = _db.VirtualMachineLogs.Where(x => x.VEProfileID == VEProfileId && x.UserID == UserId).FirstOrDefault();
            var virtualMachine = _db.VirtualMachines.Where(v => v.UserID == UserId && v.VEProfileID == VEProfileId).FirstOrDefault();

            if (virtualMachineLogs == null)
            {
                virtualMachineLogs = new VirtualMachineLogs()
                {
                    TimeStamp = date + "----" + comment.ToUpper() + ';',
                    RoleName = virtualMachineMappings.RoleName,
                    VirtualMachineID = virtualMachine.VirtualMachineID,
                    Comment = comment,
                    UserID = UserId,
                    VEProfileID = VEProfileId
                };
            }

            _db.VirtualMachineLogs.Add(virtualMachineLogs);
            _db.SaveChanges();
        }
        public static void addSchedules(CloudLabsSchedules data)
        {
            _db.CloudLabsSchedules.Add(data);
            _db.SaveChanges();
        }
        public static void addMappings(string machineName, int userId, int veProfileId)
        {
            var vmMapping = new VirtualMachineMappings
            {
                RoleName = machineName,
                UserID = userId,
                VEProfileID = veProfileId,
                CourseID = 1,
                MachineInstance = 0,
                IsProvisioned = 1,
                IsLaasProvisioned = 0
            };
            _db.VirtualMachineMappings.Add(vmMapping);
            _db.SaveChanges();            
        }

        public static void editMappings(int UserId, int VEProfileId, bool isLaas)
        {
            var vmm = _db.VirtualMachineMappings.Where(x => x.UserID == UserId && x.VEProfileID == VEProfileId).FirstOrDefault();
            if (isLaas)
                vmm.IsLaasProvisioned = 1;
            else
                vmm.IsProvisioned = 1;

            _db.Entry(vmm).State = EntityState.Modified;
            _db.SaveChanges();
        }
        public static string deductLabHours(int VEProfileID, int GroupID)
        {
            VEProfileLabCreditMappings record = _db.VEProfileLabCreditMappings.Where(x => x.VEProfileID == VEProfileID && x.GroupID == GroupID).FirstOrDefault();
            record.TotalRemainingCourseHours -= record.CourseHours;
            _db.Entry(record).State = EntityState.Modified;
            _db.SaveChanges();
            return "Successfully deduct Lab Hours";
        }
        private static DataOps ApiTenant(int userId)
        {
            var apiUrl = string.Empty;
            var tenantPrefix = string.Empty;
            var tenantName = string.Empty;

            var tenantID = _db.CloudLabUsers.Where(x => x.UserId == userId).Select(q => q.TenantId).FirstOrDefault();
            var tenant = _dbTenants.Tenants.Where(x => x.TenantId == tenantID).FirstOrDefault();

            var dataOps = new DataOps
            {
                ApiUrl = tenant.ApiUrl,
                TenantPrefix = tenant.Code,
                TenantName = tenant.TenantName,
            };

            return dataOps;
        }
        private static void AddMachineToDatabase(string machineName, string roleName, string staticIp, VirtualMachineOperations newVirtualMachine, string guacCon, string guacURL, int UserID)
        {
            var guacamoleInstance = AddGuacamoleInstance(machineName, staticIp, guacCon, guacURL, UserID);

            newVirtualMachine.GuacamoleInstances = guacamoleInstance;

            try
            {
                Thread.Sleep(20000);
                var urlContentList = JsonConvert.SerializeObject(newVirtualMachine);

                VirtualMachines vm = _db.VirtualMachines.Where(x => x.UserID == newVirtualMachine.UserID && x.VEProfileID == newVirtualMachine.VEProfileID).FirstOrDefault();
                GuacamoleInstances gi = _db.GuacamoleInstances.Where(y => y.Connection_Name == machineName).FirstOrDefault();
                if (vm == null)
                {
                    vm = new VirtualMachines();
                    vm.CourseID = newVirtualMachine.CourseID;
                    vm.DateCreated = newVirtualMachine.DateCreated;
                    vm.DateStartedTrigger = newVirtualMachine.DateStartedTrigger;
                    vm.IsStarted = newVirtualMachine.IsStarted;
                    vm.LastTimeStamp = newVirtualMachine.LastTimeStamp;
                    vm.MachineInstance = 1;
                    vm.NetworkID = newVirtualMachine.NetworkID;
                    vm.Port = null;
                    vm.RoleName = newVirtualMachine.RoleName;
                    vm.ServiceName = newVirtualMachine.ServiceName;
                    vm.UserID = newVirtualMachine.UserID;
                    vm.VEProfileID = newVirtualMachine.VEProfileID;
                    vm.VirtualMachineID = newVirtualMachine.VirtualMachineID;
                    vm.VirtualMachineType = null;

                    _db.VirtualMachines.Add(vm);
                    _db.SaveChanges();
                }
                if (gi == null)
                {
                    var guac = guacamoleInstance.FirstOrDefault();
                    gi = new GuacamoleInstances();
                    gi.Connection_Name = guac.Connection_Name;
                    gi.GuacamoleInstanceID = guac.GuacamoleInstanceID;
                    gi.GuacLinkType = guac.GuacLinkType;
                    gi.Hostname = guac.Hostname;
                    gi.Url = guac.Url;
                    gi.VirtualMachineID = vm.VirtualMachineID;

                    _db.GuacamoleInstances.Add(gi);
                    _db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return;
        }
        private static List<GuacamoleInstanceOperations> AddGuacamoleInstance(string machineName, string staticIp, string guacCon, string guacURL, int userId)
        {
            var guacDatabase = new MySqlConnection(guacCon);

            while (guacDatabase.State != ConnectionState.Open)
            {
                try
                {
                    guacDatabase.Open();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            var selectQuery =
                $"SELECT connection_id FROM guacamole_connection WHERE connection_name LIKE '{machineName}%'";
            var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);
            var dataReader = MySqlCommand.ExecuteReader();
            dataReader.Read();
            var guacamoleInstance = new List<GuacamoleInstanceOperations>();
            if (!dataReader.HasRows)
            {
                try
                {
                    dataReader.Close();
                    guacamoleInstance.Add(GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "rdp", 1, guacURL, userId));

                    //switch (protocol)
                    //{
                    //    case "rdp":
                    //        guacamoleInstance = GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "rdp", 1, i);
                    //        //guacamoleInstance.Add(GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "vnc", 2, log));
                    //        break;
                    //    case "ssh":
                    //        guacamoleInstance = GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "ssh", 1, i);
                    //        break;
                    //}

                    guacDatabase.Close();
                }
                catch (Exception e)
                {
                    dataReader.Close();
                }

            }
            else
            {
                dataReader.Close();
            }
            return guacamoleInstance;
        }
        public static GuacamoleInstanceOperations GenerateGuacamoleInstance(string staticIp, string machineName, MySqlConnection guacDatabase, string protocol, int guacType, string guacURL, int userID)
        {
            try
            {
                var hostName = machineName;

                var insertQuery =
                    "INSERT INTO guacamole_connection (connection_name, protocol, max_connections, max_connections_per_user) " +
                    $"VALUES (\'{hostName}-{protocol}\', \'{protocol}\', \'5\', \'0\')";

                var insertCommand = new MySqlCommand(insertQuery, guacDatabase);

                insertCommand.ExecuteNonQuery();

                var selectQuery =
                    $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                var dataReader = MySqlCommand.ExecuteReader();

                dataReader.Read();

                var connectionId = Convert.ToInt32(dataReader["connection_id"]);

                dataReader.Close();

                var guacUrlHostName = guacURL;
                guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                var insertParamsQuery = string.Empty;

                switch (protocol)
                {
                    case "rdp":
                        insertParamsQuery =
                            "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                            $"VALUES ({connectionId}, 'hostname', '{machineName.ToLower()}'" + " '.cloudapp.azure.com'), " +
                            $"({connectionId}, 'ignore-cert', 'true'), " +
                            $"({connectionId}, 'password', '{machinePassword}'), " +
                            $"({connectionId}, 'security', 'nla'), " +
                            $"({connectionId}, 'port', '3389'), " +
                            $"({connectionId}, 'enable-wallpaper', 'true'), " +
                            $"({connectionId}, 'username', '{machineUsername}')";
                        break;
                    case "vnc":
                        insertParamsQuery =
                            "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                            $"VALUES ({connectionId}, 'hostname', '{staticIp}'), " +
                            $"({connectionId}, 'password', '{machinePassword}')";
                        break;
                    case "ssh":
                        insertParamsQuery =
                            "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                            $"VALUES ({connectionId}, 'hostname', '{staticIp}'), " +
                            $"({connectionId}, 'username', '{machineUsername}'), " +
                            $"({connectionId}, 'password', '{machinePassword}'), " +
                           $"({connectionId}, 'port', '22') ";

                        break;
                }

                var insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);

                insertParamsCommand.ExecuteNonQuery();
                var tenant = ApiTenant(userID);

                selectQuery = $"SELECT user_id FROM guacamole_user WHERE username = '{tenant.TenantName}'";
                MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);
                var dataReader2 = MySqlCommand.ExecuteReader();
                dataReader2.Read();
                var userId = Convert.ToInt32(dataReader2["user_id"]);
                dataReader2.Close();

                var insertPermissionQuery = string.Format("INSERT INTO guacamole_connection_permission(user_id, connection_id, permission) VALUES ({1},{0}, 'READ')", connectionId, userId);

                var insertPermissionCommand = new MySqlCommand(insertPermissionQuery, guacDatabase);

                insertPermissionCommand.ExecuteNonQuery();

                var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                var connectionString = Convert.ToBase64String(bytes);

                var guacUrl =
                    $"{guacURL}/guacamole/#/client/{connectionString}?username={tenant.TenantName}&password=pr0v3byd01n6!";


                var guacamoleInstance = new GuacamoleInstanceOperations()
                {
                    Connection_Name = hostName,
                    GuacLinkType = guacType,
                    Hostname = guacUrlHostName,
                    Url = guacUrl
                };


                return guacamoleInstance;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static void SendEmail(string email)
        {
            var emails = new string[2] { email, "kenneth@cloudswyft.com"};
            string htmlBody = string.Empty;
            
            MailInfo mailInfo = new MailInfo();
            mailInfo.Subject = "Provisioning Machine";

            htmlBody = "Your Machine is being Provision, please wait";

            mailInfo.HtmlBody = htmlBody;
            for (int i = 0; emails.Count() > i; i++)
            {
                mailInfo.SendTo = emails[i];
                MailHelper.SendMail(mailInfo);
            }
        }
        public static void SendSuccessEmail(string email)
        {
            var emails = new string[2] { email, "kenneth@cloudswyft.com" };

            string htmlBody = string.Empty;

            MailInfo mailInfo = new MailInfo();
            mailInfo.Subject = "Provisioning Machine";

            htmlBody = "The Machine is Ready";


            mailInfo.HtmlBody = htmlBody;

            for (int i = 0; emails.Count() > i; i++)
            {
                mailInfo.SendTo = emails[i];
                MailHelper.SendMail(mailInfo);
            }
                

        }

    }


}
