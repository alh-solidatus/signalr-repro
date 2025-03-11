## SignalR Bug/Limitation Reproduction

When sending large messages through SignalR WebSocket connections, a message must be fully downloaded within the timeout duration set on the connection for clients/servers. If not fully downloaded, the connection will be broken due to timeout, despite the connection being in place and data transfer occurring. 

Reproduction Steps:

 1. `dotnet run`
 2. Open `http://localhost:5146` in Chrome (firefox dev tools don't seem to throttle websockets).
 3. Open dev tools network tab, throttle to 3G.
 4. Click "connect", then "Request Large Data" once connected.
 5. In ~30s the connection will be broken due to timeout on the frontend.
 6. The connection also breaks on the backend, see the backend logs.
