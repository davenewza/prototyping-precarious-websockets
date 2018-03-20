Prototyping websockets in networks with unreliability connectivity and low bandwith.

# Connecting to SignalR server with WebSockets

Using WebSockets to connect to a SignalR hub requires that you change the URI scheme from `https` to `ws`, for example:

    ws://localhost:5000/user
	
## Connection negotiation

Establishing a connection with a SignalR hub requires that you first submit a negotiation payload (always in JSON) which specifies the messaging protocol to be used (either `json` or `messagepack`). Example:

```json
{
    "protocol": "json"
}
```

## Sending messages

Non-blocking messages are to be sent with the following schema:

```json
{
	"type": 1,
	"target: "Accept",
	"arguments": [ "value" ]
}

