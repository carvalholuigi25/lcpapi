using Microsoft.AspNetCore.SignalR;

namespace lcpapi.Hubs
{
    public class DataHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        
        public async Task SendCreate(string entity, object data)
        {
            await Clients.All.SendAsync("Created", entity, data);
        }

        public async Task SendUpdate(string entity, object data)
        {
            await Clients.All.SendAsync("Updated", entity, data);
        }

        public async Task SendDelete(string entity, object id)
        {
            await Clients.All.SendAsync("Deleted", entity, id);
        }
    }
}