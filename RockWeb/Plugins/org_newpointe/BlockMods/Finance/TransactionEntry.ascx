<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TransactionEntry.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.BlockMods.Finance.TransactionEntry" %>

<script type="text/javascript">

    var isSmall = true;

    $(window).resize(function (e) {
        oldIsSmall = isSmall;
        isSmall = $(this).width() < 768;
        if (isSmall != oldIsSmall) updateRollup();
    });

    $(document).on('click', '#ButtonDropDown_btnFrequency .dropdown-menu, #collapse2 .panel-collapse', function (e) {
        if (isSmall) updateRollupLabels();
    });

    $(document).on('click', '#rblSavedAch', updateAchDiv);

    function updateAchDiv() {
        if ($('#rblSavedAch input:checked').val() == -1) {
            $('#divRecentCheck').show();
            $('#divNewBank').hide();
        } else if ($('#rblSavedAch input:checked').val() == 0) {
            $('#divNewBank').show();
            $('#divRecentCheck').hide();
        } else {
            $('#divNewBank').show();
            $('#divRecentCheck').hide();
        }
    }

    function onASPReload() {
        isSmall = $(this).width() < 768;

        // Set initial rollup state
        $('#collapse0 .panel-collapse').addClass(isSmall ? "" : "in");
        $('#collapse1 .panel-collapse').addClass(isSmall && $('#CollapsePersonData').val() === "true" ? "" : "in");
        $('#collapse2 .panel-collapse').addClass(isSmall && $('#CollapseCardData').val() === "true" ? "" : "in");

        // Update rollup state
        updateRollup();

        // Fix for page jumping on ButtonDropDownList selection
        $('.button-drop-down-list a').each(function (i, e) {
            e.removeAttribute("href");
        })

        // Enable number mode for input
        $('.currency-box .account-amount input').each(function () {
            this.type = "number";
        });

        updateAchDiv();
    }

    function updateRollup() {
        var PersonData = $('#CollapsePersonData').val() === "true";
        var SavedCardPresent = $('#CollapseCardData').val() === "true";

        toggleAccordion('collapse0', isSmall);
        toggleAccordion('collapse1', isSmall && PersonData);
        toggleAccordion('collapse2', isSmall && SavedCardPresent);

        $('#divActions2').toggleClass("hidden", !(isSmall && PersonData && SavedCardPresent));

        updateRollupLabels();
    }

    function toggleAccordion(id, hide) {
        $('#' + id + ' .panel-collapse').collapse({ toggle: false }).collapse(hide ? 'hide' : 'show');
        $('#' + id + ' .panel-title *').filter('small, span').toggleClass("hidden", !hide);
    }

    function updateRollupLabels() {
        $('#FrequencyText').html((tmp = $('#hfSelectedItemText_btnFrequency').val()) != "" ? tmp : getButtonDropDownListVal('btnFrequency'));
        if ((el = $($("#collapse2 .nav-pills .active a")[0].hash + " .radio-list input[type=radio]:checked")[0]) != undefined) {
            $('#SavedCardName').html(el.parentElement.textContent);
        }
    }

    function getButtonDropDownListVal(id) {
        val = $('#hfSelectedItemId_' + id).val()
        names = $('#ButtonDropDown_' + id + ' .dropdown-menu a').map( function(){
            return [$(this).data('id') + "", $(this).html()];
        }).get();
        return names[names.indexOf(val) + 1];
    }

</script>
<style>
    #fundadd .button-drop-down-list,
    #fundadd .button-drop-down-list .controls
    {
        display: inline-block;
    }
    .button-drop-down-list a{
        cursor: pointer;
    }
    .panel-default .panel-heading {
        color: #333333;
        background-color: #eeeeee;
        border-bottom: 1px solid #d5d5d5;
    }
    .panel-group a + .panel-collapse > .panel-body {
        border-top: 0;
    }
    #thefunds fieldset hr:last-of-type {
        display: inherit;
        border-color: lightgray;
    }
    #thefunds fieldset hr {
        display: none;
    }

