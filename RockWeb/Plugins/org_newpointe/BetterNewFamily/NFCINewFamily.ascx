<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NFCINewFamily.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.BetterNewFamily.NFCINewFamily" %>

<asp:UpdatePanel ID="upAddGroup" runat="server">
    <ContentTemplate>
        
        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title">
                    <i class="fa fa-plus-square-o"></i><asp:Literal ID="lTitle" runat="server"></asp:Literal>
                </h1>
            </div>
            <div class="panel-body">

                <asp:ValidationSummary ID="valSummaryTop" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />

                <asp:Panel runat="server" ID="pnlNewFamily">
                    <h2>Parent Info</h2>
                    <div class="row">
                        <div class="col-md-6"><Rock:RockTextBox runat="server" ID="rtbParentFirstName" Label="First Name" Required="true" RequiredErrorMessage="Parent's First Name is Required"></Rock:RockTextBox></div>
                        <div class="col-md-6"><Rock:RockTextBox runat="server" ID="rtbParentLastName" Label="Last Name" Required="true" RequiredErrorMessage="Parent's Last Name is Required"></Rock:RockTextBox></div>
                    </div>
                    <div class="row">
                        <div class="col-md-6"><Rock:PhoneNumberBox runat="server" ID="pnbParentPhoneNumber" Label="Parent Phone Number" Required="true" RequiredErrorMessage="Parent's Phone Number is Required"></Rock:PhoneNumberBox></div>
                        <div class="col-md-6"><Rock:CampusPicker runat="server" ID="cpCampus" Label="Campus" Required="true" RequiredErrorMessage="Campus is Required"></Rock:CampusPicker></div>
                    </div>
    
                    <h2>Children</h2>
                    <table class="table table-groupmembers">
                        <thead>
                            <tr>
                                <th class="required">First Name</th>
                                <th class="required">Last Name</th>
                                <th class="required">Gender</th>
                                <th>Birthdate</th>
                                <th class="required">Grade</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="rKids" OnItemDataBound="rKids_ItemDataBound" OnItemCommand="rKids_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><Rock:RockTextBox runat="server" ID="rtpKidFirstName" Required="true" RequiredErrorMessage="First Name is Required for all Children"></Rock:RockTextBox></td>
                                        <td><Rock:RockTextBox runat="server" ID="rtpKidLastName" Required="true" RequiredErrorMessage="Last Name is Required for all Children"></Rock:RockTextBox></td>
                                        <td>
                                            <Rock:RockRadioButtonList runat="server" ID="rblGender" Required="true" RequiredErrorMessage="Gender is Required for all Children">
                                                <asp:ListItem Text="Male" Value="1" />
                                                <asp:ListItem Text="Female" Value="2" />
                                            </Rock:RockRadioButtonList>
                                        </td>
                                        <td><Rock:DatePicker runat="server" ID="dpBirthdate"></Rock:DatePicker></td>
                                        <td><Rock:GradePicker runat="server" ID="gpGrade" Required="true" Label="" RequiredErrorMessage="Grade is Required for all Children"></Rock:GradePicker></td>
                                        <td><asp:LinkButton ID="lbDelete" runat="server" Text='<i class="fa fa-times"></i>' CssClass="btn btn-danger" CommandName="Delete" CausesValidation="false"></asp:LinkButton></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan="5"></td>
                                <td><asp:LinkButton runat="server" ID="lbAddKid" Text='<i class="fa fa-plus"></i>' CssClass="btn btn-success" OnClick="lbAddKid_Click" CausesValidation="false" /></td>
                            </tr>
                        </tfoot>
                    </table>
                </asp:Panel>

                <asp:Panel runat="server" ID="pnlExtra" Visible="false">
                
                <h2>Extra Info</h2>
                    <table class="table table-groupmembers">
                        <thead>
                            <tr>
                                <th>Child</th>
                                <th>Ability Level</th>
                                <th>Allergy</th>
                                <th>Legal Notes</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="rExtras" OnItemDataBound="rExtras_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td><Rock:RockLiteral runat="server" ID="rlKidName"></Rock:RockLiteral></td>
                                        <td><Rock:DefinedValuePicker runat="server" ID="dvpAbilityLevel"></Rock:DefinedValuePicker></td>
                                        <td><Rock:RockTextBox runat="server" ID="rtbAllergy"></Rock:RockTextBox></td>
                                        <td><Rock:RockTextBox runat="server" ID="rtbLegalNotes"></Rock:RockTextBox></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                
                </asp:Panel>

                <div class="actions">
                    <asp:LinkButton runat="server" ID="lbBack" CssClass="btn btn-default" Text="Back" OnClick="lbBack_Click"></asp:LinkButton>
                    <asp:LinkButton runat="server" ID="lbNext" CssClass="btn btn-primary" Text="Next" OnClick="lbNext_Click"></asp:LinkButton>
                    <asp:LinkButton runat="server" ID="lbSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="lbSubmit_Click"></asp:LinkButton>
                </div>
            
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
