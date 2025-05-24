using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace mechanical.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {

        public async Task SendNotification(string message, Guid userId = default)
        {
            if (userId == Guid.Empty)
            {
                await Clients.All.SendAsync("ReceiveNotification", message);
            }
            else
            {
                await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
            }
        }
        
        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }
            
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await base.OnConnectedAsync();
        }
    }
}