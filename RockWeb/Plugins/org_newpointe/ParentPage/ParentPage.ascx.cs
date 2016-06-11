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
    [TextField("SMS Message Text", "Default text for the SMS",false,"")]


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



            // TODO: Add people with "Allow Check-in By" relationships too



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

            lbFamilyTitle.Text = "Adult to Text";
            lbNameToText.Text = AdultToTextName;
            lbFamilyToText.Text = AdultToTextFamily;
            lbNumberToText.Text = AdultToTextNumber;


            // TODO: Get the number of the selected adult, then send them a text.

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