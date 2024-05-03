namespace API.Services.Contracts
{
    public interface IUserNotifications
    {
        Task NotificationAboutUserOnline(long userId);
    }
}
