using System.Data.SqlClient;
using googleHW.Models;

namespace googleHW;

public class AccountRepository
{
    private List<Account> _accounts = new List<Account>();
    private string connectionString;
    
    public AccountRepository(string connectionString)
    {
        this.connectionString = connectionString;
        Update();
    }

    public List<Account> GetAccountList() // получение всех объектов
    {
        return _accounts;
    }

    public Account? GetAccount(int id) // получение одного объекта по id
    {
        return _accounts.Find(acc => acc.Id == id);
    }
    
    public Account? GetAccount(string name, string password) // получение одного объекта по id
    {
        return _accounts.Find(acc => acc.Name == name && acc.Password == password);
    }

    public void Insert(Account acc) // создание объекта
    {
        var queryString = $"INSERT INTO Accounts (Name, Password) VALUES (\'{acc.Name}\', \'{acc.Password}\')";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand(queryString, connection);
            command.ExecuteNonQuery();
        }
        Update();
    }

    void Update() // обновление объекта
    {
        string sqlExpression = "SELECT * FROM Accounts";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
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
                    _accounts.Add(new Account(id, name, password));
                }
            }

            reader.Close();
        }
    }
    
}