// DataHub.cs

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SignalRBugRepro
{
    public class DataHub : Hub
    {
        private readonly ILogger<DataHub> _logger;

        public DataHub(ILogger<DataHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}. Reason: {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Generates and sends a large data payload to the client
        /// </summary>
        /// <param name="sizeMB">Size of data to generate in megabytes</param>
        public async Task RequestLargeData(int sizeMB)
        {
            _logger.LogInformation($"Client {Context.ConnectionId} requested {sizeMB}MB of data");
            
            // Generate large data
            string largeData = GenerateLargeData(sizeMB);
            
            _logger.LogInformation($"Sending {sizeMB}MB of data to client {Context.ConnectionId}");
            
            // Send the large data payload
            // This is where the bug manifests - the large payload blocks the ping response
            await Clients.Caller.SendAsync("ReceiveLargeData", largeData);
            
            _logger.LogInformation($"Finished sending data to client {Context.ConnectionId}");
        }
        
        /// <summary>
        /// Generates a string of specified size
        /// </summary>
        private string GenerateLargeData(int sizeMB)
        {
            var sb = new StringBuilder();
            
            // Calculate how many characters we need for the target size
            // Assuming each char is 2 bytes in UTF-16
            int targetSize = sizeMB * 1024 * 1024 / 2;
            
            // Generate random data in chunks to avoid memory issues
            Random random = new Random();
            const int chunkSize = 1024; // 1KB chunks
            
            for (int i = 0; i < targetSize / chunkSize; i++)
            {
                // Create a chunk of random data
                char[] chunk = new char[chunkSize];
                for (int j = 0; j < chunkSize; j++)
                {
                    // Use printable ASCII characters (32-126)
                    chunk[j] = (char)random.Next(32, 127);
                }
                
                sb.Append(chunk);
                
                // Log progress for large payloads
                if (i % 1024 == 0 && i > 0)
                {
                    _logger.LogInformation($"Generated {i * chunkSize / (1024 * 512)}MB of {sizeMB}MB");
                }
            }
            
            return sb.ToString();
        }
    }
}
