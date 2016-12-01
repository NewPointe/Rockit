using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Rest;
using Rock.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using Rock.Web.Cache;
using System.Data.SqlClient;

using System.Net.Http.Headers;
 
using Rock.Constants;
 
using Rock.Security;
using Rock.Web;
 
using Rock.Web.UI;
using Rock.Web.UI.Controls;


using RestSharp;


/// <summary>
/// This is a Rock API wrapper of sorts.. It was needed since we didn't want to unlock the apis for security reasons and we didn't
/// want to store the api key in the app in case it needed to be changed. 
/// So the app hits these, then these hit the rock stuff using the api key.
/// </summary>
public class MobileAppPrayerController : ApiControllerBase
{
    /// <summary>
    /// Post method to send a prayer
    /// </summary>
    /// <param name="prayer"></param>
    /// <returns></returns>
    public HttpResponseMessage Post(PrayerPost prayer)
    {
        //verify the token passed from the app is valid. Just an extra security measure tp make sure they're hitting from the app.
        var isAuthed = MobileAppAPIHelper.ValidateAppToken(Request);

        if (!isAuthed)
            return Request.CreateResponse(HttpStatusCode.Unauthorized);

        prayer.IsApproved = true;
        prayer.EnteredDateTime = DateTime.Now;

        RestClient restClient = new RestClient(ApiUrls.BaseUrl);
        IRestRequest patch = new RestRequest();

        var request = new RestRequest(string.Format(ApiUrls.PrayerPostUrl), Method.POST);

        //this is the api key generated in rock backend
        request.AddHeader("Authorization-Token", MobileAppAPIHelper.APIKey());

        request.AddJsonBody(prayer);
        var result = restClient.Execute(request);

        if (result.StatusCode == HttpStatusCode.Created && result.ResponseStatus == ResponseStatus.Completed)
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Success");
        }
        else
        {
            return Request.CreateResponse(result.StatusCode, result.StatusDescription);
        }

    } 
}

[Authorize]
public class MobileAppProfileController : ApiControllerBase
{

    /// <summary>
    /// Get Method for pulling a user profile
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage Get()
    {
        //verify the token passed from the app is valid. Just an extra security measure tp make sure they're hitting from the app.
        var isAuthed = MobileAppAPIHelper.ValidateAppToken(Request);

        //if this check fails, return Unauthorized
        if (!isAuthed)
            return Request.CreateResponse(HttpStatusCode.Unauthorized);

        //get the authenticated user (cookie should have been passed in header, the [Authorize] makes sure it's valid.)
        //make sure we can pull what we need from it, and find it by the username
        var rockContext = new RockContext();
        var u = new UserLoginService(rockContext);
        var userProfileController = new MobileAppProfileController();

        var p = userProfileController.User;
        if (p == null)
            return Request.CreateResponse(HttpStatusCode.Unauthorized);

        var user = u.GetByUserName(p.Identity.Name);

        if (user == null)
            return Request.CreateResponse(HttpStatusCode.NotFound);


        //if we got here, we can build the return object.
        var m = new UserProfileGet();
        try
        {
            m.Id = user.Person.Id;

            m.FirstName = user.Person.FirstName;
            m.LastName = user.Person.LastName;
            m.MiddleName = user.Person.MiddleName;
            m.Email = user.Person.Email;
            m.EmailPreference = user.Person.EmailPreference.ConvertToInt();
            m.AnniversaryDate = user.Person.AnniversaryDate.HasValue ? user.Person.AnniversaryDate.Value.ToShortDateString() : "";
            m.BirthDay = user.Person.BirthDay.HasValue ? user.Person.BirthDay.Value.ToStringSafe() : "";
            m.BirthMonth = user.Person.BirthMonth.HasValue ? user.Person.BirthMonth.Value.ToStringSafe() : "";
            m.BirthYear = user.Person.BirthYear.HasValue ? user.Person.BirthYear.Value.ToStringSafe() : "";
            m.GraduationYear = user.Person.GraduationYear.HasValue ? user.Person.GraduationYear.Value.ToStringSafe() : "";
            m.Gender = user.Person.Gender.ConvertToInt();
            m.EncodedUrl = user.Person.UrlEncodedKey;

            //and return ok.
            return Request.CreateResponse(HttpStatusCode.OK, m);
        }
        catch (Exception ex)
        {
            //todo: log the error somewhere. 
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// Patch method for updating a user profile
    /// </summary>
    /// <param name="Id">Rock Id</param>
    /// <param name="person">Patch Object</param>
    /// <returns></returns>
    public HttpResponseMessage Patch(int Id, UserProfilePatch person)
    {
        //verify the token passed from the app is valid. Just an extra security measure tp make sure they're hitting from the app.
        var isAuthed = MobileAppAPIHelper.ValidateAppToken(Request);

        if (!isAuthed)
            return Request.CreateResponse(HttpStatusCode.Unauthorized);


        RestClient restClient = new RestClient(ApiUrls.BaseUrl);
        IRestRequest patch = new RestRequest();

        var request = new RestRequest(string.Format(ApiUrls.UserPatchUrl, Id), Method.PATCH);

        //this is the api key generated in rock backend
        request.AddHeader("Authorization-Token", MobileAppAPIHelper.APIKey());

        request.AddJsonBody(person);
        var result = restClient.Execute(request);

        if (result.StatusCode == HttpStatusCode.NoContent && result.ResponseStatus == ResponseStatus.Completed)
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Success");
        }
        else
        {
            return Request.CreateResponse(result.StatusCode, result.StatusDescription);
        }

    }
 
}



public class MobileAppAboutController : ApiControllerBase
{

