<%@ WebHandler Language="C#" Class="SmsGiveBot" %>

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
using Rock.Communication;


/**
 * SMS Giving Bot
 * Author: Tyler Schrock <Tschrock123@gmail.com>
 * 
 */

public class SmsGiveBot : IHttpHandler
{

    //
    //  HTTP Handling
    //

    private HttpRequest request;
    private HttpResponse response;

    public void ProcessRequest(HttpContext context)
    {
        request = context.Request;
        response = context.Response;
        response.ContentType = "text/plain";

        if (request.HttpMethod != "POST")
        {
            response.Write("Invalid request type.");
            return;
        }

        switch (request.Form["SmsStatus"])
        {
            case "received":
                MessageRecieved();
                break;
            case "undelivered":
                MessageUndelivered();
                break;
        }

        response.StatusCode = request.Form["SmsStatus"] != null ? 200 : 500;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }


    //
    // SMS Handling
    //

    private void MessageRecieved()
    {
        toPhone = request.Form["To"] ?? String.Empty;
        fromPhone = request.Form["From"] ?? String.Empty;
        body = request.Form["Body"] ?? String.Empty;

        WriteToLog(fromPhone, toPhone, body);

        if (!(string.IsNullOrWhiteSpace(toPhone)) && !(string.IsNullOrWhiteSpace(fromPhone)))
        {
            handleSms();
        }
    }

    private void MessageUndelivered()
    {
        WriteToLog("Message Undelivered", request.Form["To"], request.Form["From"], request.Form["Body"]);
    }


    //
    // Giving SMS Handling
    //

    string toPhone;
    string fromPhone;
    string body;

    Person person;
    FinancialAccount account;

    int currstate = 0;
    decimal? amount;
    bool confirm = false;
    int number = -1;
    FinancialPersonSavedAccount ccard = null;

    DateTime? lastReplyDate = null;
    bool newPerson = false;

    const String newPersonGreeting = "Welcome to Automated SMS Giving! ";

    private void handleSms()
    {
        if (lookupPerson())
        {
            loadPersistedData();
            if (!tryHandleCommand())
            {
                newPerson = lastReplyDate == null;
                if (((lastReplyDate ?? DateTime.MinValue) - DateTime.Now).Hours > 2)
                {
                    currstate = (int)State.NONE;
                }
                lastReplyDate = DateTime.Now;
                
                if (currstate == (int)State.TRANSACTION_CONFIRM)
                {
                    if (!parseMessageForConfirm())
                    {
                        parseMessageForAmount();
                    }
                    confirmDonation_return();
                }
                else if (currstate == (int)State.CREDIT_CARD)
                {
                    parseMessageForNumber();
                    getCreditCard(number);
                }
                else if (currstate == (int)State.NONE)
                {
                    if (parseMessageForAmount())
                    {
                        getCreditCard(number);
                    }
                    else
                    {
                        replySms("How much would you like to give today?", true);
                    }
                }
                else
                {
                    replySms("Error: Invalid internal state! :(");
                    currstate = (int)State.NONE;
                }
                savePersistedData();
            }
        }
    }
    
    private bool lookupPerson()
    {
        // Lookup person by phone #
        List<Person> results = (new PersonService(new RockContext())).Queryable().Where(x => x.PhoneNumbers.Select(p => p.Number).Contains(fromPhone.Substring(1))).ToList();
        
        if (results.Count == 1)
        {
            person = results.FirstOrDefault();
            if (person.Users.Count > 0)
            {
                return true; // Yay! Got a single valid person!
            }
            else
            {
                replySms("We can't find your MyNewPointe account. Please visit https://newpointe.org/NewAccount?pn=" + HttpUtility.UrlEncode(fromPhone) + " to set one up.");
            }
        }
        else if (results.Count == 0)
        {
            replySms("We don't have this phone number on record. Visit https://newpointe.org/NewAccount?pn=" + HttpUtility.UrlEncode(fromPhone) + " to set up your account.");
        }
        else
        {
            replySms("There's multiple people linked to this number. To maintain security, please visit https://NPGive.org to make a donation.");
        }
        return false;
    }

    private bool tryHandleCommand()
    {
        switch (body.ToLower().Split(' ')[0])
        {
            case "help":
                replySms("Try 'HELP' :)");
                return true;
            case "stop":
                replySms("Try 'STOP' :)");
                return true;
            default:
                return false;
        }
    }

    public void getCreditCard(int number)
    {
        var cards = (new FinancialPersonSavedAccountService(new RockContext())).Queryable().Where(x => x.PersonAliasId == person.PrimaryAliasId).ToList();
        if (cards.Count == 0)
        {
            replySms("You don't have any stored payment methods. Visit https://newpointe.org/GiveNow?pn=" + HttpUtility.UrlEncode(fromPhone) + " to make a donation.");
        }
        else if (cards.Count == 1)
        {
            ccard = cards.FirstOrDefault();
            getCreditCard_return();
        }
        else if (cards.Count > 1 && number > 0 && number <= cards.Count)
        {
            ccard = cards.ElementAt(number - 1);
            getCreditCard_return();
        }
        else
        {
            replyPrompt((int)State.CREDIT_CARD, "You Have multipe saved accounts. Which one?\n" + String.Join("\n", cards.Select((x, i) => (i + 1) + ". " + x.Name)));
        }
    }

