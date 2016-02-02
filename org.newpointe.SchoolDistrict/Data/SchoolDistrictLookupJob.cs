using System;
using System.Data.SqlClient;
using System.Linq;

using Quartz;

using Rock.Model;
using Rock.Web.Cache;
using Rock.Attribute;



using System.IO;

using System.Net;
using System.Text;


using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace Rock.Jobs
{
    /// <summary>
    /// Job to keep a heartbeat of the job process so we know when the jobs stop working
    /// </summary>
    [IntegerField("Max Records Per Run", "The maximum number of records to run per run.", true, 1000)]
    [IntegerField("Throttle Period", "The number of milliseconds to wait between records. This helps to throttle requests to the lookup services.", true, 500)]
    [IntegerField("Retry Period", "The number of days to wait before retrying a unsuccessful address lookup.", true, 200)]
    [DisallowConcurrentExecution]
    public class SchoolDistrictLookupJob : IJob
    {

        /// <summary> 
        /// Empty constructor for job initilization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public SchoolDistrictLookupJob()
        {
        }

        /// <summary> 
        /// Job that updates the JobPulse setting with the current date/time.
        /// This will allow us to notify an admin if the jobs stop running.
        /// 
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="ITrigger" /> fires that is associated with
        /// the <see cref="IJob" />.
        /// </summary>
        public virtual void Execute(IJobExecutionContext context)
        {
            // get the job map
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int maxRecords = Int32.Parse(dataMap.GetString("MaxRecordsPerRun"));
            int throttlePeriod = Int32.Parse(dataMap.GetString("ThrottlePeriod"));
            int retryPeriod = Int32.Parse(dataMap.GetString("RetryPeriod"));

            DateTime retryDate = DateTime.Now.Subtract(new TimeSpan(retryPeriod, 0, 0, 0));

            var rockContext = new Rock.Data.RockContext();
            LocationService locationService = new LocationService(rockContext);
            int addressesVerified = 0;
            var addresses = locationService.Queryable()
                                .Where(l => (
                                   (l.IsGeoPointLocked == null || l.IsGeoPointLocked == false) // don't ever try locked address
                                   && (l.IsActive == true && l.Street1 != null && l.PostalCode != null) // or incomplete addresses
                                   && (
                                       (l.GeocodedDateTime == null && (l.GeocodeAttemptedDateTime == null || l.GeocodeAttemptedDateTime < retryDate)) // has not been attempted to be geocoded since retry date
                                       ||
                                       (l.StandardizedDateTime == null && (l.StandardizeAttemptedDateTime == null || l.StandardizeAttemptedDateTime < retryDate)) // has not been attempted to be standardize since retry date
                                   )
                               ))
                                .Take(maxRecords).ToList();

            foreach (var address in addresses)
            {
                //locationService.Verify(address, false); // currently not reverifying 

                //use GetSchoolDistrict method to get a string that contains the School District
                string addressString = address.Street1 + " " + address.City + " " + address.State + " " + address.PostalCode;
                string disctict = GetSchoolDistrict(addressString);
                
                //write the School District to the database
                string addressId = address.Id.ToString();

                rockContext.Database.ExecuteSqlCommand( "UPDATE Location SET SchoolDistrict = '@disctict' WHERE Id = @id", new SqlParameter("district", disctict), new SqlParameter("id", addressId) );

                addressesVerified++;
                rockContext.SaveChanges();
                System.Threading.Thread.Sleep(throttlePeriod);
            }

            context.Result = string.Format("{0} addresses verified", addressesVerified);

        }



        //Get the district from Geocod.io
        public string GetSchoolDistrict(string addressG)
        {

            string theUrl = String.Format("https://api.geocod.io/v1/geocode?q={0}&fields=school&api_key={1}", Uri.EscapeDataString(addressG), "05d88b8e068e656d704558d85d8fd74370e45d7");

            //GET request to API
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(theUrl);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    String responseStr = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(responseStr))
                    {
                        throw new ArgumentException(String.Format("Could not get school district for '{0}'", "URL"));

                    }
                    else
                    {
                        dynamic data = JObject.Parse(responseStr);
                        string schoolDistrict = data.results[0].fields.school_districts.unified.name;
                        return schoolDistrict;
                    }
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();

                }
                throw;
            }
        }


    }
}