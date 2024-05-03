using API.Services.Contracts;

namespace API.Services.Implementation
{
    public class UserNotifications(ILogger<UserNotifications> logger) : IUserNotifications
    {
        public Task NotificationAboutUserOnline(long userId)
        {
            //Send notifications when user logined
            logger.LogDebug($"Notification about user {userId} sent.");
            return Task.FromResult(Task.CompletedTask);
        }
    }
}
