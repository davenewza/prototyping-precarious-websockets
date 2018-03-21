# Connecting to a SignalR hub with WebSockets

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

## Invocation messages to SignalR

Non-blocking messages are to be sent with the following schema:

```json
{
    "type": 1,
    "target": "MethodName",
    "arguments": [ "value 1", "value 2" ]
}
```

**NOTE:** All JSON messages needs to be separated with `0x1e`.

## Sending messages to the User Hub

The user hub has a non-blocking method named `Accept` with a single string parameter. Invoking this target would look something like this:

```json
{
    "invocationId": "123",
    "type": 1,
    "target": "Accept",
    "arguments": [ "some value" ]
}
```

The field `invocationId` is used for blocking messages. This can be removed for non-blocking.

# Broadcasting a message from the server

You can broadcast a message to all clients using the following HTTP GET request:

`http://precariouswebsockets.azurewebsites.net/api/send?value=hello`

The above request will execute the `Accept("hello")` method on each client.

# Monitor server activity

The following HTTP GET request provides a log of connection and message activity on the server:

`http://precariouswebsockets.azurewebsites.net/api/`

Example:

```
3/21/2018 10:39:31 AM	9e56f704-ca6b-4883-a3de-76c2e01b4ef0	Connected
3/21/2018 10:39:53 AM	9e56f704-ca6b-4883-a3de-76c2e01b4ef0	Accept(hello)
3/21/2018 10:40:12 AM	9e56f704-ca6b-4883-a3de-76c2e01b4ef0	Accept(another message sent from this device)
3/21/2018 10:40:27 AM	9e56f704-ca6b-4883-a3de-76c2e01b4ef0	Disconnected
```