using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock;
using Rock.Security;

namespace org.newpointe.ProtectMyMinistry
{
    [Description("Better Protect My Ministry Background Check")]
    [Export(typeof(BackgroundCheckComponent))]
    [ExportMetadata("ComponentName", "Protect My Ministry Plus")]

    [TextField("User Name", "Protect My Ministry User Name", true, "", "", 0)]
    [EncryptedTextField("Password", "Protect My Ministry Password", true, "", "", 1, null, true)]
    [BooleanField("Test Mode", "Should requests be sent in 'test' mode?", false, "", 2)]
    [TextField("Request URL", "The Protect My Ministry URL to send requests to.", true, "https://services.priorityresearch.com/webservice/default.cfm", "", 3)]
    [TextField("Return URL", "The Web Hook URL for Protect My Ministry to send results to (e.g. 'http://www.mysite.com/Webhooks/ProtectMyMinistryPlus.ashx').", true, "", "", 4)]
    public class ProtectMyMinistryPlus : BackgroundCheckComponent
    {
        private HttpStatusCode _HTTPStatusCode;

        public override bool SendRequest(RockContext rockContext, Workflow workflow, AttributeCache personAttribute, AttributeCache ssnAttribute, AttributeCache requestTypeAttribute, AttributeCache billingCodeAttribute, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            rockContext.SaveChanges();

            // Check to make sure workflow is not null
            if (workflow == null || workflow.Id == 0)
            {
                errorMessages.Add("The 'Protect My Ministry' background check provider requires a valid persisted workflow.");
                return false;
            }

            workflow.LoadAttributes();

            // Get the person that the request is for
            Person person = null;
            if (personAttribute != null)
            {
                Guid? personAliasGuid = workflow.GetAttributeValue(personAttribute.Key).AsGuidOrNull();
                if (personAliasGuid.HasValue)
                {
                    person = new PersonAliasService(rockContext).Queryable()
                        .Where(p => p.Guid.Equals(personAliasGuid.Value))
                        .Select(p => p.Person)
                        .FirstOrDefault();
                    person.LoadAttributes(rockContext);
                }
            }

            if (person == null)
            {
                errorMessages.Add("The 'Protect My Ministry' background check provider requires the workflow to have a 'Person' attribute that contains the person who the background check is for.");
                return false;
            }

            string billingCode = workflow.GetAttributeValue(billingCodeAttribute.Key);
            Guid? campusGuid = billingCode.AsGuidOrNull();
            if (campusGuid.HasValue)
            {
                var campus = CampusCache.Read(campusGuid.Value);
                if (campus != null)
                {
                    billingCode = campus.Name;
                }
            }
            string ssn = Encryption.DecryptString(workflow.GetAttributeValue(ssnAttribute.Key));
            DefinedValueCache requestType = DefinedValueCache.Read(workflow.GetAttributeValue(requestTypeAttribute.Key).AsGuid());

            if (requestType == null)
            {
                errorMessages.Add("Unknown request type.");
                return false;
            }

            int orderId = workflow.Id;

            string requestTypePackageName = requestType.GetAttributeValue("PMMPackageName").Trim();

            BackgroundCheck bgCheck = getBgCheck(rockContext, workflow, person.PrimaryAliasId.Value);

            List<string[]> SSNTraceCounties = null;
            if (requestTypePackageName.Equals("PLUS", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrWhiteSpace(bgCheck.ResponseXml))
            {
                IEnumerable<XElement> transactions = XDocument.Parse(bgCheck.ResponseXml).Root.Elements("Transaction");
                IEnumerable<XElement> OrderXMLs = transactions.SelectMany(x => x.Elements("OrderXML"));
                IEnumerable<XElement> Orders = OrderXMLs.Select(x => x.Element("Order")).Where(x => x != null);
                IEnumerable<XElement> OrderDetails = Orders.SelectMany(x => x.Elements("OrderDetail"));
                IEnumerable<XElement> SSNTraces = OrderDetails.Where(x => x.Attribute("ServiceCode")?.Value == "SSNTrace" && x.Element("Status") != null);
                IEnumerable<XElement> SSNTraceResults = SSNTraces.Select(x => x.Element("Result"));
                IEnumerable<XElement> SSNTraceIndividuals = SSNTraceResults.SelectMany( x => x.Elements( "Individual" ) );
                IEnumerable<XElement> SSNTraceIndividualsToSearch = SSNTraceIndividuals.Where( x => x.Element( "EndDate" ) == null || x.Element( "EndDate" ).Element( "Year" ) == null || ( x.Element( "EndDate" ).Element( "Year" ).Value.AsInteger() > ( DateTime.Now.Year - 8 ) ) );
                SSNTraceCounties = SSNTraceIndividualsToSearch.Where(x => x.Element( "County" ) != null && x.Element( "State" ) != null).Select(x => new string[] { x.Element("County").Value, x.Element("State").Value }).ToList();
            }

            XElement xTransaction = makeBGRequest(bgCheck, ssn, requestType, billingCode, SSNTraceCounties);
            saveTransaction(rockContext, bgCheck, xTransaction);
            handleTransaction(rockContext, bgCheck, xTransaction);
            return true;

            //catch (Exception ex)
            //{
            //    ExceptionLogService.LogException(ex, null);
            //    errorMessages.Add(ex.Message);
            //    return false;
            //}
        }

        private static BackgroundCheck getBgCheck(RockContext rockContext, Workflow workflow, int personAliasId)
        {
            var backgroundCheckService = new BackgroundCheckService(rockContext);
            var backgroundCheck = backgroundCheckService.Queryable()
                .Where(c =>
                   c.WorkflowId.HasValue &&
                   c.WorkflowId.Value == workflow.Id)
                .FirstOrDefault();

            if (backgroundCheck == null)
            {
                backgroundCheck = new Rock.Model.BackgroundCheck();
                backgroundCheck.PersonAliasId = personAliasId;
                backgroundCheck.WorkflowId = workflow.Id;
                backgroundCheck.RequestDate = RockDateTime.Now;
                backgroundCheckService.Add(backgroundCheck);
                rockContext.SaveChanges();
            }

            return backgroundCheck;
        }

        private XElement makeBGRequest(BackgroundCheck bgCheck, string ssn, DefinedValueCache requestType, string billingCode, List<string[]> ssnTraceCounties)
        {

            XDocument xRequestDoc = BuildRequest(
                bgCheck.WorkflowId.ToString(),
                bgCheck.PersonAlias.Person,
                ssn,
                requestType,
                billingCode,
                ssnTraceCounties
            );

            var requestDateTime = RockDateTime.Now;

            XDocument xResultDoc = PostToWebService(xRequestDoc, GetAttributeValue("RequestURL"));
            var responseDateTime = RockDateTime.Now;

            var xTransaction = new XElement("Transaction",
                                  new XAttribute("TransactionType", "REQUEST"),
                                  new XAttribute("RequestDateTime", requestDateTime),
                                  new XAttribute("ResponseDateTime", responseDateTime),
                                  xRequestDoc.Root,
                                  xResultDoc.Root
                              );

            return xTransaction;
        }

        public static XDocument saveTransaction(RockContext rockContext, BackgroundCheck bgCheck, XElement xTransaction)
        {
            XDocument xTransactionLog;

            // Clear any SSN nodes before saving XML to record
            foreach (var xSSNElement in xTransaction.Descendants("SSN"))
            {
                xSSNElement.Value = "XXX-XX-XXXX";
            }

            if (!String.IsNullOrWhiteSpace(bgCheck.ResponseXml))
            {
                xTransactionLog = XDocument.Parse(bgCheck.ResponseXml);
            }
            else
            {
                xTransactionLog = new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement("BgChkTransactions"));
            }

            xTransactionLog.Root.Add(xTransaction);
            bgCheck.ResponseXml = xTransactionLog.ToString();
            rockContext.SaveChanges();

            return xTransactionLog;
        }

