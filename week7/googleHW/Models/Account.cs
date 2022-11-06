namespace googleHW.Models;

public class Account
{
    public int Id { get; }
    
    public string Name { get; }
    
    public string Password { get;  }
    
    public Account(int id, string name, string password)
    {
        Id = id;
        Name = name;
        Password = password;
    }
}