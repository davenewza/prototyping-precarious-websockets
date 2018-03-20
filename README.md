Prototyping websockets in networks with unreliability connectivity and low bandwith.

# Connecting to a SignalR server with WebSockets

Using WebSockets to connect to a SignalR hub requires that you change the URI scheme from `https` to `ws`, for example:

    ws://localhost:5000/user
	
## Connection negotiation

Establishing a connection with a SignalR hub requires that you first submit a negotiation payload (which is always in JSON) which specifies the messaging protocol to be used (either `json` or `messagepack`). Example:

```json
{
    "protocol": "json"
}
```

This request will return with a response. If an error occurs, an error message will be returned, for example:

```json
{
    "error": "Requested protocol 'whoops' is not available."
}
```

## Sending messages to SignalR

Non-blocking messages are to be sent with the following schema:

```json
{
    "type": 1,
    "target": "MethodName",
    "arguments": [ "value 1", "value 2" ]
}
```

**NOTE:** All JSON messages needs to be separated with `0x1e`.

## Sending messages to the UserHub

The user hub has a non-blocking method called `Accept` which accepts a single string parameter.

```json
{
    "invocationId": "123",
    "type": 1,
    "target": "Accept",
    "arguments": [ "32.45345,11.6762" ]
}
```

The field `invocationId` is used for blocking messages. This can be removed for non-blocking.