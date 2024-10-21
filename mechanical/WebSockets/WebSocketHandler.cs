using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace mechanical.WebSockets
{
    public class WebSocketHandler
    {
        private readonly ConcurrentDictionary<WebSocket, string> _clients = new();

        public async Task HandleWebSocket(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _clients[webSocket] = ""; // Store the connected client

                await Receive(webSocket);
            }
        }

        private async Task Receive(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4]; 
            var receivedMessage = new StringBuilder(); 

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        var messageSegment = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        receivedMessage.Append(messageSegment);

                    } while (!result.EndOfMessage);

                    var message = receivedMessage.ToString();
                    receivedMessage.Clear();

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        _clients.TryRemove(webSocket, out _);
                    }
                    else
                    {
                        await BroadcastMessage(message);
                    }
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
                _clients.TryRemove(webSocket, out _);
            }
        }

        private async Task BroadcastMessage(string message)
        {
            foreach (var client in _clients.Keys)
            {
                if (client.State == WebSocketState.Open)
                {
                    var msg = Encoding.UTF8.GetBytes(message);
                    await client.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task CleanupDisconnectedClients()
        {
            while (true)
            {
                foreach (var client in _clients.Keys)
                {
                    if (client.State == WebSocketState.Closed || client.State == WebSocketState.Aborted)
                    {
                        _clients.TryRemove(client, out _);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1)); // Wait for a minute before checking again
            }
        }
    }
}