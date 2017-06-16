<%@ WebHandler Language="C#" Class="RockWeb.Webhooks.ProtectMyMinistry" %>
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
using System.Linq;
using System.Xml.Linq;

using Rock;
using Rock.Model;

namespace RockWeb.Webhooks
{
    /// <summary>
    /// Handles the background check results sent from Protect My Ministry
    /// </summary>
    public class ProtectMyMinistry : IHttpHandler
    {
        public void ProcessRequest( HttpContext context )
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            response.ContentType = "text/plain";

            if ( request.HttpMethod != "POST" )
            {
                ExceptionLogService.LogException( new ArgumentException( "Invalid HttpMethod: " + request.HttpMethod ), context );
                response.Write( "Invalid request type." );
                return;
            }

            if ( request.Form["REQUEST"] != null )
            {
                try
                {
                    XDocument xResult = XDocument.Parse( HttpUtility.UrlDecode( request.Form["REQUEST"] ) );

                    // Get the orderid from the XML
                    string orderId = ( from o in xResult.Descendants( "OrderDetail" ) select (string)o.Attribute( "orderID" ) ).FirstOrDefault() ?? "OrderIdUnknown";

                    if ( string.IsNullOrEmpty( orderId ) || orderId == "OrderIdUnknown" )
                    {
                        orderId = ( from o in xResult.Descendants( "OrderDetail" ) select (string)o.Attribute( "OrderId" ) ).FirstOrDefault() ?? "OrderIdUnknown";
                    }

                    // Return the success XML to PMM
                    XDocument xdocResult = new XDocument( new XDeclaration( "1.0", "UTF-8", "yes" ),
                        new XElement( "OrderXML",
                            new XElement( "Success", "TRUE" ) ) );

                    if ( !string.IsNullOrEmpty( orderId ) && orderId != "OrderIdUnknown" )
                    {
                        // Find and update the associated workflow
                        var rockContext = new Rock.Data.RockContext();
                        var workflow = new WorkflowService( rockContext ).Get( orderId.AsInteger() );
                        if ( workflow != null && workflow.IsActive )
                        {
                            workflow.LoadAttributes();

                            var backgroundCheck = new BackgroundCheckService( rockContext ).Queryable().FirstOrDefault( c => c.WorkflowId == workflow.Id );

                            var xTransaction = new XElement( "Transaction",
                                new XAttribute( "TransactionType", "RESPONSE" ),
                                new XAttribute( "RequestDateTime", RockDateTime.Now ),
                                new XAttribute( "ResponseDateTime", RockDateTime.Now ),
                                xResult.Root,
                                xdocResult.Root
                            );

                            org.newpointe.ProtectMyMinistry.ProtectMyMinistryPlus.saveTransaction( rockContext, backgroundCheck, xTransaction );
                            org.newpointe.ProtectMyMinistry.ProtectMyMinistryPlus.handleTransaction( rockContext, backgroundCheck, xTransaction );

                            rockContext.WrapTransaction( () =>
                            {
                                rockContext.SaveChanges();
                                workflow.SaveAttributeValues( rockContext );
                                foreach ( var activity in workflow.Activities )
                                {
                                    activity.SaveAttributeValues( rockContext );
                                }
                            } );

                        }
                    }
                    else
                    {
                        ExceptionLogService.LogException( new ArgumentException( "Could not find OrderId." ), context );
                    }

                    response.StatusCode = 200;
                    response.ContentType = "text/xml";
                    response.AddHeader( "Content-Type", "text/xml" );
                    xdocResult.Save( response.OutputStream );

                }
                catch ( SystemException ex )
                {
                    ExceptionLogService.LogException( ex, context );
                }
            }
            else
            {
                ExceptionLogService.LogException( new ArgumentException( "Could not find 'REQUEST' parameter." ), context );
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}