    private void getCreditCard_return()
    {
        if (amount == null || account == null)
        {
            replySms("Invalid amount or account name.");
        }
        else if (amount <= 0)
        {
            replySms("Amount must be more than $0.");
        }
        else
        {
            replyPrompt((int)State.TRANSACTION_CONFIRM, "Charge $" + amount + " to your card (" + ccard.Name + ")? (Reply 'Yes' or 'No')");
        }
    }

    private void confirmDonation_return()
    {
        String txt = body.Trim().ToLower();
        if (txt == "yes")
        {
            // Charge Account

            account = null;
            ccard = null;
            amount = -1;
            currstate = 0;
            savePersistedData();
            replySms("Thank you for your donation! :)");
        }
        else if (txt == "no")
        {
            account = null;
            ccard = null;
            amount = -1;
            currstate = 0;
            savePersistedData();
            replySms("Ok, transaction canceled. :(");
        }
        else
        {
            getCreditCard_return();
        }
    }

    //
    // Utility
    //

    // General
    
    private void WriteToLog(string fromPhone, string toPhone, string body)
    {
        WriteToLog("", fromPhone, toPhone, body);
    }

    private void WriteToLog(string note, string fromPhone, string toPhone, string body)
    {
        WriteToLog(string.Format("{0} -- From: '{1}', To: '{2}', Message: '{3}'", note, fromPhone, toPhone, body));
    }
    private void WriteToLog(string message)
    {
        string logFile = HttpContext.Current.Server.MapPath("~/App_Data/Logs/Twilio-Text2Give-Log.txt");

        using (System.IO.FileStream fs = new System.IO.FileStream(logFile, System.IO.FileMode.Append, System.IO.FileAccess.Write))
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
        {
            sw.WriteLine(string.Format("{0} -- {1}", RockDateTime.Now.ToString(), message));
        }
    }
    
    // Sms
    private void replySms(string msg)
    {
        replySms(msg, newPerson);
    }
    
    private void replySms(string msg, bool includeGreeting)
    {
        if (includeGreeting && msg.Length + newPersonGreeting.Length > 160)
        {
            replySms(newPersonGreeting, false);
        }
        else
        {
            msg = (includeGreeting ? newPersonGreeting : "") + msg;
        }
        
        if (msg.Length > 160)
        {
            WriteToLog("Warning: message longer than 160 characters!");
        }
        
        WriteToLog(toPhone, fromPhone, "Reply: " + msg);
        return;

        RockContext rContext = new RockContext();

        var mediumData = new Dictionary<string, string>();
        mediumData.Add("FromValue", toPhone);
        mediumData.Add("Message", msg);

        var recipients = new List<string>();
        recipients.Add(fromPhone);

        var mediumEntity = EntityTypeCache.Read(Rock.SystemGuid.EntityType.COMMUNICATION_MEDIUM_SMS.AsGuid(), rContext);
        if (mediumEntity != null)
        {
            var medium = MediumContainer.GetComponent(mediumEntity.Name);
            if (medium != null && medium.IsActive)
            {
                var transport = medium.Transport;
                if (transport != null && transport.IsActive)
                {
                    var appRoot = GlobalAttributesCache.Read(rContext).GetValue("InternalApplicationRoot");
                    transport.Send(mediumData, recipients, appRoot, string.Empty);
                }
            }
        }
    }

    private void replyPrompt(int stateId, string msg)
    {
        replyPrompt(stateId, msg, newPerson);
    }
    
    private void replyPrompt(int stateId, string msg, bool includeGreeting)
    {
        currstate = stateId;
        replySms(msg, includeGreeting);
    }
    
    // Giving

    private enum State
    {
        NONE = 0,
        CREDIT_CARD = 1,
        TRANSACTION_CONFIRM = 2
    }

    private bool parseMessageForAmount()
    {
        String[] txt = body.Trim().ToLower().Split(' ');

        if (txt.Length == 1)
        {
            String accountTxt = "DOV General Donation";
            decimal tmp;
            if (decimal.TryParse(txt[0].Replace("$", ""), out tmp))
            {
                account = getAccount(accountTxt);
                amount = tmp;
                return true;
            }
        }
        else if (txt.Length > 1)
        {
            String accountTxt = "";
            for (int i = 0; i < txt.Length - 1; i++)
            {
                accountTxt += txt[i] + " ";
            }
            accountTxt = accountTxt.Trim();
            decimal tmp;
            if (decimal.TryParse(txt[txt.Length - 1].Replace("$", ""), out tmp))
            {
                account = getAccount(accountTxt);
                amount = tmp;
                return true;
            }
        }
        return false;
    }
    
    private bool parseMessageForConfirm()
    {
        String txt = body.Trim().ToLower();
        if (txt == "yes")
        {
            confirm = true;
            return true;
        }
        if (txt == "no")
        {
            confirm = false;
            return true;
        }
        return false;
    }
    