</style>
<asp:UpdatePanel ID="upPayment" runat="server">
    <ContentTemplate>

        <asp:HiddenField ID="CollapsePersonData" ClientIDMode="Static" runat="server" Value="false" />
        <asp:HiddenField ID="CollapseCardData" ClientIDMode="Static" runat="server" Value="false"/>

        <Rock:RockLiteral ID="dbgTst" runat="server" />

        <asp:Panel ID="pnlPaymentInfo" CssClass="panel panel-block" runat="server">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-credit-card"></i> <asp:Literal ID="lPanelTitle1" runat="server" /></h1>
            </div>
            <div class="panel-body">

                <% if ( FluidLayout )
                { %>
                <div class="row">
                    <div class="col-md-6">
                <% } %>

                        <div class="panel panel-default contribution-info">
                            <div class="panel-heading"><h3 class="panel-title"><asp:Literal ID="lContributionInfoTitle" runat="server" /></h3></div>
                            <div class="panel-body" id="thefunds">
                                <fieldset>

                                    <div id="fundadd">
                                        <Rock:ButtonDropDownList ID="btnCamp" runat="server" CssClass="btn btn-primary" Visible="false" Label=" "
                                            DataTextField="Name" DataValueField="Id" OnSelectionChanged="btnCamp_SelectionChanged" />
                                        <Rock:ButtonDropDownList ID="btnAddAccount" runat="server" CssClass="btn btn-primary" Visible="false" Label=" "
                                            DataTextField="PublicName" DataValueField="Id" OnSelectionChanged="btnAddAccount_SelectionChanged" AutoPostBack="true"/>
                                    </div>

                                    <asp:Repeater ID="rptAccountList" runat="server">
                                        <ItemTemplate>
                                            <Rock:CurrencyBox ID="txtAccountAmount" runat="server" Label='<%# Eval("PublicName") %>' Text='<%# ((decimal)Eval("Amount") != 0) ? ((decimal)Eval("Amount")).ToString("N2") : "" %>' Placeholder="0.00" CssClass="account-amount" />
                                            <%# ((decimal)Eval("Amount") != 0) ? "<hr />" : "" %>
                                        </ItemTemplate>
                                    </asp:Repeater>


                                    <div class="form-group contribution-total">
                                        <label>Total:</label>
                                        <asp:Label ID="lblTotalAmount" runat="server" CssClass="form-control-static total-amount" />
                                    </div>

                                </fieldset>
                            </div>
                        </div>

                        <div id="divActions2" class="actions clearfix margin-b-lg hidden">
                            <asp:LinkButton ID="LinkButton1" runat="server" Text="Previous" CssClass="btn btn-link" OnClick="btnPrev_Click" Visible="false" />
                            <asp:LinkButton ID="LinkButton2" runat="server" Text="Next" CssClass="btn btn-primary pull-right" OnClick="btnNext_Click"/>
                        </div>
                        
                        <div id="collapse0" class="panel-group">
                            <div class="panel panel-default contribution-info">
                                <a data-toggle="collapse" href="#collapse0 .panel-collapse" style="text-decoration: none">
                                    <div class="panel-heading">
                                        <h3 class="panel-title">
                                            Frequency <small>Touch to Change<br /></small>
                                            <span id="FrequencyText" style="text-decoration: none; color: #000; font-size: 14px; font-weight: normal;"></span>
                                        </h3>
                                    </div>
                                </a>
                                <div class="panel-collapse collapse">
                                    <div class="panel-body">
                                        <fieldset>
                                            <div id="divRepeatingPayments" runat="server" visible="false">
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <Rock:ButtonDropDownList ID="btnFrequency" runat="server" CssClass="btn btn-primary" Label="Frequency" DataTextField="Value" DataValueField="Id" />
                                                    </div>
                                                    <div class="col-md-6">
                                                        <Rock:DatePicker ID="dtpStartDate" runat="server" Label="First Payment" />
                                                    </div>
                                                </div>
                                            </div>
                                        </fieldset>
                                    </div>
                                </div>
                            </div>
                        </div>

                    <% if ( FluidLayout )
                    { %>
                    </div>
                    <div class="col-md-6">
                    <% } %>
                        
                        <div id="collapse1" class="panel-group">
                            <div class="panel panel-default contribution-personal">
                                <a data-toggle="collapse" href="#collapse1 .panel-collapse" style="text-decoration: none">
                                    <div class="panel-heading">
                                        <h3 class="panel-title">
                                            <asp:Literal ID="lPersonalInfoTitle" runat="server" /> <small>Touch to Change</small>
                                        </h3>
                                    </div>
                                </a>
                                <div class="panel-collapse collapse">
                                    <div class="panel-body" id="PersonDetails">
                                        <fieldset>
                                            <Rock:RockLiteral ID="txtCurrentName" runat="server" Label="Name" Visible="true" />
                                            <Rock:RockTextBox ID="txtFirstName" runat="server" Label="First Name"></Rock:RockTextBox>
                                            <Rock:RockTextBox ID="txtLastName" runat="server" Label="Last Name"></Rock:RockTextBox>
                                            <Rock:PhoneNumberBox ID="pnbPhone" runat="server" Label="Phone"></Rock:PhoneNumberBox>
                                            <Rock:RockTextBox ID="txtEmail" runat="server" Label="Email"></Rock:RockTextBox>
                                            <Rock:AddressControl ID="acAddress" runat="server" UseStateAbbreviation="true" UseCountryAbbreviation="false" />
                                        </fieldset>
                                    </div>
                                </div>
                            </div>
                        </div>

                <% if ( FluidLayout )
                { %>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                <% } %>
                        
                        <div id="collapse2" class="panel-group">
                            <div class="panel panel-default contribution-payment">
                                <asp:HiddenField ID="hfPaymentTab" runat="server" />
                                <a data-toggle="collapse" href="#collapse2 .panel-collapse" style="text-decoration: none">
                                    <div class="panel-heading">
                                        <h3 class="panel-title">
                                            <asp:Literal ID="lPaymentInfoTitle" runat="server" /> <small>Touch to Change<br /></small>
                                            <span id="SavedCardName" style="text-decoration: none; color: #000; font-size:14px; font-weight: normal;"></span>
                                        </h3>
                                    </div>
                                </a>
                                <div class="panel-collapse collapse">
                                    <div class="panel-body">   
                                        <asp:PlaceHolder ID="phPills" runat="server" Visible="false">
                                            <ul class="nav nav-pills">
                                                <li id="liCreditCard" runat="server"><a href='#<%=divCCPaymentInfo.ClientID%>' data-toggle="pill">Credit Card</a></li>
                                                <li id="liACH" runat="server"><a href='#<%=divACHPaymentInfo.ClientID%>' data-toggle="pill">Bank Account</a></li>
                                            </ul>
                                        </asp:PlaceHolder>
                                        <div class="tab-content">
                                            <div id="divCCPaymentInfo" runat="server" visible="false">
                                                <fieldset>
                                                    <Rock:RockRadioButtonList ID="rblSavedCC" runat="server" Label=" " CssClass="radio-list" RepeatDirection="Vertical" DataValueField="Id" DataTextField="Name" ClientIDMode="Static" />
                                                    <div id="divNewCard" runat="server" class="radio-content">
                                                        <div class="row">
                                                            <div class="col-md-4">
                                                                <Rock:RockTextBox ID="txtCardFirstName" runat="server" Label="First Name on Card" Visible="false"></Rock:RockTextBox>
                                                                <Rock:RockTextBox ID="txtCardLastName" runat="server" Label="Last Name on Card" Visible="false"></Rock:RockTextBox>
                                                                <Rock:RockTextBox ID="txtCardName" runat="server" Label="Name on Card" Visible="false"></Rock:RockTextBox>
                                                            </div>
                                                            <div class="col-md-3">
                                                                <Rock:RockTextBox ID="txtCreditCard" runat="server" Label="Credit Card Number" MaxLength="19" CssClass="credit-card" />
                                                            </div>
                                                            <div class="col-md-3">
                                                                <Rock:MonthYearPicker ID="mypExpiration" runat="server" Label="Expires" />
                                                            </div>
                                                            <div class="col-md-2">
                                                                <Rock:NumberBox ID="txtCVV" Label="Security Code" CssClass="input-width-xs" runat="server" MaxLength="4" />
                                                            </div>
                                                        </div>

                                                        <ul class="card-logos list-unstyled">
                                                            <li class="card-visa"></li>
                                                            <li class="card-mastercard"></li>
                                                            <li class="card-amex"></li>
                                                            <li class="card-discover"></li>
                                                        </ul>

                                                        <Rock:RockCheckBox ID="cbBillingAddress" runat="server" Text="Enter a different billing address" CssClass="toggle-input" />
                                                        <div id="divBillingAddress" runat="server" class="toggle-content">
                                                            <Rock:AddressControl ID="acBillingAddress" runat="server" UseStateAbbreviation="true" UseCountryAbbreviation="false" />
                                                        </div>                                    
                                                    </div>
                                                </fieldset>
                                            </div>

                                            <div id="divACHPaymentInfo" runat="server" visible="false">
                                                <fieldset>
                                                    <Rock:RockRadioButtonList ID="rblSavedAch" runat="server" Label=" " CssClass="radio-list" RepeatDirection="Vertical" DataValueField="Id" DataTextField="Name" ClientIDMode="Static"/>
                                                    <div class="radio-content">
                                                    <div id="divNewBank" runat="server" ClientIDMode="Static">     
                                                        <div class="row">
                                                            <Rock:RockTextBox ID="txtBankName" runat="server" Label="Bank Name" Text="Bank" Visible="false"/>
                                                            <div class="col-md-6">
                                                                <Rock:RockTextBox ID="txtRoutingNumber" runat="server" Label="Routing Number" />
                                                            </div>
                                                            <div class="col-md-6">
                                                                <Rock:RockTextBox ID="txtAccountNumber" runat="server" Label="Account Number" />
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-sm-6 col-md-8"> 
                                                                <Rock:RockRadioButtonList ID="rblAccountType" runat="server" RepeatDirection="Horizontal" Label="Account Type">
                                                                    <asp:ListItem Text="Checking" Selected="true" />
                                                                    <asp:ListItem Text="Savings" />
                                                                </Rock:RockRadioButtonList>
                                                            </div>
                                                            <div class="col-sm-6 col-md-4">
                                                                <asp:Image ID="imgCheck" runat="server" ImageUrl="<%$ Fingerprint:~/Assets/Images/check-image.png %>" />  
                                                            </div>
                                                        </div>                                  
                                                    </div>
                                                    <div id="divRecentCheck" runat="server" ClientIDMode="Static">
                                                        <div class="row">                            
                                                            <Rock:RockTextBox ID="txtCheckBankName" runat="server" Label="Bank Name" Text="Bank" Visible="false"/>
                                                            <div class="col-md-6">
                                                                <Rock:RockTextBox ID="txtCheckRoutingNumber" runat="server" Label="Routing Number" />
                                                            </div>
                                                            <div class="col-md-6">
                                                                <Rock:RockTextBox ID="txtCheckAccountNumber" runat="server" Label="Account Number" />
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-sm-6 col-md-8"> 
                                                                <Rock:RockRadioButtonList ID="rblCheckAccountType" runat="server" RepeatDirection="Horizontal" Label="Account Type">
                                                                    <asp:ListItem Text="Checking" Selected="true" />
                                                                    <asp:ListItem Text="Savings" />
                                                                </Rock:RockRadioButtonList>
                                                            </div>
                                                            <div class="col-sm-6 col-md-4">
                                                                <asp:Image ID="imgCheck2" runat="server" ImageUrl="<%$ Fingerprint:~/Assets/Images/check-image.png %>" />  
                                                            </div>
                                                        </div>                                  
                                                    </div>
                                                    </div>
                                                </fieldset>
                                            </div>
                                        </div>
                                    </div>
                                </div> 
                            </div>
                        </div>

                <% if ( FluidLayout )
                { %>
                    </div>
                </div>            
                <% } %>

            </div>
        </asp:Panel>

        <asp:Panel ID="pnlConfirmation" CssClass="panel panel-block" runat="server" Visible="false">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-credit-card"></i> <asp:Literal ID="lPanelTitle2" runat="server" /></h1>
            </div>
            <div class="panel-body">
                <div class="panel panel-default">

                    <div class="panel-heading">
                        <h1 class="panel-title"><asp:Literal ID="lConfirmationTitle" runat="server" /></h1>
                    </div>
                    <div class="panel-body">
                        <asp:PlaceHolder ID="phConfirmationHeader" runat="server"></asp:PlaceHolder>
                        <dl class="dl-horizontal gift-confirmation margin-b-md">
                            <Rock:TermDescription ID="tdNameConfirm" runat="server" Term="Name" />
                            <Rock:TermDescription ID="tdPhoneConfirm" runat="server" Term="Phone" />
                            <Rock:TermDescription ID="tdEmailConfirm" runat="server" Term="Email" />
                            <Rock:TermDescription ID="tdAddressConfirm" runat="server" Term="Address" />
                            <Rock:TermDescription runat="server" />
                            <asp:Repeater ID="rptAccountListConfirmation" runat="server">
                                <ItemTemplate>
                                    <Rock:TermDescription ID="tdAmount" runat="server" Term='<%# Eval("Name") %>' Description='<%# Eval("AmountFormatted") %>' />
                                </ItemTemplate>
                            </asp:Repeater>
                            <Rock:TermDescription ID="tdTotalConfirm" runat="server" Term="Total" />
                            <Rock:TermDescription runat="server" />
                            <Rock:TermDescription ID="tdPaymentMethodConfirm" runat="server" Term="Payment Method" />
                            <Rock:TermDescription ID="tdAccountNumberConfirm" runat="server" Term="Account Number" />
                            <Rock:TermDescription ID="tdWhenConfirm" runat="server" Term="When" />
                        </dl>
                
                        <asp:PlaceHolder ID="phConfirmationFooter" runat="server" />
                        <asp:Panel ID="pnlDupWarning" runat="server" CssClass="alert alert-block">
                            <h4>Warning!</h4>
                            <p>
                                You have already submitted a similar transaction that has been processed.  Are you sure you want
                            to submit another possible duplicate transaction?
                            </p>
                            <asp:LinkButton ID="btnConfirm" runat="server" Text="Yes, submit another transaction" CssClass="btn btn-danger margin-t-sm" OnClick="btnConfirm_Click" />
                        </asp:Panel>
                    </div>
                </div>
            </div>

        </asp:Panel>

        <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
            <div class="well">
                <legend><asp:Literal ID="lSuccessTitle" runat="server" /></legend>
                <asp:PlaceHolder ID="phSuccessHeader" runat="server"></asp:PlaceHolder>
                <dl class="dl-horizontal gift-success">
                    <Rock:TermDescription ID="tdScheduleId" runat="server" Term="Payment Schedule ID" />
                    <Rock:TermDescription ID="tdTransactionCodeReceipt" runat="server" Term="Confirmation Code" />
                    <Rock:TermDescription runat="server" />
                    <Rock:TermDescription ID="tdNameReceipt" runat="server" Term="Name" />
                    <Rock:TermDescription ID="tdPhoneReceipt" runat="server" Term="Phone" />
                    <Rock:TermDescription ID="tdEmailReceipt" runat="server" Term="Email" />
                    <Rock:TermDescription ID="tdAddressReceipt" runat="server" Term="Address" />
                    <Rock:TermDescription runat="server" />
                    <asp:Repeater ID="rptAccountListReceipt" runat="server">
	                    <ItemTemplate>
		                    <Rock:TermDescription ID="tdAccountAmountReceipt" runat="server" Term='<%# Eval("Name") %>' Description='<%# Eval("AmountFormatted") %>' />
	                    </ItemTemplate>
                    </asp:Repeater>
                    <Rock:TermDescription ID="tdTotalReceipt" runat="server" Term="Total" />
                    <Rock:TermDescription runat="server" />
                    <Rock:TermDescription ID="tdPaymentMethodReceipt" runat="server" Term="Payment Method" />
                    <Rock:TermDescription ID="tdAccountNumberReceipt" runat="server" Term="Account Number" />
                    <Rock:TermDescription ID="tdWhenReceipt" runat="server" Term="When" />
                </dl>


                <dl class="dl-horizontal gift-confirmation margin-b-md">
                            
                </dl>
            </div>

            <asp:Panel ID="pnlSaveAccount" runat="server" Visible="false">
                <div class="well">
                    <legend><asp:Literal ID="lSaveAcccountTitle" runat="server" /></legend>
                    <fieldset>
                        <Rock:RockCheckBox ID="cbSaveAccount" runat="server" Text="Save account information for future gifts" CssClass="toggle-input" />
                        <div id="divSaveAccount" runat="server" class="toggle-content">
                            <Rock:RockTextBox ID="txtSaveAccount" runat="server" Label="Name for this account" CssClass="input-large"></Rock:RockTextBox>

                            <asp:PlaceHolder ID="phCreateLogin" runat="server" Visible="false">

                                <div class="control-group">
                                    <div class="controls">
                                        <div class="alert alert-info">
                                            <b>Note:</b> For security purposes you will need to login to use your saved account information.  To create
	    			                    a login account please provide a user name and password below. You will be sent an email with the account 
	    			                    information above as a reminder.
                                        </div>
                                    </div>
                                </div>

                                <Rock:RockTextBox ID="txtUserName" runat="server" Label="Username" CssClass="input-medium" />
                                <Rock:RockTextBox ID="txtPassword" runat="server" Label="Password" CssClass="input-medium" TextMode="Password" />
                                <Rock:RockTextBox ID="txtPasswordConfirm" runat="server" Label="Confirm Password" CssClass="input-medium" TextMode="Password" />

                            </asp:PlaceHolder>

                            <Rock:NotificationBox ID="nbSaveAccount" runat="server" Visible="false" NotificationBoxType="Danger"></Rock:NotificationBox>

                            <div id="divSaveActions" runat="server" class="actions">
                                <asp:LinkButton ID="lbSaveAccount" runat="server" Text="Save Account" CssClass="btn btn-primary" OnClick="lbSaveAccount_Click" />
                            </div>
                        </div>
                    </fieldset>                    
                </div>
            </asp:Panel>

            <asp:PlaceHolder ID="phSuccessFooter" runat="server" />

        </asp:Panel>

        <Rock:NotificationBox ID="nbMessage" runat="server" Visible="false"></Rock:NotificationBox>

        <div id="divActions" runat="server" class="actions clearfix margin-b-lg">
            <asp:LinkButton ID="btnPrev" runat="server" Text="Previous" CssClass="btn btn-link" OnClick="btnPrev_Click" Visible="false" />
            <asp:LinkButton ID="btnNext" runat="server" Text="Next" CssClass="btn btn-primary pull-right" OnClick="btnNext_Click" />
        </div>

        <asp:HiddenField ID="hfCurrentPage" runat="server" />

    </ContentTemplate>
</asp:UpdatePanel>
