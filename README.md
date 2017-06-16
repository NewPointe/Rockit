Rockit
=======

NewPointe's Implementation of the Rock (SDK)

 - [Blocks](#blocks)
 - [Workflow Actions](#workflow-actions)
 - [Jobs](#jobs)
 - [Webhooks](#webhooks)
 - [Themes](#themes)
 - [Mods](#mods)
 - [Misc](#misc)



Blocks
=======

#### ServiceU Calendar
[/org.newpointe.ServiceUCalendar/](/org.newpointe.ServiceUCalendar/)  
[/RockWeb/Plugins/org_newpointe/ServiceUEvents/](/RockWeb/Plugins/org_newpointe/ServiceUEvents/)  
[/RockWeb/Plugins/org_newpointe/Calendar.ashx](/RockWeb/Plugins/org_newpointe/Calendar.ashx)  
[/RockWeb/Plugins/org_newpointe/CalendarSearch.aspx](/RockWeb/Plugins/org_newpointe/CalendarSearch.aspx)  
[/RockWeb/Plugins/org_newpointe/CalendarSearch.aspx.cs](/RockWeb/Plugins/org_newpointe/CalendarSearch.aspx.cs)  
[/RockWeb/Styles/autocomplete-styles.css](/RockWeb/Styles/autocomplete-styles.css)  
[/RockWeb/Scripts/tmpls/](/RockWeb/Scripts/tmpls/)  
[/RockWeb/Scripts/components/](/RockWeb/Scripts/components/)  
[/RockWeb/Scripts/?](/RockWeb/Scripts/)  


#### Custom CheckinSuccess
Custom Checkin Success block modded to allow batch-cutting labels.  
[/RockWeb/Plugins/org_newpointe/BlockMods/Checkin/](/RockWeb/Plugins/org_newpointe/BlockMods/Checkin/)  


#### Custom Financial TransactionEntry
Custom Financial TransactionEntry block. Adds a camus selector to the list of accounts, adds auto-hiding acordians for pre-filled fields on mobile, adds query parameter handling to pre-fill accounts, values, and the date, and a few other minor tweeks.  
[/RockWeb/Plugins/org_newpointe/BlockMods/Finance/](/RockWeb/Plugins/org_newpointe/BlockMods/Finance/)  


#### Custom GroupFinder
Custom GroupFinder block modded to change the campus checkboxes into a dropdown.  
[/RockWeb/Plugins/org_newpointe/BlockMods/Groups/](/RockWeb/Plugins/org_newpointe/BlockMods/Groups/)  


#### CheckinAutoStart
Auto-starts check-in with all Group Types selected and allows specifying a campus query parameter to auto-select a kiosk.  
[/RockWeb/Plugins/org_newpointe/Checkin/](/RockWeb/Plugins/org_newpointe/Checkin/)  


#### CustomMenu
The floating nav menu for our website.  
[/RockWeb/Plugins/org_newpointe/CustomMenu](/RockWeb/Plugins/org_newpointe/CustomMenu)  


#### LiveMenu
Creates a popup and shows global attribute 'LiveServiceTextLive' when global attribute 'LiveService' is `true`. Otherwise shows global attribute 'LiveServiceTextNotLive'. Need to change hard-coded Watch Live link.  
[/RockWeb/Plugins/org_newpointe/LiveMenu/](/RockWeb/Plugins/org_newpointe/LiveMenu/)  


#### MachFormEmbed
Embeds a MachForm into a page. Can also auto-fill field values from person name or page parameters.  
[/RockWeb/Plugins/org_newpointe/MachFormEmbed](/RockWeb/Plugins/org_newpointe/MachFormEmbed)  


#### PersonPicker
A block with a PersonPicker. Puts the person's Id as a url parameter.  
[/RockWeb/Plugins/org_newpointe/PersonPicker](/RockWeb/Plugins/org_newpointe/PersonPicker)  


#### Podcasts
Displays the top 4 podcast items in '~/content/assets/podcast.xml'.  
[/RockWeb/Plugins/org_newpointe/Podcasts](/RockWeb/Plugins/org_newpointe/Podcasts)  


#### DatePicker
A block with a DateRangePicker. Puts the daterange as a url parameter.  
[/RockWeb/Plugins/org_newpointe/Reporting/DatePicker.ascx](/RockWeb/Plugins/org_newpointe/Reporting/DatePicker.ascx)  
[/RockWeb/Plugins/org_newpointe/Reporting/DatePicker.ascx.cs](/RockWeb/Plugins/org_newpointe/Reporting/DatePicker.ascx.cs) 


#### Staff
Shows staff/group member photos.  
[/RockWeb/Plugins/org_newpointe/Staff](/RockWeb/Plugins/org_newpointe/Staff)  


#### EmbedTableau
Embeds a chart from a Tableau server.  
[/RockWeb/Plugins/org_newpointe/Tableau](/RockWeb/Plugins/org_newpointe/Tableau)  




Workflow Actions
=======

#### Entity edit/create workflow actions
[/org.newpointe.WorkflowEntities/](/org.newpointe.WorkflowEntities/)  


#### Checkin Location Misc
[/org.newpointe.Checkin/](/org.newpointe.Checkin/)  


#### Shorten URL
Gets a shortened url from a YOURLS url-shortener. Need to change hard-coded shortener URL.  
[/org.newpointe.Giving/](/org.newpointe.Giving/)




Jobs
=======

#### SQL To Workflow job
A Rock Job that runs SQL and starts a workflow for each returned row, saving each column value as an attribute (Column name = Key).  
[/RockWeb/App_Code/SqlToWorkflow.cs](/RockWeb/App_Code/SqlToWorkflow.cs) 


#### ExpiredCards Job
A Rock Job that starts a workflow for everyperson whose card expires in the current month.  
[/org.newpointe.ExpiredCards/](/org.newpointe.ExpiredCards/)  


#### ChurchOnlinePlatform live checker
Checks the ChurchOnlinePlatform API to see if there is any live events, and stores it in a global attribute with the key 'LiveService'  
[/org.newpointe.LiveService/](/org.newpointe.LiveService/)  




Webhooks
=======

#### Basic SMS to Workflow webhook
[/RockWeb/Webhooks/SmsToWorkflow.ashx](/RockWeb/Webhooks/SmsToWorkflow.ashx)  


#### SMS Giving bot
Halfway-written SMS bot for giving.  
[/RockWeb/Webhooks/TS-TextToGive.ashx](/RockWeb/Webhooks/TS-TextToGive.ashx)  


#### Machforms attendance webhook
Webhook for MachForms for tracking attendance counts.  
[/RockWeb/Webhooks/Attendance.ashx](/RockWeb/Webhooks/Attendance.ashx)  




Themes
=======

#### NewPointe Checkin theme
[/RockWeb/Themes/CheckinNewPointe/](/RockWeb/Themes/CheckinNewPointe/)  


#### NewPointe Orange Checkin theme
[/RockWeb/Themes/CheckinNewPointeOrange/](/RockWeb/Themes/CheckinNewPointeOrange/)  


#### NewPointe PointePark Checkin theme
[/RockWeb/Themes/CheckinPointePark/](/RockWeb/Themes/CheckinPointePark/)  


#### At The Movies Checkin theme
Checkin theme for At The Movies series  
[/RockWeb/Themes/CheckinAtTheMovies/](/RockWeb/Themes/CheckinAtTheMovies/)  


#### NewPointe.org theme
[/RockWeb/Themes/NewPointeMain/](/RockWeb/Themes/NewPointeMain/)  




Mods
=======

#### Server-side batch printing checkin labels
Copy of GetFile that allows for batch printing checkin labels.  
[/RockWeb/GetCheckinLabel.ashx](/RockWeb/GetCheckinLabel.ashx)  
[/RockWeb/App_Code/GetCheckinLabel.ashx.cs](/RockWeb/App_Code/GetCheckinLabel.ashx.cs)   
[/RockWeb/Plugins/org_newpointe/BlockMods/Checkin](/RockWeb/Plugins/org_newpointe/BlockMods/Checkin)   


#### Connections Tweeks
Only show active campuses  
[/RockWeb/Blocks/Connection/ConnectionOpportunitySearch.ashx.cs](/RockWeb/Blocks/Connection/ConnectionOpportunitySearch.ashx.cs) !!  
Only show active & available campuses  
[/RockWeb/Blocks/Connection/ConnectionOpportunitySignup.ashx.cs](/RockWeb/Blocks/Connection/ConnectionOpportunitySignup.ashx.cs) !!++  
Add CreatedDateTime column  
[/RockWeb/Blocks/Connection/MyConnectionOpportunities.ashx](/RockWeb/Blocks/Connection/MyConnectionOpportunities.ashx) !!  
[/RockWeb/Blocks/Connection/MyConnectionOpportunities.ashx.cs](/RockWeb/Blocks/Connection/MyConnectionOpportunities.ashx.cs) !!  


#### Add IP to PersonPageViews
[/RockWeb/Blocks/Crm/PersonPageViews.ascx.cs](/RockWeb/Blocks/Crm/PersonPageViews.ascx.cs) !!  


#### Fix giving group in ScheduledTransactionSummary
[/RockWeb/Blocks/Finance/ScheduledTransactionSummary.ascx.cs](/RockWeb/Blocks/Finance/ScheduledTransactionSummary.ascx.cs) !!++  


#### Show inactive accounts in TransactionMatching
[/RockWeb/Blocks/Finance/TransactionMatching.ascx](/RockWeb/Blocks/Finance/TransactionMatching.ascx) !!  


#### Change DatePicker to DateTimePicker in MetricValueDetail
[/RockWeb/Blocks/Reporting/MetricValueDetail.ascx](/RockWeb/Blocks/Reporting/MetricValueDetail.ascx) !!  
[/RockWeb/Blocks/Reporting/MetricValueDetail.ascx.cs](/RockWeb/Blocks/Reporting/MetricValueDetail.ascx.cs) !!  


#### Custom RegistrationEntry
Only show active campuses  
Pre-fill Registerer from first registrant  
[/RockWeb/Plugins/org_newpointe/BlockMods/Event/RegistrationEntry.ascx](/RockWeb/Plugins/org_newpointe/BlockMods/Event/RegistrationEntry.ascx)  
[/RockWeb/Plugins/org_newpointe/BlockMods/Event/RegistrationEntry.ascx.cs](/RockWeb/Plugins/org_newpointe/BlockMods/Event/RegistrationEntry.ascx.cs)  


#### Custom TransactionEntry
Auto-fill from url  
Auto-collapse saved info sections on mobile  
Hack to make CurrencyFields use type="number" ++  
Save date to attribute if url parameter  
Ask to create account after transaction  
[/RockWeb/Plugins/org_newpointe/BlockMods/Finance/TransactionEntry.ascx](/RockWeb/Plugins/org_newpointe/BlockMods/Finance/TransactionEntry.ascx)  
[/RockWeb/Plugins/org_newpointe/BlockMods/Finance/TransactionEntry.ascx.cs](/RockWeb/Plugins/org_newpointe/BlockMods/Finance/TransactionEntry.ascx.cs) 


#### Custom GroupFinder
Add option to filter by campus  ++?  
[/RockWeb/Plugins/org_newpointe/BlockMods/Groups/GroupFinder.ascx](/RockWeb/Plugins/org_newpointe/BlockMods/Groups/GroupFinder.ascx)  
[/RockWeb/Plugins/org_newpointe/BlockMods/Groups/GroupFinder.ascx.cs](/RockWeb/Plugins/org_newpointe/BlockMods/Groups/GroupFinder.ascx.cs)  




Misc
=======

#### .StartOfWeek() DateTime extention
`public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)`  
[/RockWeb/App_Code/DateTimeExtentions.cs](/RockWeb/App_Code/DateTimeExtentions.cs)  


#### Rewrite of ProtectMyMinistry w/ SSNTrace + Multi-County BGChk support
[/RockWeb/Webhooks/ProtectMyMinistryPlus.ashx](/RockWeb/Webhooks/ProtectMyMinistryPlus.ashx)  
[/org.newpointe.ProtectMyMinistry/](/org.newpointe.ProtectMyMinistry/)  



