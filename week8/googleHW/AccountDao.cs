using System.Data.SqlClient;
using googleHW.Models;

namespace googleHW;

public class AccountDao
{
    private readonly string connectionString;

    public AccountDao(string connectionString)
    {
        this.connectionString = connectionString;
    }
    
    List<Account> GetAccountList() // получение всех объектов
    {
        var queryString = "SELECT * FROM Accounts";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            var result = new List<Account>();
            var command = new SqlCommand(queryString, connection);
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

    Account? GetAccount(int id) // получение одного объекта по id
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

    void Insert(Account acc) // создание объекта
    {
        var queryString = $"SELECT INTO Accounts (Id, Name, Password) VALUES ({acc.Id}, {acc.Name}, {acc.Password})";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand(queryString, connection);
            command.ExecuteNonQuery();
        }
    }
}