        public static void handleTransaction(RockContext rockContext, BackgroundCheck bgCheck, XElement xTransaction)
        {
            bool createdNewAttribute = false;
            // Handle transaction status
            XElement xOrderResponse = null;
            if (xTransaction.Attribute("TransactionType").Value == "REQUEST")
            {
                xOrderResponse = xTransaction.Elements().Last();

                XElement xResponseStatus = xOrderResponse.Element("Status");
                if (xResponseStatus != null)
                {
                    string status = xResponseStatus.Value;
                    createdNewAttribute = SaveAttributeValue(bgCheck.Workflow, "RequestStatus", status, FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), rockContext, null) || createdNewAttribute;

                    if (status == "ERROR")
                    {
                        createdNewAttribute = SaveAttributeValue(bgCheck.Workflow, "RequestMessage", xOrderResponse.Elements("Message").Select(x => x.Value).ToList().AsDelimited(Environment.NewLine), FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), rockContext, null) || createdNewAttribute;
                    }
                }

                XElement xResponseErrors = xOrderResponse.Elements("Errors").FirstOrDefault();
                if (xResponseErrors != null)
                {
                    createdNewAttribute = SaveAttributeValue(bgCheck.Workflow, "RequestMessage", xResponseErrors.Elements("Message").Select(x => x.Value).ToList().AsDelimited(Environment.NewLine), FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), rockContext, null) || createdNewAttribute;
                }
            }
            else if (xTransaction.Attribute("TransactionType").Value == "RESPONSE")
            {
                xOrderResponse = xTransaction.Elements().First();
            }

            // Handle request status
            if (xOrderResponse != null)
            {
                XElement xOrder = xOrderResponse.Element("Order");

                if (xOrder != null && xOrder.Elements("OrderDetail").Any(x => x.Elements("Status").Any()))
                {
                    SaveResults(rockContext, bgCheck, xOrderResponse);
                }
            }
        }

        public static void SaveResults(RockContext rockContext, BackgroundCheck bgCheck, XElement xResult)
        {
            bool createdNewAttribute = false;

            if (xResult != null)
            {
                var xOrder = xResult.Elements("Order").FirstOrDefault();
                if (xOrder != null)
                {
                    bool resultFound = false;

                    // Find any order details with a status element
                    string reportStatus = "Pass";
                    foreach (var xOrderDetail in xOrder.Elements("OrderDetail"))
                    {
                        var goodStatus = (xOrderDetail.Attribute("ServiceCode")?.Value == "SSNTrace") ? "COMPLETE" : "NO RECORD";

                        var xStatus = xOrderDetail.Elements("Status").FirstOrDefault();
                        if (xStatus != null)
                        {
                            resultFound = true;
                            if (xStatus.Value != goodStatus)
                            {
                                reportStatus = "Review";
                                break;
                            }
                        }
                    }

                    if (resultFound)
                    {
                        // If no records found, still double-check for any alerts
                        if (reportStatus != "Review")
                        {
                            var xAlerts = xOrder.Elements("Alerts").FirstOrDefault();
                            if (xAlerts != null)
                            {
                                if (xAlerts.Elements("OrderId").Any())
                                {
                                    reportStatus = "Review";
                                }
                            }
                        }

                        // Save the recommendation 
                        string recommendation = (from o in xOrder.Elements("Recommendation") select o.Value).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(recommendation))
                        {
                            if (SaveAttributeValue(bgCheck.Workflow, "ReportRecommendation", recommendation,
                                FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), rockContext,
                                new Dictionary<string, string> { { "ispassword", "false" } }))
                            {
                                createdNewAttribute = true;
                            }

                        }

                        // Save the report link 
                        Guid? binaryFileGuid = null;
                        string reportLink = (from o in xOrder.Elements("ReportLink") select o.Value).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(reportLink))
                        {
                            if (SaveAttributeValue(bgCheck.Workflow, "ReportLink", reportLink,
                                FieldTypeCache.Read(Rock.SystemGuid.FieldType.URL_LINK.AsGuid()), rockContext))
                            {
                                createdNewAttribute = true;
                            }

                            // Save the report
                            binaryFileGuid = SaveFile(bgCheck.Workflow.Attributes["Report"], reportLink, bgCheck.Workflow.Id.ToString() + ".pdf");
                            if (binaryFileGuid.HasValue)
                            {
                                if (SaveAttributeValue(bgCheck.Workflow, "Report", binaryFileGuid.Value.ToString(),
                                    FieldTypeCache.Read(Rock.SystemGuid.FieldType.BINARY_FILE.AsGuid()), rockContext,
                                    new Dictionary<string, string> { { "binaryFileType", "" } }))
                                {
                                    createdNewAttribute = true;
                                }
                            }
                        }

                        // Save the status
                        if (SaveAttributeValue(bgCheck.Workflow, "ReportStatus", reportStatus,
                            FieldTypeCache.Read(Rock.SystemGuid.FieldType.SINGLE_SELECT.AsGuid()), rockContext,
                            new Dictionary<string, string> { { "fieldtype", "ddl" }, { "values", "Pass,Fail,Review" } }))
                        {
                            createdNewAttribute = true;
                        }

                        // Update the background check file
                        if (bgCheck != null)
                        {
                            bgCheck.ResponseDate = RockDateTime.Now;
                            bgCheck.RecordFound = reportStatus == "Review";

                            if (binaryFileGuid.HasValue)
                            {
                                var binaryFile = new BinaryFileService(rockContext).Get(binaryFileGuid.Value);
                                if (binaryFile != null)
                                {
                                    bgCheck.ResponseDocumentId = binaryFile.Id;
                                }
                            }
                        }
                    }
                }
            }

            rockContext.SaveChanges();

            if (createdNewAttribute)
            {
                AttributeCache.FlushEntityAttributes();
            }
        }

        public XDocument BuildRequest(string orderId, Person person, string ssn, DefinedValueCache requestType, string billingCode, List<String[]> counties = null)
        {
            XElement rootElement = new XElement("OrderXML",
                    new XElement("Method", "SEND ORDER"),
                    new XElement("Authentication",
                        new XElement("Username", GetAttributeValue("UserName")),
                        new XElement("Password", Encryption.DecryptString(GetAttributeValue("Password")))
                    )
                );

            if (GetAttributeValue("TestMode").AsBoolean())
            {
                rootElement.Add(new XElement("TestMode", "YES"));
            }

            rootElement.Add(new XElement("ReturnResultURL", GetAttributeValue("ReturnURL")));

            XElement orderElement = new XElement("Order");
            rootElement.Add(orderElement);

            if (!String.IsNullOrWhiteSpace(billingCode))
            {
                orderElement.Add(new XElement("BillingReferenceCode", billingCode));
            }
            XElement subjectElement = new XElement("Subject",
                   new XElement("FirstName", person.FirstName),
                   new XElement("MiddleName", person.MiddleName),
                   new XElement("LastName", person.LastName)
               );
            orderElement.Add(subjectElement);

            if (person.SuffixValue != null)
            {
                subjectElement.Add(new XElement("Generation", person.SuffixValue.Value));
            }
            if (person.BirthDate.HasValue)
            {
                subjectElement.Add(new XElement("DOB", person.BirthDate.Value.ToString("MM/dd/yyyy")));
            }

            ssn = ssn.AsNumeric();
            if (!String.IsNullOrWhiteSpace(ssn) && ssn.Length == 9)
            {
                subjectElement.Add(new XElement("SSN", ssn.Insert(5, "-").Insert(3, "-")));
            }

            if (person.Gender == Gender.Male)
            {
                subjectElement.Add(new XElement("Gender", "Male"));
            }
            if (person.Gender == Gender.Female)
            {
                subjectElement.Add(new XElement("Gender", "Female"));
            }

            string dlNumber = person.GetAttributeValue("com.sparkdevnetwork.DLNumber");
            if (!string.IsNullOrWhiteSpace(dlNumber))
            {
                subjectElement.Add(new XElement("DLNumber", dlNumber));
            }

            var homelocation = person.GetHomeLocation();

            if (homelocation != null)
            {
            
                var addressStatesDefinedValues = DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.LOCATION_ADDRESS_STATE.AsGuid() ).DefinedValues;
                var mappedStateId = addressStatesDefinedValues.Where( x => x.Description.ToLower() == homelocation.State.ToLower() ).Select(x => x.Value).FirstOrDefault();
                if(mappedStateId != null)
                {
                    homelocation.State = mappedStateId;
                }

                subjectElement.Add(new XElement("CurrentAddress",
                    new XElement("StreetAddress", homelocation.Street1),
                    new XElement("City", homelocation.City),
                    new XElement("State", homelocation.State),
                    new XElement("Zipcode", homelocation.PostalCode)
                ));
            }

            XElement aliasesElement = new XElement("Aliases");
            if (person.NickName != person.FirstName)
            {
                aliasesElement.Add(new XElement("Alias", new XElement("FirstName", person.NickName)));
            }

            foreach (var previousName in person.GetPreviousNames())
            {
                aliasesElement.Add(new XElement("Alias", new XElement("LastName", previousName.LastName)));
            }

            if (aliasesElement.HasElements)
            {
                subjectElement.Add(aliasesElement);
            }

            string packageName = requestType.GetAttributeValue("PMMPackageName");

            string county = requestType.GetAttributeValue("DefaultCounty");
            string state = requestType.GetAttributeValue("DefaultState");
            string mvrJurisdiction = GetMVRJurisdiction(requestType.GetAttributeValue("MVRJurisdiction"));
            string mvrState = mvrJurisdiction.Left(2);

            if (homelocation != null)
            {
                if (String.IsNullOrWhiteSpace(homelocation.County) &&
                    requestType.GetAttributeValue("SendHomeCounty").AsBoolean())
                {
                    county = homelocation.County;
                }
                if (!String.IsNullOrWhiteSpace(homelocation.State))
                {
                    if (requestType.GetAttributeValue("SendHomeState").AsBoolean())
                    {
                        state = homelocation.State;
                    }
                    if (requestType.GetAttributeValue("SendHomeStateMVR").AsBoolean())
                    {
                        mvrState = homelocation.State;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(packageName) && !packageName.Trim().Equals("SSNTRACE", StringComparison.OrdinalIgnoreCase))
            {
                orderElement.Add(new XElement("PackageServiceCode", requestType, new XAttribute("OrderId", orderId)));
            }

            if (packageName.Trim().Equals("BASIC", StringComparison.OrdinalIgnoreCase) ||
                packageName.Trim().Equals("PLUS", StringComparison.OrdinalIgnoreCase))
            {
                orderElement.Add(new XElement("OrderDetail",
                    new XAttribute("OrderId", orderId),
                    new XAttribute("ServiceCode", "combo")));
            }

            if (packageName.Trim().Equals("SSNTRACE", StringComparison.OrdinalIgnoreCase))
            {
                orderElement.Add(new XElement("OrderDetail",
                    new XAttribute("OrderId", orderId),
                    new XAttribute("ServiceCode", "SSNTrace")));
            }

            if (counties == null)
            {
                counties = new List<String[]>();
            }
            counties.Add(new string[] { county, state });
            counties = counties.Where(x => !x.Any(y => y == null)).Select(x => x.Select(y => y.ToLower()).ToArray()).Distinct(new StrArEquals()).ToList();
            foreach (var location in counties)
            {
                if (!string.IsNullOrWhiteSpace(location[0]) ||
                        !string.IsNullOrWhiteSpace(location[1]))
                {
                    orderElement.Add(new XElement("OrderDetail",
                        new XAttribute("OrderId", orderId),
                        new XAttribute("ServiceCode", string.IsNullOrWhiteSpace(location[0]) ? "StateCriminal" : "CountyCrim"),
                        new XElement("County", location[0]),
                        new XElement("State", location[1]),
                        new XElement("YearsToSearch", 7),
                        new XElement("CourtDocsRequested", "NO"),
                        new XElement("RushRequested", "NO"),
                        new XElement("SpecialInstructions", ""))
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(mvrJurisdiction) && !string.IsNullOrWhiteSpace(mvrState))
            {
                orderElement.Add(new XElement("OrderDetail",
                    new XAttribute("OrderId", orderId),
                    new XAttribute("ServiceCode", "MVR"),
                    new XElement("JurisdictionCode", mvrJurisdiction),
                    new XElement("State", mvrState))
                );
            }

            return new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), rootElement);
        }

        #region Helper Methods

        private XDocument PostToWebService(XDocument data, string requestUrl)
        {
            string stringData = "REQUEST=" + data.Declaration.ToString() + data.ToString(SaveOptions.DisableFormatting);
            byte[] postData = ASCIIEncoding.ASCII.GetBytes(stringData);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.ContentLength = postData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Close();

            //try
            //{
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return GetResponse(response.GetResponseStream(), response.ContentType, response.StatusCode);
            //}
            //catch (WebException webException)
            //{
            //    string message = GetResponseMessage(webException.Response.GetResponseStream());
            //    throw new Exception(webException.Message + " - " + message);
            //}
        }

        private XDocument GetResponse(Stream responseStream, string contentType, HttpStatusCode statusCode)
        {
            _HTTPStatusCode = statusCode;

            Stream receiveStream = responseStream;
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);

            StringBuilder sb = new StringBuilder();
            Char[] read = new Char[8192];
            int count = 0;
            do
            {
                count = readStream.Read(read, 0, 8192);
                String str = new String(read, 0, count);
                sb.Append(str);
            }
            while (count > 0);

            string HTMLResponse = sb.ToString();

            if (HTMLResponse.Trim().Length > 0 && HTMLResponse.Contains("<?xml"))
                return XDocument.Parse(HTMLResponse);
            else
                return null;
        }

        private static string GetResponseMessage(Stream responseStream)
        {
            Stream receiveStream = responseStream;
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);

            StringBuilder sb = new StringBuilder();
            Char[] read = new Char[8192];
            int count = 0;
            do
            {
                count = readStream.Read(read, 0, 8192);
                String str = new String(read, 0, count);
                sb.Append(str);
            }
            while (count > 0);

            return sb.ToString();
        }

        private static string GetMVRJurisdiction(string MVRJurisdictionGuid)
        {
            Guid? mvrJurisdictionGuid = MVRJurisdictionGuid.AsGuidOrNull();
            if (mvrJurisdictionGuid.HasValue)
            {
                var mvrJurisdictionDv = DefinedValueCache.Read(mvrJurisdictionGuid.Value);
                if (mvrJurisdictionDv != null)
                {
                    return mvrJurisdictionDv.Value;
                }
            }
            return String.Empty;
        }

        private static bool SaveAttributeValue(Rock.Model.Workflow workflow, string key, string value,
            FieldTypeCache fieldType, RockContext rockContext, Dictionary<string, string> qualifiers = null)
        {
            bool createdNewAttribute = false;

            if (workflow.Attributes.ContainsKey(key))
            {
                workflow.SetAttributeValue(key, value);
            }
            else
            {
                // Read the attribute
                var attributeService = new AttributeService(rockContext);
                var attribute = attributeService
                    .Get(workflow.TypeId, "WorkflowTypeId", workflow.WorkflowTypeId.ToString())
                    .Where(a => a.Key == key)
                    .FirstOrDefault();

                // If workflow attribute doesn't exist, create it 
                // ( should only happen first time a background check is processed for given workflow type)
                if (attribute == null)
                {
                    attribute = new Rock.Model.Attribute();
                    attribute.EntityTypeId = workflow.TypeId;
                    attribute.EntityTypeQualifierColumn = "WorkflowTypeId";
                    attribute.EntityTypeQualifierValue = workflow.WorkflowTypeId.ToString();
                    attribute.Name = key.SplitCase();
                    attribute.Key = key;
                    attribute.FieldTypeId = fieldType.Id;
                    attributeService.Add(attribute);

                    if (qualifiers != null)
                    {
                        foreach (var keyVal in qualifiers)
                        {
                            var qualifier = new Rock.Model.AttributeQualifier();
                            qualifier.Key = keyVal.Key;
                            qualifier.Value = keyVal.Value;
                            attribute.AttributeQualifiers.Add(qualifier);
                        }
                    }

                    createdNewAttribute = true;
                }

                // Set the value for this action's instance to the current time
                var attributeValue = new Rock.Model.AttributeValue();
                attributeValue.Attribute = attribute;
                attributeValue.EntityId = workflow.Id;
                attributeValue.Value = value;
                new AttributeValueService(rockContext).Add(attributeValue);
            }

            return createdNewAttribute;
        }

        private static Guid? SaveFile(AttributeCache binaryFileAttribute, string url, string fileName)
        {
            // get BinaryFileType info
            if (binaryFileAttribute != null &&
                binaryFileAttribute.QualifierValues != null &&
                binaryFileAttribute.QualifierValues.ContainsKey("binaryFileType"))
            {
                Guid? fileTypeGuid = binaryFileAttribute.QualifierValues["binaryFileType"].Value.AsGuidOrNull();
                if (fileTypeGuid.HasValue)
                {
                    RockContext rockContext = new RockContext();
                    BinaryFileType binaryFileType = new BinaryFileTypeService(rockContext).Get(fileTypeGuid.Value);

                    if (binaryFileType != null)
                    {
                        byte[] data = null;

                        using (WebClient wc = new WebClient())
                        {
                            data = wc.DownloadData(url);
                        }

                        BinaryFile binaryFile = new BinaryFile();
                        binaryFile.Guid = Guid.NewGuid();
                        binaryFile.IsTemporary = true;
                        binaryFile.BinaryFileTypeId = binaryFileType.Id;
                        binaryFile.MimeType = "application/pdf";
                        binaryFile.FileName = fileName;
                        binaryFile.ContentStream = new MemoryStream(data);

                        var binaryFileService = new BinaryFileService(rockContext);
                        binaryFileService.Add(binaryFile);

                        rockContext.SaveChanges();

                        return binaryFile.Guid;
                    }
                }
            }

            return null;
        }
        #endregion
    }

    public class StrArEquals : IEqualityComparer<String[]>
    {
        public bool Equals(String[] left, String[] right)
        {
            if ((object)left == null && (object)right == null)
            {
                return true;
            }
            if ((object)left == null || (object)right == null)
            {
                return false;
            }
            return String.Join(":", left) == String.Join(":", right);
        }

        public int GetHashCode(String[] str)
        {
            return String.Join(":", str).GetHashCode();
        }
    }
}
