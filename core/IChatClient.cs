using server.models;

public interface IChatClient
{
    Task RecieveMessage(MessageModel messageModel);
    Task NotifyLogin(IEnumerable<UserModel> user, string message);
    Task Disconected(string user, string message);
}