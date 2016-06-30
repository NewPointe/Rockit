<%@ WebHandler Language="C#" Class="RockWeb.Webhooks.Shape" %>
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
using Newtonsoft.Json;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Communication;
using System.Diagnostics;
using System.Globalization;

namespace RockWeb.Webhooks
{

    public class Shape : IHttpHandler
    {
        private int transactionCount = 0;
        private RockContext rockContext = new RockContext();

        private string TopGift1;
        private string TopGift2;
        private string LowestGift;

        private string Email;
        private string FirstName;
        private string LastName;

        private Person ThePerson;

        private string FormId;
            



        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            response.ContentType = "text/plain";

            if (request.HttpMethod != "POST")
            {
                response.Write("Invalid request type.");
                return;
            }

            // Get personal fields out of POST data
            Email = request.Form["Email"];
            FirstName = request.Form["FirstName"];
            LastName = request.Form["LastName"];
            FormId = request.Form["FormID"] + "-" + request.Form["EntryNumber"];


            // Get the person based on the form (or make a new one)
            var person = GetPerson(rockContext);
            ThePerson = person;


            // Build dictionary of <GiftId> <TotalScore>
            Dictionary<string, int> GiftDictionary = new Dictionary<string, int>();

            // Pre-populate dictionary based on number of gifts in POST data
            int numberOfGifts = 5;

            for (int i = 1; i <= numberOfGifts; i++)
            {
                GiftDictionary.Add(i.ToString(), 0);
            }


            // Go through Post Data and add up scores for each Gift type
            foreach (string x in request.Params.Keys)
            {
                if (x.Length == 5 && x.Contains("-"))
                {
                    string gift = Int32.Parse(x.Substring(3, 2)).ToString();

                    GiftDictionary[gift] = GiftDictionary[gift] + Int32.Parse(request.Params[x]);

                }

            }

            
            // Make a SortedDictionary to sort highest scores descending (yay for avoiding sorting algorithm!)
            var sortedGiftDictionary = from entry in GiftDictionary orderby entry.Value descending select entry;

            // Set highest and lowest gifts
            TopGift1 = sortedGiftDictionary.ElementAt(0).Key;
            TopGift2 = sortedGiftDictionary.ElementAt(1).Key;
            LowestGift = sortedGiftDictionary.Last().Key;

            // Save the attributes
            SaveAttributes(Int32.Parse(TopGift1),Int32.Parse(TopGift2));


            // Testing: write each value in the response for varification
            foreach (var x in GiftDictionary)
            {
                response.Write("Key: " + x.Key + " | Value: " + x.Value + "<br>");
            }
            response.Write("<br>PersonId: " + person.Id);
            response.Write("<br>Top Gift: " + TopGift1);
            response.Write("<br>2nd Gift: " + TopGift2);
            response.Write("<br>Bottom Gift: " + LowestGift);


            // Write a 200 code in the response
            response.ContentType = "text/xml";
            response.AddHeader("Content-Type", "text/xml");
            response.StatusCode = 200;

            

        }

        /// <summary>
        /// Write the 2 highest gift attributes on the person's record.
        /// </summary>
        /// <param name="Gift1">Int of category of Gift1</param>
        /// <param name="Gift2">Int of category of Gift2</param>
        /// <returns></returns>
        public void SaveAttributes(int Gift1, int Gift2)
        {

            AttributeService attributeService = new AttributeService(rockContext);
            AttributeValueService attributeValueService = new AttributeValueService(rockContext);
            AttributeValue attributeValue;
            AttributeValue attributeValue2;
            AttributeValue formAttributeValue;


            var spiritualGift1Attribute = attributeService.Queryable().FirstOrDefault(a => a.Key == "SpiritualGift1");
            var spiritualGift2Attribute = attributeService.Queryable().FirstOrDefault(a => a.Key == "SpiritualGift2");
            var spiritualGiftFormAttribute = attributeService.Queryable().FirstOrDefault(a => a.Key == "SpiritualGiftForm");


            attributeValue = attributeValueService.GetByAttributeIdAndEntityId(spiritualGift1Attribute.Id, ThePerson.Id);
            attributeValue2 = attributeValueService.GetByAttributeIdAndEntityId(spiritualGift2Attribute.Id, ThePerson.Id);



            if (attributeValue == null)
            {
                attributeValue = new AttributeValue();
                attributeValue.AttributeId = spiritualGift1Attribute.Id;
                attributeValue.EntityId = ThePerson.Id;
                attributeValue.Value = Gift1.ToString();
                attributeValueService.Add(attributeValue);
            }
            else
            {
                attributeValue.AttributeId = spiritualGift1Attribute.Id;
                attributeValue.EntityId = ThePerson.Id;
                attributeValue.Value = Gift1.ToString();
            }



            if (attributeValue2 == null)
            {
                attributeValue2 = new AttributeValue();
                attributeValue2.AttributeId = spiritualGift2Attribute.Id;
                attributeValue2.EntityId = ThePerson.Id;
                attributeValue2.Value = Gift2.ToString();
                attributeValueService.Add(attributeValue2);
            }
            else
            {
                attributeValue2.AttributeId = spiritualGift2Attribute.Id;
                attributeValue2.EntityId = ThePerson.Id;
                attributeValue2.Value = Gift2.ToString();
            }



            formAttributeValue = new AttributeValue();
            formAttributeValue.AttributeId = spiritualGiftFormAttribute.Id;
            formAttributeValue.EntityId = ThePerson.Id;
            formAttributeValue.Value = FormId;
            attributeValueService.Add(formAttributeValue);
                

            rockContext.SaveChanges();


        }



        /// <summary>
        /// Gets the person from form data, or creates a new person if one doesn't exist
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        private Person GetPerson(RockContext rockContext)
        {
            var personService = new PersonService(rockContext);

            var personMatches = personService.GetByEmail(Email)
                .Where(p =>
                   p.LastName.Equals(LastName, StringComparison.OrdinalIgnoreCase) &&
                   ((p.FirstName != null && p.FirstName.Equals(FirstName, StringComparison.OrdinalIgnoreCase)) ||
                       (p.NickName != null && p.NickName.Equals(FirstName, StringComparison.OrdinalIgnoreCase))))
                .ToList();
            if (personMatches.Count() >= 1)
            {
                return personMatches.FirstOrDefault();
            }
            else
            {
                DefinedValueCache dvcConnectionStatus = DefinedValueCache.Read("368DD475-242C-49C4-A42C-7278BE690CC2");
                DefinedValueCache dvcRecordStatus = DefinedValueCache.Read("283999EC-7346-42E3-B807-BCE9B2BABB49");

                Person person = new Person();
                person.FirstName = FirstName;
                person.LastName = LastName;
                person.Email = Email;
                person.IsEmailActive = true;
                person.EmailPreference = EmailPreference.EmailAllowed;
                person.RecordTypeValueId = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid()).Id;
                if (dvcConnectionStatus != null)
                {
                    person.ConnectionStatusValueId = dvcConnectionStatus.Id;
                }

                if (dvcRecordStatus != null)
                {
                    person.RecordStatusValueId = dvcRecordStatus.Id;
                }

                PersonService.SaveNewPerson(person, rockContext, null, false);

                return personService.Get(person.Id);
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
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





    }


}