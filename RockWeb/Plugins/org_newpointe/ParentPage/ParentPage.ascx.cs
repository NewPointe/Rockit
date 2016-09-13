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
    [DisplayName( "Parent Page" )]
    [Category( "NewPointe > Checkin" )]
    [Description( "Parent Page Block." )]

    [IntegerField( "Days Back to Search", "Select how many days back to search", true, 1, "", 0 )]
    [CustomCheckboxListField( "Included Relationships", "The relationships to include.", "SELECT Name AS Text, Guid AS Value FROM GroupTypeRole WHERE GroupTypeId = 11 OR GroupTypeId = 10", true, ""
        + Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT + ","
        + Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_ALLOW_CHECK_IN_BY + ","
        + Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_GRANDPARENT + ","
        + Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_INVITED_BY + ","
        + Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_PARENT + ","
        + Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_STEP_PARENT
        + "", "", 1 )]
    [WorkflowTypeField( "Workflow Type", "The workflow to launch.", false, false, "", "", 2 )]

    // TODO: How do we limit by only certain check-in groups (kids and students)?
    // TODO: Save search query douring check-in


    public partial class ParentPage : Rock.Web.UI.RockBlock
    {

        RockContext rockContext = new RockContext();

        protected string CheckinCode
        {
            get { return ViewState["CheckinCode"] as string; }
            set { ViewState["CheckinCode"] = value; }
        }
        protected int? SelectedPersonAliasId
        {
            get { return ViewState["SelectedPersonAliasId"] as int?; }
            set { ViewState["SelectedPersonAliasId"] = value; }
        }

        protected override void OnLoad( EventArgs e )
        {
            if ( !IsPostBack )
            {
                CheckinCode = PageParameter( "CheckinCode" );
                if ( !String.IsNullOrWhiteSpace( CheckinCode ) )
                    bindGrid();
            }
        }

        protected void bindGrid()
        {
            if ( !String.IsNullOrWhiteSpace( CheckinCode ) )
            {
                pnlCheckinCode.Visible = false;
                pnlSearchedCheckinCode.Visible = true;
                rlCheckinCode.Text = CheckinCode + " <a href='?'><i class='fa fa-times'></i></a>";

                if ( SelectedPersonAliasId.HasValue )
                {

                    Person SelectedPerson = new PersonAliasService( rockContext ).Get( SelectedPersonAliasId.Value ).Person;
                    pnlSelectedPerson.Visible = true;
                    pnlRelationSearch.Visible = true;
                    pnlCodeSearch.Visible = false;
                    rlSelectedPerson.Text = SelectedPerson.FullName + " <a href='?CheckinCode=" + CheckinCode + "'><i class='fa fa-times'></i></a>";

                    // Adult Family Members
                    var familyMembers = SelectedPerson.GetFamilyMembers( false, rockContext ).Select( m => new PersonRelationship { Person = m.Person, Role = m.GroupRole, Priority = 100 } ).ToList();

                    // Known Relationships
                    var knownRelationship_GroupMemberships = new GroupMemberService( rockContext ).Queryable().Where( gm => gm.Group.GroupTypeId == 11 && gm.PersonId == SelectedPerson.Id );

                    var ownedRelationshipGroups = knownRelationship_GroupMemberships.Where( gm => gm.GroupRole.Guid.ToString() == Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_OWNER ).Select( gm => gm.Group );
                    var thirdPartyReletionshipGroupMemberShips = knownRelationship_GroupMemberships.Where( gm => gm.GroupRole.Guid.ToString() != Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_OWNER );

                    // For Owned relationships, we need the reverse of the 3rd party's role
                    var ownedRelationship_GroupMembers = ownedRelationshipGroups.SelectMany( g => g.Members.Where( gm => gm.PersonId != SelectedPerson.Id ) );

                    var ownedRelationships = ownedRelationship_GroupMembers.ToList().Select( gm => new PersonRelationship
                    {
                        Person = gm.Person,
                        Role = gm.GroupRole,
                        Priority = 50
                    } );

                    var GroupRoleServ = new GroupTypeRoleService( rockContext );
                    var ownedRelationshipsList = new List<PersonRelationship>();
                    foreach ( var relationship in ownedRelationships )
                    {
                        relationship.Role.LoadAttributes();
                        relationship.Role = GroupRoleServ.Get( relationship.Role.GetAttributeValue( "InverseRelationship" ).AsGuid() );
                        ownedRelationshipsList.Add( relationship );
                    }

                    // For 3rd party relationships, we need our role
                    var thirdPartyRelationships = thirdPartyReletionshipGroupMemberShips.Select( gm => new PersonRelationship
                    {
                        Person = gm.Group.Members.Where( gm2 => gm2.GroupRole.Guid.ToString() == Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_OWNER ).FirstOrDefault().Person,
                        Role = gm.GroupRole,
                        Priority = 40
                    } ).ToList();



                    var relationships = familyMembers.Union( ownedRelationshipsList ).Union( thirdPartyRelationships );

                    var shownRelationships = GetAttributeValue( "IncludedRelationships" ).Split( ',' ).Select( g => Guid.Parse( g ) );
                    relationships = relationships.Where( r => shownRelationships.Contains( r.Role.Guid ) );

                    var rels = relationships.GroupBy( x => new
                    {
                        PersonId = x.Person.Id,
                        Person = x.Person,
                        Roles = String.Join( ", ", relationships.Where( r => r.Person.Id == x.Person.Id ).Select( r => r.Role.Name ) ),
                        Priority = x.Priority,
                        HomePhone = x.Person.PhoneNumbers.Where( p => p.NumberTypeValueId == 12 ).Select( p => p.NumberFormatted + ( p.IsMessagingEnabled ? " &nbsp;<i class=\"fa fa-comments\"></i>" : "" ) ).FirstOrDefault(),
                        MobilePhone = x.Person.PhoneNumbers.Where( p => p.NumberTypeValueId == 13 ).Select( p => p.NumberFormatted + ( p.IsMessagingEnabled ? " &nbsp;<i class=\"fa fa-comments\"></i>" : "" ) ).FirstOrDefault()
                    } ).Select( g => g.Key );

                    gReleventPeople.DataSource = rels.OrderByDescending( r => r.Priority );
                    gReleventPeople.DataKeyNames = new string[] { "PersonId" };
                    gReleventPeople.DataBind();
                }
                else
                {
                    pnlSelectedPerson.Visible = false;
                    pnlRelationSearch.Visible = false;
                    pnlCodeSearch.Visible = true;

                    int daysBacktoSearch = GetAttributeValue( "DaysBacktoSearch" ).AsInteger();
                    var searchDate = DateTime.Now.Date.AddDays( -daysBacktoSearch );
                    gSearchResults.SetLinqDataSource( new AttendanceCodeService( rockContext ).Queryable().Where( c => c.Code == CheckinCode && c.IssueDateTime > searchDate ).SelectMany( c => c.Attendances ).OrderByDescending( "StartDateTime" ) );
                    gSearchResults.DataKeyNames = new string[] { "PersonAliasId" };
                    gSearchResults.DataBind();
                }
            }
        }

        protected string FormatCheckedIntoString( string grp, string loc )
        {
            return loc.Equals( grp, StringComparison.OrdinalIgnoreCase ) ? grp : grp + " at " + loc;
        }


        protected void rbbSearch_Click( object sender, EventArgs e )
        {
            CheckinCode = rtbCheckinCode.Text.ToUpper();
            bindGrid();
        }

        protected void gSearchResults_RowSelected( object sender, RowEventArgs e )
        {
            SelectedPersonAliasId = e.RowKeyId;
            bindGrid();
        }

        protected void gReleventPeople_RowSelected( object sender, RowEventArgs e )
        {
            int pagePersonId = e.RowKeyId;
            Person pagePerson = new PersonService( rockContext ).Get( pagePersonId );

            if(pagePerson != null)
            {
                var workflowType = new WorkflowTypeService( rockContext ).Get( GetAttributeValue( "WorkflowType" ).AsGuid() );
                if (workflowType != null)
                {
                    var workflow = Rock.Model.Workflow.Activate( workflowType, null, rockContext );
                    List<string> errorMessages;
                    new Rock.Model.WorkflowService( rockContext ).Process( workflow, out errorMessages );
                }
            }
        }

        /*
        //public Guid typeGuid;

        public string SelectedPersonName;
        public string SelectedPersonFamily;
        public string SelectedPersonCampus;
        public string AdultToTextName;
        public string AdultToTextFamily;
        public string AdultToTextNumber;

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

    */
    }
}


class PersonRelationship
{
    public Person Person { get; set; }
    public GroupTypeRole Role { get; set; }
    public int Priority { get; set; }
}