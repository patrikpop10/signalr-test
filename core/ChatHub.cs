using Microsoft.AspNetCore.SignalR;
using server.models;

namespace core;

public sealed class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IUserRepository userRepository;

    public ChatHub(ILogger<ChatHub> logger, IUserRepository userRepository)
    {
        _logger = logger;
        this.userRepository = userRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context?.GetHttpContext()?.Request?.Query["userName"].ToString() ?? "default";

        _logger.LogInformation("Logged in: {userId} @ {date}", username, DateTime.Now);
        var user = new UserModel(Context!.ConnectionId, username);
        userRepository.Add(user);
        //await Clients.All.NotifyLogin(user, $"{Context.ConnectionId} has joined");
        
    }
    public async Task SendMessage(MessageModel message)
    {
        try
        { 
            _logger.LogInformation("{userId} said: {message}", Context.ConnectionId, message);
            var updatedMessage = message with {Date = DateTime.UtcNow};
        await Clients.Clients([userRepository.GetByName(message.To!).ConnectionId, Context.ConnectionId]).RecieveMessage(updatedMessage);
        }
         catch(NullReferenceException @null)
         {
            _logger.LogError("Could not find logged in user {to}, {execption}", message.To, @null);
         }   
       
    }
    public async Task PatchName(string name)
    {
       var x = userRepository.Get(Context.ConnectionId);
        userRepository.Update(x, name); 
       var user = userRepository.Get(Context.ConnectionId); 
        await Clients.All.NotifyLogin(userRepository.GetAll().Where(x => x.Name != "default" && x.Name != "balao"), $"{Context.ConnectionId} has joined");
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
       userRepository.Delete(Context.ConnectionId);
       Clients.All.Disconected(Context.ConnectionId, "Dsiconected");
       _logger.LogInformation("User {user} disconected", Context.ConnectionId);
        Clients.All.NotifyLogin(userRepository.GetAll().Where(x => x.Name != "default" && x.Name != "balao"), $"{Context.ConnectionId} has joined").Wait();

       return base.OnDisconnectedAsync(exception);
    }

}