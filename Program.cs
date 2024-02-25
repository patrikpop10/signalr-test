using core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.WebHost.UseUrls("http://*:8080");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

ILogger logger = LoggerFactory.Create(config =>
    {
        config.AddConsole();
        config.AddConfiguration(builder.Configuration.GetSection("Logging"));
    }).CreateLogger("Program");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            // token validation code
        };
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments("/chat"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.MapHub<ChatHub>("/chat");

app.MapGet("hello",  () => {
    return Results.Ok("world");
});

app.UseHttpsRedirection();

app.Run();

