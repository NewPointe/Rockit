using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_newpointe.BetterNewFamily
{
    /// <summary>
    /// Block for adding new families
    /// </summary>
    [DisplayName( "NFCI New Family" )]
    [Category( "NewPointe Check-in" )]
    [Description( "Allows for quickly adding a new Family" )]

    [LinkedPage( "Person Details Page", "The page to use to show person details.", false, "", "", 0 )]
    public partial class NFCINewFamily : Rock.Web.UI.RockBlock
    {
        List<KidData> kidsList = new List<KidData>();
        int _abilityLevelDefinedTypeId;

        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {

                ShowPage( 1 );

                //kidsList.Add( new KidData { FirstName = "", LastName = "", Gender = Gender.Unknown } );
                BindControls();

                var contextCampus = RockPage.GetCurrentContext( EntityTypeCache.Read( "Rock.Model.Campus" ) ) as Campus;
                if ( contextCampus != null )
                {
                    cpCampus.SelectedCampusId = contextCampus.Id;
                }
            }
            else
            {
                loadKids();
            }
        }

        protected void BindControls()
        {
            _abilityLevelDefinedTypeId = DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.PERSON_ABILITY_LEVEL_TYPE.AsGuid() ).Id;

            var camp = cpCampus.SelectedCampusId;
            cpCampus.Campuses = CampusCache.All( false );
            cpCampus.SelectedCampusId = camp;

            rKids.DataSource = kidsList;
            rKids.DataBind();
            rExtras.DataSource = kidsList;
            rExtras.DataBind();

        }

        private class KidData
        {
            public String FirstName { get; set; }
            public String LastName { get; set; }
            public Gender Gender { get; set; }
            public DateTime? Birthdate { get; set; }
            public DefinedValueCache Grade { get; set; }
            public DefinedValueCache AbilityLevel { get; set; }
            public String Allergy { get; set; }
            public String LegalNotes { get; set; }
        }

        protected void loadKids()
        {
            kidsList.Clear();
            foreach ( RepeaterItem Item in rKids.Items )
            {
                RockTextBox firstName = Item.FindControl( "rtpKidFirstName" ) as RockTextBox;
                RockTextBox lastName = Item.FindControl( "rtpKidLastName" ) as RockTextBox;
                RockRadioButtonList gender = Item.FindControl( "rblGender" ) as RockRadioButtonList;
                DatePicker birthdate = Item.FindControl( "dpBirthdate" ) as DatePicker;
                GradePicker grade = Item.FindControl( "gpGrade" ) as GradePicker;

                var kidData = new KidData
                {
                    FirstName = firstName.Text,
                    LastName = lastName.Text,
                    Gender = gender.SelectedValueAsEnum<Gender>( Gender.Unknown ),
                    Birthdate = birthdate.SelectedDate,
                    Grade = grade.SelectedGradeValue
                };

                if ( rExtras.Items.Count > Item.ItemIndex )
                {
                    RepeaterItem extraItem = rExtras.Items[Item.ItemIndex];
                    DefinedValuePicker ability = extraItem.FindControl( "dvpAbilityLevel" ) as DefinedValuePicker;
                    RockTextBox allergy = extraItem.FindControl( "rtbAllergy" ) as RockTextBox;
                    RockTextBox legal = extraItem.FindControl( "rtbLegalNotes" ) as RockTextBox;

                    kidData.AbilityLevel = DefinedValueCache.Read( ability.SelectedValueAsId() ?? -1 );
                    kidData.Allergy = allergy.Text;
                    kidData.LegalNotes = legal.Text;
                }

                kidsList.Add( kidData );
            }
        }

        protected void rKids_ItemDataBound( object sender, RepeaterItemEventArgs e )
        {
            RepeaterItem Item = e.Item;
            if ( Item.ItemType == ListItemType.Item || Item.ItemType == ListItemType.AlternatingItem )
            {
                var kid = e.Item.DataItem as KidData;

                RockTextBox firstName = Item.FindControl( "rtpKidFirstName" ) as RockTextBox;
                RockTextBox lastName = Item.FindControl( "rtpKidLastName" ) as RockTextBox;
                RockRadioButtonList gender = Item.FindControl( "rblGender" ) as RockRadioButtonList;
                DatePicker birthdate = Item.FindControl( "dpBirthdate" ) as DatePicker;
                GradePicker grade = Item.FindControl( "gpGrade" ) as GradePicker;

                firstName.Text = kid.FirstName;
                lastName.Text = kid.LastName;
                gender.SelectedValue = kid.Gender.ConvertToInt().ToString();
                birthdate.SelectedDate = kid.Birthdate;
                grade.SelectedGradeValue = kid.Grade;
            }
        }

        protected void rExtras_ItemDataBound( object sender, RepeaterItemEventArgs e )
        {
            RepeaterItem Item = e.Item;
            if ( Item.ItemType == ListItemType.Item || Item.ItemType == ListItemType.AlternatingItem )
            {
                var kid = e.Item.DataItem as KidData;

                RockLiteral name = Item.FindControl( "rlKidName" ) as RockLiteral;
                DefinedValuePicker ability = Item.FindControl( "dvpAbilityLevel" ) as DefinedValuePicker;
                RockTextBox allergy = Item.FindControl( "rtbAllergy" ) as RockTextBox;
                RockTextBox legal = Item.FindControl( "rtbLegalNotes" ) as RockTextBox;

                name.Text = kid.FirstName + ' ' + kid.LastName;
                ability.DefinedTypeId = _abilityLevelDefinedTypeId;
                ability.SelectedValue = kid.AbilityLevel != null ? kid.AbilityLevel.Id.ToString() : "";
                allergy.Text = kid.Allergy;
                legal.Text = kid.LegalNotes;
            }
        }


        protected void lbAddKid_Click( object sender, EventArgs e )
        {
            kidsList.Add( new KidData { FirstName = "", LastName = rtbParentLastName.Text, Gender = Gender.Unknown } );
            BindControls();
        }

        protected void rKids_ItemCommand( object source, RepeaterCommandEventArgs e )
        {
            if ( e.CommandName == "Delete" )
            {
                kidsList.RemoveAt( e.Item.ItemIndex );
                BindControls();
            }
        }

        protected void ShowPage( int pageNumber )
        {
            pnlNewFamily.Visible = pageNumber == 1;
            pnlExtra.Visible = pageNumber != 1;
            lbBack.Visible = pageNumber != 1;
            lbNext.Visible = pageNumber == 1;
            lbSubmit.Visible = pageNumber != 1;
        }

        protected void lbBack_Click( object sender, EventArgs e )
        {
            ShowPage( 1 );
        }

        protected void lbNext_Click( object sender, EventArgs e )
        {
            if ( Page.IsValid )
            {
                ShowPage( 2 );
                BindControls();
            }
        }

        protected void lbSubmit_Click( object sender, EventArgs e )
        {

            if ( Page.IsValid )
            {
                RockContext rockContext = new RockContext();

                var adultRole = new GroupTypeRoleService( rockContext ).Get( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() );
                var childRole = new GroupTypeRoleService( rockContext ).Get( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() );

                Person parent = new Person();
                parent.FirstName = rtbParentFirstName.Text;
                parent.LastName = rtbParentLastName.Text;

                parent.LoadAttributes( rockContext );

                PhoneNumber parentPhone = new PhoneNumber();
                parentPhone.Number = pnbParentPhoneNumber.Number;

                parent.PhoneNumbers.Add( parentPhone );

                List<GroupMember> familyMembers = new List<GroupMember>();

                GroupMember parentGM = new GroupMember();
                parentGM.Person = parent;
                parentGM.GroupRoleId = adultRole.Id;

                familyMembers.Add( parentGM );

                foreach ( KidData kidData in kidsList )
                {
                    Person kid = new Person();
                    kid.FirstName = kidData.FirstName;
                    kid.LastName = kidData.LastName;
                    kid.Gender = kidData.Gender;
                    kid.SetBirthDate( kidData.Birthdate );
                    kid.GradeOffset = kidData.Grade.Value.AsIntegerOrNull();

                    kid.LoadAttributes( rockContext );
                    kid.SetAttributeValue( "AbilityLevel", kidData.AbilityLevel.Guid.ToString() );
                    kid.SetAttributeValue( "Allergy", kidData.Allergy );
                    kid.SetAttributeValue( "LegalNotes", kidData.LegalNotes );

                    GroupMember kidGM = new GroupMember();
                    kidGM.Person = kid;
                    kidGM.GroupRoleId = childRole.Id;

                    familyMembers.Add( kidGM );
                }

                Group family = GroupService.SaveNewFamily( rockContext, familyMembers, cpCampus.SelectedValueAsInt(), true );

                if ( !String.IsNullOrWhiteSpace( GetAttributeValue( "PersonDetailsPage" ) ) )
                {
                    NavigateToLinkedPage( "PersonDetailsPage", new Dictionary<string, string>() { { "PersonId", parent.Id.ToString() } } );
                }
                else
                {
                    NavigateToCurrentPage( new Dictionary<string, string>() { { "PersonId", parent.Id.ToString() } } );
                }

            }

        }

    }
}