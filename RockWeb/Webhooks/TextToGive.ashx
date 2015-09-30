<%@ WebHandler Language="C#" Class="TextToGive" %>
// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//

using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Communication;
using Twilio;


public class TextToGive : IHttpHandler
{
    private HttpRequest request;
    private HttpResponse response;


    private RockContext rockContext = new RockContext();

    public void ProcessRequest( HttpContext context )
    {

        request = context.Request;
        response = context.Response;

        response.ContentType = "text/plain";

        if ( request.HttpMethod != "POST" )
        {
            response.Write( "Invalid request type." );
            return;
        }


        if ( request.Form["SmsStatus"] != null)
        {
            switch ( request.Form["SmsStatus"] )
            {
                case "received":
                    MessageRecieved();
                    break;
                case "undelivered":
                    MessageUndelivered();
                    break;
            }

            response.StatusCode = 200;
        }

        else
        {
            response.StatusCode = 500;
        }

    }

    private void MessageUndelivered()
    {
        string messageSid = string.Empty;

        if ( !string.IsNullOrEmpty( request.Form["MessageSid"] ) )
        {
            messageSid = request.Form["MessageSid"];

            // get communication from the message side
            RockContext rockContext = new RockContext();
            CommunicationRecipientService recipientService = new CommunicationRecipientService(rockContext);

            var communicationRecipient = recipientService.Queryable().Where( r => r.UniqueMessageId == messageSid ).FirstOrDefault();
            communicationRecipient.Status = CommunicationRecipientStatus.Failed;
            communicationRecipient.StatusNote = "Message failure notified from Twilio on " + RockDateTime.Now.ToString();
            rockContext.SaveChanges();
        }
    }

    private void MessageRecieved()
    {
        string fromPhone = string.Empty;
        string toPhone = string.Empty;
        string body = string.Empty;

        if ( !string.IsNullOrEmpty( request.Form["To"] ) ) {
            toPhone = request.Form["To"];
        }

        if ( !string.IsNullOrEmpty( request.Form["From"] ) )
        {
            fromPhone = request.Form["From"];
        }

        if (!string.IsNullOrEmpty(request.Form["Body"]))
        {
            body = request.Form["Body"];
        }

        // determine if we should log
        if ( !string.IsNullOrEmpty( request.QueryString["Log"] ) && request.QueryString["Log"] == "true" )
        {
            WriteToLog( fromPhone, toPhone, body );
        }

        if ( !(string.IsNullOrWhiteSpace(toPhone)) && !(string.IsNullOrWhiteSpace(fromPhone)) )
        {
            string errorMessage = string.Empty;

            //Look up the person
            string personDetails = "";
            char[] trimThese = {'+', '1',' ' };
            string lookupNumber = fromPhone.TrimStart(trimThese);

            IQueryable<Person> personList = LookupPersonByPhone(lookupNumber);

            if (personList == null)
            {
                //personDetails = thePerson.FullName;
                //SendEmail("bwitting@newpointe.org", "bwitting@newpointe.org", "Test SMS Email ", "Crap, there are no results for: " + lookupNumber, rockContext);
                SendSms(lookupNumber, "We don't have an account set up for this phone number yet. Go to: https://my.newpointe.org and set up an account.  If you already have an account, log in and add this cell phone number.");
            }
            else if (personList.Count() == 1)
            {
                string responseMessage = "";
                //Get person details
                var thePerson = personList.FirstOrDefault();
                personDetails = thePerson.FullName;


                //Todo: Check if the person has a MyNewPointe account
                if (thePerson.Users.Count > 0)
                {
                    var test = thePerson.Users.ToList();
                    responseMessage += "You have a MyNewPointe Account. Username: " + test[0].UserName + ". ";
                }
                else
                {
                    responseMessage += "You don't have a MyNewPointe Account. ";
                    
                }

                //Todo: Check if opted into text to give

                //Does the person have a saved CC account?
                var ccCurrencyType = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CREDIT_CARD ) );
                var ccResults =  new FinancialPersonSavedAccountService(new RockContext()).GetByPersonId( (int)thePerson.PrimaryAliasId).Where( a =>
                            a.CurrencyTypeValueId == ccCurrencyType.Id)
                        .OrderBy( a => a.Name )
                        .Select( a => new
                        {
                            Id = a.Id,
                            Name = a.Name,
                            CardType = a.CreditCardTypeValue.Value.ToString(),
                            MaskedAccountNumber = a.MaskedAccountNumber
                        } ).ToList();

                if ( ccResults.Count == 0 )  {
                    //do something if no accounts saved
                    responseMessage = "You don't have credit card saved in MyNewPointe. Go here to save one now: https://my.newpointe.org/SavedCards";

                }
                else
                {
                    if (ccResults.Count == 1)
                    {
                        char[] trim = { '*' };
                        responseMessage = ccResults[0].Name + " (" + ccResults[0].CardType + " ending in " +
                                      ccResults[0].MaskedAccountNumber.TrimStart(trim) + ")";
                    }
                    else
                    {
                        //how do we know which account to use?  
                        responseMessage =
                            "You have more than one saved credit card in MyNewPointe.  Go here to set a default: https://my.newpointe.org/SavedCaeds";
                    }

                }



                //SendEmail("bwitting@newpointe.org", "bwitting@newpointe.org", "Test SMS Email ", "Sweet, there is one result for this number: " + personDetails, rockContext);
                SmsTest(thePerson);
                SendSms(lookupNumber, thePerson.NickName + ", thank you for giving $" + body + " to NewPointe with " + responseMessage);
            }
            else if (personList.Count() > 1)
            {
                //List all people and ask who they are.
                string smsBody = "";
                var thePeople = personList.ToList();
                int i = 1;
                foreach (var person in thePeople)
                {
                    smsBody += "\nReply with \"#"+i+"\" if this is " + person.FullName + " ";
                    i++;
                }
                //SendEmail("bwitting@newpointe.org", "bwitting@newpointe.org", "Test SMS Email ", "Oh no, there is more than one result for this number: " + smsBody, rockContext);
                SendSms(lookupNumber, "There is more than one result for this number. " + smsBody);
            }


