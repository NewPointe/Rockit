using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Attribute;
using Rock.Communication;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;


namespace RockWeb.Plugins.org_newpointe.ParentPage
{

    /// <summary>
    /// Block to pick a person and get their URL encoded key.
    /// </summary>
    [DisplayName("Parent Page")]
    [Category("NewPointe Check-In")]
    [Description("Parent Page Block.")]
    [IntegerField("Days Back to Search","Select how many days back to search",true,2)]
    [TextField("Default SMS Message Text", "Default text for the SMS",false,"Test Parent Page Message")]
    [DefinedValueField("611BDE1F-7405-4D16-8626-CCFEDB0E62BE", "Default SMS From Value","Configure in Defined Values",true,false)]


    // TODO: How do we limit by only certain check-in groups (kids and students)?
    // TODO: Save search query douring check-in


    public partial class ParentPage : Rock.Web.UI.RockBlock
    {
        //public Guid typeGuid;
        RockContext rockContext = new RockContext();

        public string Code;
        public string SelectedPersonName;
        public string SelectedPersonFamily;
        public string SelectedPersonCampus;
        public string AdultToTextName;
        public string AdultToTextFamily;
        public string AdultToTextNumber;

        

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void FindPerson(object sender, EventArgs e)
        {
            //Get the code from the Text Box
            Code = rtbCode.Text;

            // Get days back to search
            int daysBacktoSearch = Int32.Parse(GetAttributeValue("DaysBacktoSearch"));
            var searchDate = DateTime.Now.AddDays(-daysBacktoSearch);

            // Create Needed Services
            PersonAliasService personAliasService = new PersonAliasService(rockContext);
            AttendanceCodeService attendanceCodeService = new AttendanceCodeService(rockContext);
            AttendanceService attendanceService = new AttendanceService(rockContext);

            // Make our custom attendance list object
            var resultList = new List<PersonList>();


            // Get results for searched code
            var attendanceObject =
                attendanceCodeService.Queryable()
                    .AsNoTracking()
                    .OrderByDescending(a => a.IssueDateTime)
                    .Where(a => a.Code == Code && a.IssueDateTime >= searchDate)
                    .Select(a => a.Attendances);


            foreach (var x in attendanceObject)
            {
                foreach (var y in x)
                {

                    int? paid = y.PersonAliasId;


                    var thePerson = personAliasService.Queryable().AsNoTracking()
                        .Where(a => a.AliasPersonId == paid)
                        .Select(a => a.Person)
                        .FirstOrDefault();


                    PersonList personList = new PersonList();

                    personList.FullName = thePerson.FullName;
                    personList.Age = thePerson.Age.ToString();
                    personList.Campus = y.Campus.Name.ToString();
                    personList.Device = y.Device.Name.ToString();
                    personList.Gender = thePerson.Gender.ToString();
                    personList.GradeFormatted = thePerson.GradeFormatted;
                    personList.Group = y.Group.Name.ToString();
                    personList.Time = y.StartDateTime.ToShortTimeString();
                    personList.Date = y.StartDateTime.ToShortDateString();
                    personList.Id = thePerson.Id;
                    personList.PersonAliasId = y.PersonAliasId.GetValueOrDefault();


                    resultList.Add(personList);

                }

            }


            gPeople.DataSource = resultList;
            gPeople.DataBind();

            pnlResults.Visible = true;


            lbFamilyTitle.Text = "";
            lbNameToText.Text = "";
            lbFamilyToText.Text = "";
            lbNumberToText.Text = "";
            lbName.Text = "";
            lbFamily.Text = "";
            lbCampus.Text = "";
            lbTitle.Text = "";


        }