    /// <summary>
    /// Get Method for pulling the about us text
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage Get()
    {
        //verify the token passed from the app is valid. Just an extra security measure tp make sure they're hitting from the app.
        var isAuthed = MobileAppAPIHelper.ValidateAppToken(Request);

        //if this check fails, return Unauthorized
        if (!isAuthed)
            return Request.CreateResponse(HttpStatusCode.Unauthorized);

        try
        {
            var aboutText = Rock.Web.Cache.GlobalAttributesCache.Value("AppAboutUs");
            //and return ok.
            return Request.CreateResponse(HttpStatusCode.OK, aboutText);
        }
        catch (Exception ex) 
        {
            //todo: log the error somewhere. 
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
      
}

public class MobileAppLoginController : ApiControllerBase
{
    /// <summary>
    /// Get Method for logging the user in.
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage Post(LoginPost login)
    {
        //verify the token passed from the app is valid. Just an extra security measure tp make sure they're hitting from the app.
        var isAuthed = MobileAppAPIHelper.ValidateAppToken(Request);

        //if this check fails, return Unauthorized
        if (!isAuthed)
            return Request.CreateResponse(HttpStatusCode.Unauthorized);

        try
        {

            RestClient restClient = new RestClient(ApiUrls.BaseUrl);
 

            var request = new RestRequest(string.Format(ApiUrls.LoginPostUrl), Method.POST);
 
            request.AddJsonBody(login);
            var result = restClient.Execute(request);

            if (result.StatusCode == HttpStatusCode.NoContent && result.ResponseStatus == ResponseStatus.Completed)
            {
                var obj = new LoginReturnObject();
                obj.Cookie = result.Cookies[0].Name + "=" + result.Cookies[0].Value;
            
                

                var rc = new RockContext();
                var u = new UserLoginService(rc).GetByUserName(login.Username);
                 
                if (u == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                obj.Username = u.UserName;
                obj.EncodedUrl = u.UrlEncodedKey;

                u.Person.LoadAttributes();

                u.Person.SetAttributeValue("AppPlatform", login.Platform);
                u.Person.SaveAttributeValues();

                return Request.CreateResponse(HttpStatusCode.OK, obj);
            }
            else
            {
                return Request.CreateResponse(result.StatusCode, result.StatusDescription);
            }
 
        }
        catch (Exception ex)
        {
            //todo: log the error somewhere. 
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

}


public class MobileAppRegisterController : ApiControllerBase
{
    /// <summary>
    /// Get Method for logging the user in.
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage Post(RegisterPost register)
    {
        //verify the token passed from the app is valid. Just an extra security measure tp make sure they're hitting from the app.
        var isAuthed = MobileAppAPIHelper.ValidateAppToken(Request);

        //if this check fails, return Unauthorized
        if (!isAuthed)
            return Request.CreateResponse(HttpStatusCode.Unauthorized);

        try
        {

 
            var rockContext = new RockContext();
            var personService = new PersonService(rockContext);


            if (new UserLoginService(rockContext).GetByUserName(register.Username.Trim()) != null)
            {
                //check the username
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The selected username is already taken.");
            }


            if (!UserLoginService.IsPasswordValid(register.Password.Trim()))
            {
                //check the password.
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Password is invalid.");
            }





            Person person = null;
            Person spouse = null;
            Group family = null;
            GroupLocation homeLocation = null;

            var changes = new List<string>();
            var spouseChanges = new List<string>();
            var familyChanges = new List<string>();



            // Try to find person by name/email 
            if (person == null)
            {
                var matches = personService.GetByMatch(register.FirstName.Trim(), register.LastName.Trim(), register.Email.Trim());
                if (matches.Count() == 1)
                {
                    person = matches.First();
                }
            }

            // Check to see if this is a new person
            if (person == null)
            {
                // If so, create the person and family record for the new person
                person = new Person();
                person.FirstName = register.FirstName.Trim();
                person.LastName = register.LastName.Trim();
                person.Email = register.Email.Trim();

                person.IsEmailActive = true;
                person.EmailPreference = EmailPreference.EmailAllowed;
                person.RecordTypeValueId = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid()).Id;


                
        

            //    person.ConnectionStatusValueId = DefinedValueCache.Read(GetAttributeValue("ConnectionStatus").AsGuid()).Id;
              //  person.RecordStatusValueId = DefinedValueCache.Read(GetAttributeValue("RecordStatus").AsGuid());

                person.Gender = Gender.Unknown;

                family = PersonService.SaveNewPerson(person, rockContext, null, false);
            }


            if (person != null)
            {
                var user = UserLoginService.Create(
                rockContext,
                person,
                Rock.Model.AuthenticationServiceType.Internal,
                EntityTypeCache.Read(Rock.SystemGuid.EntityType.AUTHENTICATION_DATABASE.AsGuid()).Id,
                register.Username.Trim(),
                register.Password.Trim(),
                true);



                //var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields(RockPage);
                //mergeObjects.Add("ConfirmAccountUrl", RootPath + "ConfirmAccount");

                //var personDictionary = person.ToLiquid() as Dictionary<string, object>;
                //mergeObjects.Add("Person", personDictionary);

                //mergeObjects.Add("User", user);

                //var recipients = new List<Rock.Communication.RecipientData>();
                //recipients.Add(new Rock.Communication.RecipientData(person.Email,
                //    mergeObjects));

                //Rock.Communication.Email.Send(GetAttributeValue("ConfirmAccountTemplate").AsGuid(),
                //    recipients, ResolveRockUrl("~/"), ResolveRockUrl("~~/"), false);

            }

            
            return Request.CreateResponse(HttpStatusCode.OK, "SUccess");
  

        }
        catch (Exception ex)
        {
            //todo: log the error somewhere. 
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

}


public class MobileAppBackgroundController : ApiControllerBase
{

    /// <summary>
    /// Get Method for checking the background image
    /// </summary>
    /// <returns></returns>
    public HttpResponseMessage Get()
    {
        //verify the token passed from the app is valid. Just an extra security measure tp make sure they're hitting from the app.
        var isAuthed = MobileAppAPIHelper.ValidateAppToken(Request);

        //if this check fails, return Unauthorized
        if (!isAuthed)
            return Request.CreateResponse(HttpStatusCode.Unauthorized);

        try
        {
            var version = Rock.Web.Cache.GlobalAttributesCache.Value("AppBackgroundVersion");
            //and return ok.
            return Request.CreateResponse(HttpStatusCode.OK, version);
        }
        catch (Exception ex)
        {
            //todo: log the error somewhere. 
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

}



#region Custom API Stuff

public class ApiUrls
{
    public static string BaseUrl = "http://newpointe.org/api/";

    public static string UserPatchUrl = "/People/{0}";
    public static string PrayerPostUrl = "/PrayerRequests";
    public static string LoginPostUrl = "/auth/login";
}

public class MobileAppAPIHelper
{ 
    /// <summary>
    /// this just verifies (loosely) that the request came from the app. By matching the key stored in the app's code. 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private static bool ValidateToken(string token)
    {
        //this is stored in the app, so don't change it. Otherwise the app won't work until it's updated.
        return token == "rKEaetxPmqe55LjoWy7lXgCo";
    }

    public static bool ValidateAppToken(HttpRequestMessage request)
    {
        var isAuthed = false;
        string token = "";
        IEnumerable<string> headerValues;

        if (request.Headers.TryGetValues("appToken", out headerValues))
        {
            token = headerValues.First();
            if (ValidateToken(token))
            {
                //yay, continue doing what you're doing.
                isAuthed = true;
            }
        }

        return isAuthed;
    }


    /// <summary>
    /// This will pull the api key generated in rock. The name of it needs to be AppRestKey.
    /// It can be changed whenever from the website, this function will pull it based on the name in case the key becomes compromised
    /// if it can't find it, it will return empty string and the request will return unauthorized
    /// </summary>
    /// <returns></returns>
    public static string APIKey()
    {
        var rc = new RockContext();
        var u = new PersonService(rc).GetByFullName("AppRestKey", false);
        var key = "";

        if (u.Count() > 0)
        {
            var restUser = u.First();

            var userLoginService = new UserLoginService(rc);
            var userLogin = userLoginService.Queryable().Where(a => a.PersonId == restUser.Id).FirstOrDefault();

            if (userLogin != null)
            {
                key = userLogin.ApiKey;
            }

        }

        return key;
    }
}

#endregion


#region "Get/Post/Patch objects."

public class UserProfileGet
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }


    public string BirthDay { get; set; }
    public string BirthMonth { get; set; }
    public string BirthYear { get; set; }
    public int Gender { get; set; }

    public string AnniversaryDate { get; set; }
    public string GraduationYear { get; set; }
    public string Email { get; set; }

    //    public string CellPhone { get; set; }

    /// <summary>
    /// 0 - Email Allowed,  1 - No Mass Emails,  2 - Do Not Email
    /// </summary>
    public int EmailPreference { get; set; }


    public string EncodedUrl { get; set; }
}

public class UserProfilePatch
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }


    public string BirthDay { get; set; }
    public string BirthMonth { get; set; }
    public string BirthYear { get; set; }
    public int Gender { get; set; }

    public string AnniversaryDate { get; set; }
    public string GraduationYear { get; set; }
    public string Email { get; set; }

    //    public string CellPhone { get; set; }

    /// <summary>
    /// 0 - Email Allowed,  1 - No Mass Emails,  2 - Do Not Email
    /// </summary>
    public int EmailPreference { get; set; }

}

public class PrayerPost
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Text { get; set; }
    public DateTime? EnteredDateTime { get; set; }
    public bool? IsApproved { get; set; }
}

public class LoginPost
{  
    public string Username { get; set; }
    public string Password { get; set; }
    public bool Persited { get; set; }
    public string Platform { get; set; }
}

public class RegisterPost
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

public class LoginReturnObject
{
    public string Username { get; set; }
    public string Cookie { get; set; }
    public string EncodedUrl { get; set; }
}


#endregion