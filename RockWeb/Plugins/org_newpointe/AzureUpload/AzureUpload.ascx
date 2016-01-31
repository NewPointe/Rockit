<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AzureUpload.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.AzureUpload.AzureUpload" %>
 
<Rock:FileUploader ID="fuTemplateBinaryFile" runat="server" Label="Upload File" Required="true" OnFileUploaded="fuTemplateBinaryFile_FileUploaded" BinaryFileTypeGuid="2CF8A379-33BB-49C1-8CBB-DF8B822C3E75" />
<br/>
<Rock:NotificationBox ID="nbSuccess" runat="server" Title="File Uploaded Successfully!" Text="This is a success message." NotificationBoxType="Success" visible="false" />