            if ( errorMessage != string.Empty )
            {
                response.Write( errorMessage );
            }
        }
    }

    private void WriteToLog (string fromPhone, string toPhone, string body) {

        string logFile = HttpContext.Current.Server.MapPath( "~/App_Data/Logs/TwilioLog.txt" );

        using ( System.IO.FileStream fs = new System.IO.FileStream( logFile, System.IO.FileMode.Append, System.IO.FileAccess.Write ) )
        using ( System.IO.StreamWriter sw = new System.IO.StreamWriter( fs ) )
        {
            sw.WriteLine( string.Format("{0} - From: '{1}', To: '{2}', Message: '{3}'", RockDateTime.Now.ToString(), fromPhone, toPhone, body) );
        }
    }


    private void SendEmail(string recipient, string from, string subject, string body, RockContext rockContext)
    {
        var recipients = new List<string>();
        recipients.Add(recipient);

        var mediumData = new Dictionary<string, string>();
        mediumData.Add("From", from);
        mediumData.Add("Subject", subject);
        mediumData.Add("Body", body);

        var mediumEntity = EntityTypeCache.Read(Rock.SystemGuid.EntityType.COMMUNICATION_MEDIUM_EMAIL.AsGuid(), rockContext);
        if (mediumEntity != null)
        {
            var medium = MediumContainer.GetComponent(mediumEntity.Name);
            if (medium != null && medium.IsActive)
            {
                var transport = medium.Transport;
                if (transport != null && transport.IsActive)
                {
                    var appRoot = GlobalAttributesCache.Read(rockContext).GetValue("InternalApplicationRoot");
                    transport.Send(mediumData, recipients, appRoot, string.Empty);
                }
            }
        }
    }


    private void SendSms(string number, string body)
    {
        string AccountSid = "AC829f7d9fb532f01252621c6af4e7849e";
        string AuthToken = "47676cee39cb444ead74a3ed9bd48ba1";
        var twilio = new TwilioRestClient(AccountSid, AuthToken);

        var message = twilio.SendMessage("+13305566655", "3309340252", body);


        Console.WriteLine(message.Sid);
    }


    private void SmsTest(Rock.Model.Person person)
    {
        var testCommunication = new Rock.Model.Communication();
        testCommunication.FutureSendDateTime = null;
        testCommunication.Status = CommunicationStatus.Approved;
        testCommunication.ReviewedDateTime = RockDateTime.Now;
        testCommunication.ReviewerPersonAliasId = 10779;
        testCommunication.SenderPersonAliasId = 10779;
        testCommunication.Subject = "test";
        testCommunication.IsBulkCommunication = false;
        testCommunication.MediumDataJson = null;
        testCommunication.AdditionalMergeFieldsJson = null;
        testCommunication.MediumEntityTypeId = 38;

        var testRecipient = new CommunicationRecipient();
        testRecipient.Status = CommunicationRecipientStatus.Pending;
        //testRecipient.PersonAliasId = person.PrimaryAliasId ?? default(int);
        testRecipient.PersonAliasId = 10779;
        testCommunication.Recipients.Add( testRecipient );


        var rockContext = new RockContext();
        var communicationService = new CommunicationService( rockContext );
        communicationService.Add( testCommunication );
        rockContext.SaveChanges();

        var medium = testCommunication.Medium;
        if ( medium != null )
        {
            medium.Send( testCommunication );
        }

        communicationService.Delete( testCommunication );
        rockContext.SaveChanges();


    }



    public IQueryable<Person> LookupPersonByPhone(string phone)
    {
        var personService = new PersonService( rockContext );
        IQueryable<Person> people = null;
        var phoneService = new PhoneNumberService( rockContext );
        var personIds = phoneService.GetPersonIdsByNumber( phone );
        people = personService.Queryable().Where( p => personIds.Contains( p.Id ) );

        return people;
    }



    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}