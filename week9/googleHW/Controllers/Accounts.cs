using System.Data.SqlClient;
using System.Net;
using googleHW.Attributes;
using googleHW.Models;

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
    public List<Account> getAccounts()
    {
        var rep = new AccountRepository(connectionString);
        return rep.GetAccountList();
    }
    
    [HttpGET($"")]
    public Account? GetAccountById(int id)
    {
        var rep = new AccountRepository(connectionString);
        return rep.GetAccount(id);
    }
    
    [HttpPOST("")]
    public bool Login(HttpListenerContext listener, string name, string password)
    {
        var rep = new AccountRepository(connectionString);
        return !(rep.GetAccount(name) is null);
    }


    [HttpPOST("save")]
    public void SaveAccount(HttpListenerContext listener)
    {
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var Params = bodyParam.Split("&");
        var name = Params[0].Split("=")[1];
        var password = Params[1].Split("=")[1];
        
        var rep = new AccountRepository(connectionString);
        rep.Insert(new Account(0, name, password));
        
        listener.Response.Redirect(@"https://steamcommunity.com/login/home/");
    }
}