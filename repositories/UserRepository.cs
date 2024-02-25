using server.models;

public sealed class UserRepository : IUserRepository
{
    private readonly List<UserModel> _users = [];

    public void Add(UserModel model)
    {
        _users.Add(model);
    }

    public void Delete(string connectionId)
    {
        var u = _users.Find(x => x.ConnectionId == connectionId);
        _users.Remove(u);
    }

    public UserModel Get(string connectionId)
    {
     return _users.Find(u => u.ConnectionId == connectionId);
    }

    public IEnumerable<UserModel> GetAll()
    {
       return _users;
    }

    public UserModel GetByName(string name)
    {
      return _users.Find(u => u.Name == name);
    }

    public void Update(UserModel model, string name)
    {
        var u = Get(model.ConnectionId);
         _users.Remove(model);
        var newNameUser = u with {Name = name};
        _users.Add(newNameUser);
    }
  
}