    private bool parseMessageForNumber()
    {
        int choice = -1;
        if (int.TryParse(body.Trim(), out choice))
        {
            number = choice;
            return true;
        }
        return false;
    }

    // Persistance
    
    private void savePersistedData()
    {
        if (person != null)
        {
            setAttVal("SMS_GIVE_ACCOUNT", 15, 38, person.PrimaryAlias.Person.Id, account != null ? account.Guid.ToString() : "");
            setAttVal("SMS_GIVE_STATE", 15, 7, person.PrimaryAlias.Person.Id, currstate.ToString());
            setAttVal("SMS_GIVE_AMMOUNT", 15, 7, person.PrimaryAlias.Person.Id, ((int)(amount * 100)).ToString());
            setAttVal("SMS_GIVE_CARD", 15, 7, person.PrimaryAlias.Person.Id, ccard != null ? ccard.Id.ToString() : "");
            setAttVal("SMS_GIVE_LASTREPLYDATETIME", 15, 11, person.PrimaryAlias.Person.Id, lastReplyDate != null ? (lastReplyDate ?? DateTime.MinValue).ToString("o") : "");
        }
    }
    
    private void loadPersistedData()
    {
        if (person != null)
        {
            var rc = new RockContext();
            
            var cctGuidS = getAttVal("SMS_GIVE_ACCOUNT", person.PrimaryAlias.Person.Id, "");
            account = (new FinancialAccountService(rc)).Queryable().Where(x => x.Guid.ToString() == cctGuidS).FirstOrDefault();
            
            currstate = int.TryParse(getAttVal("SMS_GIVE_STATE", person.PrimaryAlias.Person.Id, "0"), out currstate) ? currstate : 0;
            
            int amounttmp = int.TryParse(getAttVal("SMS_GIVE_AMMOUNT", person.PrimaryAlias.Person.Id, "0"), out amounttmp) ? amounttmp : 0;
            amount = ((decimal)amounttmp) / 100;
            
            var ccdIdS = getAttVal("SMS_GIVE_CARD", person.PrimaryAlias.Person.Id, "");
            ccard = (new FinancialPersonSavedAccountService(rc)).Queryable().Where(x => x.Id.ToString() == ccdIdS).FirstOrDefault();

            DateTime lrd;
            if (DateTime.TryParse(getAttVal("SMS_GIVE_LASTREPLYDATETIME", person.PrimaryAlias.Person.Id, ""), out lrd))
            {
                lastReplyDate = lrd;
            }
            else
            {
                lastReplyDate = null;
            }
        }
    }
    
    private String getAttVal(String attKey, int entityId, String defaultValue)
    {
        var rc = new RockContext();
        var ats = new AttributeService(rc);
        var argsd = ats.Queryable().Where(x => x.Key == attKey).FirstOrDefault();
        if (argsd != null)
        {
            var atvs = new AttributeValueService(rc);
            var argsdVal = atvs.Queryable().Where(x => x.AttributeId == argsd.Id && x.EntityId == entityId).FirstOrDefault();
            if (argsdVal != null)
            {
                return argsdVal.Value;
            }
        }
        return null;
    }

    private void setAttVal(String attKey, int entityTypeId, int fieldTypeId, int entityId, String attValue)
    {
        var rc = new RockContext();
        var ats = new AttributeService(rc);
        var argsd = ats.Queryable().Where(x => x.Key == attKey).FirstOrDefault();
        if (argsd == null)
        {
            argsd = new Rock.Model.Attribute();
            argsd.FieldTypeId = fieldTypeId;
            argsd.EntityTypeId = entityTypeId;
            argsd.Key = attKey;
            argsd.Name = attKey;
            argsd.Guid = Guid.NewGuid();
            argsd.CreatedDateTime = argsd.ModifiedDateTime = DateTime.Now;
            ats.Add(argsd);
            rc.SaveChanges();
            rc = new RockContext();
            ats = new AttributeService(rc);
            argsd = ats.Queryable().Where(x => x.Key == attKey).FirstOrDefault();
        }
        if (argsd != null)
        {
            var atvs = new AttributeValueService(rc);
            var argsdVal = atvs.Queryable().Where(x => x.AttributeId == argsd.Id && x.EntityId == entityId).FirstOrDefault();
            if (argsdVal == null)
            {
                argsdVal = new Rock.Model.AttributeValue();
                argsdVal.AttributeId = argsd.Id;
                argsdVal.EntityId = entityId;
                argsdVal.Value = attValue.ToString();
                argsdVal.Guid = Guid.NewGuid();
                argsdVal.CreatedDateTime = argsdVal.ModifiedDateTime = DateTime.Now;

                atvs.Add(argsdVal);
                rc.SaveChanges();
            }
            else
            {
                argsdVal.Value = attValue.ToString();
                rc.SaveChanges();
            }
        }
    }

    private FinancialAccount getAccount(string acctName)
    {
        return (new FinancialAccountService(new RockContext())).Queryable().Where(x => x.Name == acctName).FirstOrDefault();
    }




}