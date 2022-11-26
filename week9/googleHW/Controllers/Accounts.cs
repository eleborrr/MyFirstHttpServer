using System.Data.SqlClient;
using System.Net;
using googleHW.Attributes;
using googleHW.Models;
using System.Web;

namespace googleHW.Controllers;

[HttpController("accounts")]
public class Accounts
{
    //Get /accounts/ - spisok accauntov
    //Get /accounts/{id} - account
    //POST /accounts - dobavl9t infu na server cherez body

    string connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;";


    [HttpGET("list")]
    public List<Account>? getAccounts(HttpListenerContext listener)  //
    {
        var cookie = listener.Request.Cookies["SessionId"];
        if (cookie is null)
        {
            listener.Response.StatusCode = 401;
            return null;
        }

        var cookieVal = cookie.Value.Split("@").ToList();
        if(!CheckCookie(cookieVal, "IsAuthorized", "true"))
        {
            listener.Response.StatusCode = 401;
            return null;
        }
        var rep = new AccountRepository(connectionString);
        return rep.GetAccountList();
    }


    [HttpGET($"")]
    public Account? GetAccountById(HttpListenerContext listener)
    {
        int id = int.Parse(listener.Request.RawUrl.Split("/").LastOrDefault());
        var rep = new AccountRepository(connectionString);
        return rep.GetAccount(id);
    }
    
    [HttpGET($"")]
    public Account? GetAccountInfo(HttpListenerContext listener)
    {
        var cookie = listener.Request.Cookies["SessionId"];
        if (cookie is null)
        {
            listener.Response.StatusCode = 401;
            return null;
        }
        var cookieVal = cookie.Value.Split("@").ToList();
        if(!CheckCookie(cookieVal, "IsAuthorized", "True"))
        {
            listener.Response.StatusCode = 401;
            return null;
        }

        if (!cookieVal.Contains("Id"))
        {
            listener.Response.StatusCode = 401;
            return null;
        }

        var id = int.Parse(GetCookieVal(cookieVal, "Id"));
        var rep = new AccountRepository(connectionString);
        return rep.GetAccount(id);
    }
    //
    // [HttpPOST("")]
    // public bool Login(HttpListenerContext listener, string name, string password)
    // {
    //     var rep = new AccountRepository(connectionString);
    //     var acc = rep.GetAccount(name);
    //     if (acc is null)
    //         return false;
    //     listener.Response.AddHeader("Set-Cookie", $"SessionID= IsAuthorize = true, Id = {acc.Id} ");
    //     return true;
    // }
    


    [HttpPOST("save")]
    public void SaveAccount(HttpListenerContext listener)
    {
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var Params = bodyParam.Split("&");
        var name = Params[0].Split("=")[1];
        var password = Params[1].Split("=")[1];
        var rep = new AccountRepository(connectionString);
        var acc = rep.GetAccount(name, password);
        // cookie.Value = new string[]{ "IsAuthorize = true", "Id = { rep.GetAccount(name, password).Id}" };
        if (acc is null)
        {
            rep.Insert(new Account(0, name, password));
            // // listener.Response.Cookies["SessionId"]["1"] = "IsAuthorized = true";
            // listener.Response.AddHeader("Set-Cookie", $"SessionID= IsAuthorize = true . Id = { rep.GetAccount(name, password).Id}");
            var cookie = new Cookie("SessionId", $"IsAuthorized={true} @ Id={rep.GetAccount(name, password).Id}");
            listener.Response.SetCookie(cookie);
        }

        // listener.Response.Redirect(@"https://steamcommunity.com/login/home/");
    }
    
    private bool CheckCookie(List<string> values,string needKey, string needValue)
    {
        foreach (var value in values)
        {
            var key= value.Split("=")[0];
            var val= value.Split("=")[1];
            if (val == needKey && val == needValue)
                return true;
        }
        return false;
    }

    private string? GetCookieVal(List<string> values, string needKey)
    {
        var cookie = values.Where(s => s.Split("=")[0] == needKey).FirstOrDefault();
        if (cookie is null)
            return null;
        return cookie.Split("=")[1];
    }
}