namespace JWTTest;

public class UserModel
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public UserModel()
    {
        
    }

    public UserModel(string username, string password)
    {
        Id = new Random().Next(1, 20);
        UserName = username;
        Password = password;
    }
}