        protected void gPeople_RowSelected(object sender, RowEventArgs e)
        {
            int personId = (int)e.RowKeyValues["Id"];
            //int personAliasId = (int)e.RowKeyValues["PersonAliasId"];

            pnlResults.Visible = false;
            pnlNumbers.Visible = true;
            
            PersonService personService = new PersonService(rockContext);

            var resultList = new List<FamilyList>();

            var selectedPerson = personService.Queryable().Where(a => a.Id == personId).FirstOrDefault();

            var family = selectedPerson.GetFamilyMembers(false, rockContext);

            foreach (var x in family)
            {
                if (x.GroupRole.Name == "Adult")
                {
                    FamilyList familyList = new FamilyList();

                    familyList.FullName = x.Person.FullName;
                    familyList.Id = x.PersonId;
                    familyList.FamilyName = x.Group.Name;

                    var phone = x.Person.PhoneNumbers.AsQueryable().Where(a => a.NumberTypeValueId == 12);
                    familyList.PhoneNumber = phone.FirstOrDefault().ToString();
                    
                    resultList.Add(familyList);
                }

                
            }

            
            GroupService groupService = new GroupService(rockContext);
            GroupMemberService groupMemberService = new GroupMemberService(rockContext);

            var groupMemberList = new List<GroupMember>();

            var relationshipGroupMembers = groupMemberService.GetByPersonId(personId).AsQueryable().Where(a => a.GroupRoleId == 9).Select(a => a.Group); ;
            var relationshipGroups = relationshipGroupMembers.AsQueryable().Where(a => a.GroupTypeId == 11).Select(a => a.Members);


            foreach (var b in relationshipGroups)
            {
                foreach (var c in b)
                {
                    if (c.GroupRoleId == 8)
                    {
                        groupMemberList.Add(c);
                    }
                }
            }

            // TODO: Grabbing the wrong person 

            var personRelationships = groupMemberList.AsQueryable()
                .Select(a => a.Person);

            Debug.WriteLine(personRelationships.ToJson());

            foreach (var y in personRelationships)
            {

                FamilyList familyList = new FamilyList();

                familyList.FullName = y.FullName;
                familyList.Id = y.Id;
                familyList.FamilyName = y.GetFamilies().AsQueryable().Select(a => a.Name).FirstOrDefault();

                var phone = y.PhoneNumbers.AsQueryable().Where(a => a.NumberTypeValueId == 12);
                var firstOrDefault = phone.FirstOrDefault();
                if (firstOrDefault != null) familyList.PhoneNumber = firstOrDefault.ToString();

                resultList.Add(familyList);


                }





                SelectedPersonName = selectedPerson.FullName;
            SelectedPersonCampus = selectedPerson.GetCampus().Name;
            SelectedPersonFamily = selectedPerson.GetFamilies().FirstOrDefault().Name.ToString();

            lbName.Text = SelectedPersonName;
            lbFamily.Text = SelectedPersonFamily;
            lbCampus.Text = SelectedPersonCampus;
            lbTitle.Text = "Selected Person";




            //Populate the grid with family members
            gFamily.DataSource = resultList;
            gFamily.DataBind();

            // this remaining stuff prevents .NET from quietly throwing ThreadAbortException
            Context.ApplicationInstance.CompleteRequest();
            return;
        }



        protected void gFamily_RowSelected(object sender, RowEventArgs e)
        {
            int personId = (int) e.RowKeyValues["Id"];


            pnlMessage.Visible = true;
            pnlNumbers.Visible = false;


            PersonService personService = new PersonService(rockContext);
            PersonAliasService personAliasService = new PersonAliasService(rockContext);

            var personToText = personService.Queryable().Where(a => a.Id == personId).FirstOrDefault();

            AdultToTextName = personToText.FullName;
            AdultToTextFamily = personToText.GetFamilies().AsQueryable().FirstOrDefault().Name.ToString();

            var phone = personToText.PhoneNumbers.AsQueryable().Where(a => a.NumberTypeValueId == 12).Select(a => a.Number);
            var firstOrDefault = phone.FirstOrDefault();
            if (firstOrDefault != null) AdultToTextNumber = firstOrDefault.ToString();

            lbFamilyTitle.Text = "Adult to Text";
            lbNameToText.Text = AdultToTextName;
            lbFamilyToText.Text = AdultToTextFamily;
            lbNumberToText.Text = AdultToTextNumber;

            rtMessage.Text = GetAttributeValue("DefaultSMSMessageText");

            // TODO: Show error if no cell phone number

        }


        protected void lbSend_Click(object sender, EventArgs e)
        {
            string message = rtMessage.Text;
            SendSMS(lbNumberToText.Text, message, "622");
        }

        protected void SendSMS(string smsNumber, string message, string fromNumber)
        {
            var recipients = new List<RecipientData>();
            var recipient = new RecipientData(smsNumber);
            recipients.Add(recipient);

            if (recipients.Any() && !string.IsNullOrWhiteSpace(message))
            {
                var mediumEntity = EntityTypeCache.Read(Rock.SystemGuid.EntityType.COMMUNICATION_MEDIUM_SMS.AsGuid(),
                    rockContext);
                if (mediumEntity != null)
                {
                    var medium = MediumContainer.GetComponent(mediumEntity.Name);
                    if (medium != null && medium.IsActive)
                    {
                        var transport = medium.Transport;
                        if (transport != null && transport.IsActive)
                        {
                            var appRoot = GlobalAttributesCache.Read(rockContext).GetValue("InternalApplicationRoot");

                            foreach (var x in recipients)
                            {
                                var mediumData = new Dictionary<string, string>();
                                mediumData.Add("FromValue", fromNumber);
                                mediumData.Add("Message", message);

                                var number = new List<string> {x.To};

                                transport.Send(mediumData, number, appRoot, string.Empty);
                            }
                        }
                    }
                }
            }
        }


    }
}

class PersonList
{
    public string FullName { get; set; }
    public string GradeFormatted { get; set; }
    public string Age { get; set; }
    public string Gender { get; set; }
    public string Date { get; set; }
    public string Campus { get; set; }
    public string Device { get; set; }
    public string Group { get; set; }
    public string Time { get; set; }
    public int Id { get; set; }
    public int PersonAliasId { get; set; }
}

class FamilyList
{
    public string FullName { get; set; }
    public int Id { get; set; }
    public string PhoneNumber { get; set; }
    public string FamilyName { get; set; }
}