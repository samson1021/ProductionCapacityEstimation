using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace mechanical.Hubs
{
    public class NotificationHub : Hub
    {
        // Remove database operations from the hub
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }
    }
}