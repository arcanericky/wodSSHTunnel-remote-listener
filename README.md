### Purpose
The [original example code](http://www.weonlydo.com/code.asp?did=Remote-port-forwarding&lang=CS) (no specific license attached) for remote port forwarding using wodSSHTunnel works only for remote listeners on the loopback (127.0.0.1) device. Some simple changes will allow it to work for a remote listener which is bound to all network interfaces. What this modification does is equivalent to the command line expression:
```
$ ssh -R :5900:localhost:80 <ssh-server-address>
```
Note there are some security implications in doing this so be sure you understand them before you implement them in a public facing environment.

In short, the key is to implement the UserConnecting event, returning true for the Allow parameter, then subscribe to it with your tunnel.

### Code Changes
#### SSH Server
The SSH server must be configured to allow remote listeners to be bound to any network interface. This is done by adding the line `GatewayPorts yes` or `GatewayPorts clientspecified` to the `sshd_config` file which is usually found at `/etc/ssh`. For more information see the page on [TCP port forwarding and the GatewayPorts option](http://www.snailbook.com/faq/gatewayports.auto.html).

#### Client Program
You must implement and subscribe to a [UserConnecting event](http://www.weonlydo.com/SSHTunnel/Help/wodSSHTunnel-UserConnecting-Event.html), setting the allow option to true.
### Example Code
Two changes must be made to the example code

1. Implement a `UserConnecting` event handler, setting the allow option to `true`.
```
private static void wodSSHTunnel1_UserConnecting(wodSSHTunnelCOMLib.Channel Chan, string hostname, int port, ref bool allow)
	{
	allow = true;
	}
```
2. For your tunnel add a subscription to your `UserConnecting` event.
```
wodSSHTunnel1.UserConnecting += new wodSSHTunnelCOMLib._IwodTunnelComEvents_UserConnectingEventHandler(wodSSHTunnel1_UserConnecting);
```

That's it! Let your new remote listener enjoy its freedom. Clone, compile and execute this project for a working example.