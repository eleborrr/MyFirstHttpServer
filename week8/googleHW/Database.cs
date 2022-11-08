using System.Data;
using System.Data.SqlClient;
using googleHW.Interfaces;
using googleHW.Models;

namespace googleHW;

public class Database
{
    public IDbConnection Connection = null;
    public IDbCommand cmd = null;
    string _connectionString =
        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;";

    public Database()
    {
        Connection = new SqlConnection(_connectionString);
        cmd = Connection.CreateCommand();
    }

    public Database AddParameter<T>(string name, T value)
    {
        SqlParameter parameter = new SqlParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        cmd.Parameters.Add(parameter);
        return this;
    }

    public int ExecuteNonQuery(string query, bool isStoredProcedure=false)
    {
        int noOfAffectedRows = 0;

        using (Connection)
        {
            if (isStoredProcedure)
                cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = query;
            Connection.Open();
            noOfAffectedRows = cmd.ExecuteNonQuery();
        }

        return noOfAffectedRows;
    }
    

    public IEnumerable<T> ExecuteQuery<T>(string query, bool isStoredProcedure=false)
    {
        List<T> list = new List<T>();
        Type t = typeof(T);

        using (Connection)
        {
            if (isStoredProcedure)
                cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = query;
            Connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                T obj = (T)Activator.CreateInstance(t);
                t.GetProperties().ToList().ForEach(p =>
                {
                    p.SetValue(obj, reader[p.Name]);
                });
                
                list.Add(obj);
            }
        }

        return list;
    }

    public T? ExecuteScalar<T>(string query, bool isStoredProcedure=false)
    {
        T? result = default(T);
        using (Connection)
        {
            if (isStoredProcedure)
                cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = query;
            Connection.Open();
            result = (T?)cmd.ExecuteScalar();
        }
        return result;
    }
    
    public IEnumerable<T> Select<T>(string[]? args=null) where T: IModel
    {
        var queryString = $"SELECT {String.Join(", ", args)} FROM Accounts"; // if args == null
        var result = ExecuteQuery<T>(queryString);
        return result;
    }

    public void Delete(int id)
    {
        var queryString = $"DELETE FROM Accounts WHERE id = {id}";
        ExecuteNonQuery(queryString);
    }
    
    public void Update(int id, string updateField, string value)
    {
        var queryString = $"UPDATE Accounts SET {updateField} = {value} WHERE id = {id}";
        ExecuteNonQuery(queryString);
    }

    public void Insert(Account acc) // update with reflection
    {
        var queryString = $"SELECT INTO Accounts (Id, Name, Password) VALUES ({acc.Id}, {acc.Name}, {acc.Password})";
        ExecuteNonQuery(queryString);
    }
}