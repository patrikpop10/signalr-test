using server.models;

public sealed record MessageModel(UserModel From, string? To, string Message, DateTime? Date);