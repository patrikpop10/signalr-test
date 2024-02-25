using server.models;

public interface IUserRepository
{
    void Add(UserModel model);
     UserModel Get(string connectionId);
     UserModel GetByName(string name);
     void Update(UserModel model, string name);
     void Delete(string model);
    IEnumerable<UserModel> GetAll();
}