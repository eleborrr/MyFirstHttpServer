using googleHW.Interfaces;

namespace googleHW.Models;

public class Account: IModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Password { get; set; }
    
    public Account(int id, string name, string password)
    {
        Id = id;
        Name = name;
        Password = password;
    }
    
    public Account(string name, string password)
    {
        Name = name;
        Password = password;
    }
    
    
}