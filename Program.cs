using System;
using wodSSHTunnelCOMLib;

namespace SshTunnel
{
    class Program
    {
        static wodSSHTunnelCOMLib.wodTunnelCom wodSSHTunnel1;
        private static void BuildTunnel(string hostname, int port, string login, string password)
        {
            wodSSHTunnel1 = new wodSSHTunnelCOMLib.wodTunnelCom();
            wodSSHTunnel1.Connected += new wodSSHTunnelCOMLib._IwodTunnelComEvents_ConnectedEventHandler(wodSSHTunnel1_Connected);
            wodSSHTunnel1.Disconnected += new wodSSHTunnelCOMLib._IwodTunnelComEvents_DisconnectedEventHandler(wodSSHTunnel1_Disconnected);
            wodSSHTunnel1.ChannelStop += new wodSSHTunnelCOMLib._IwodTunnelComEvents_ChannelStopEventHandler(wodSSHTunnel1_ChannelStop);
            wodSSHTunnel1.UserConnected += new wodSSHTunnelCOMLib._IwodTunnelComEvents_UserConnectedEventHandler(wodSSHTunnel1_UserConnected);
            wodSSHTunnel1.UserDisconnected += new wodSSHTunnelCOMLib._IwodTunnelComEvents_UserDisconnectedEventHandler(wodSSHTunnel1_UserDisconnected);
            wodSSHTunnel1.UserConnecting += new wodSSHTunnelCOMLib._IwodTunnelComEvents_UserConnectingEventHandler(wodSSHTunnel1_UserConnecting);

            //Authenticate with SSH server using hostname, login, password.
            wodSSHTunnel1.Hostname = hostname;
            wodSSHTunnel1.Port = port;
            wodSSHTunnel1.Login = login;
            wodSSHTunnel1.Password = password;
            wodSSHTunnel1.Protocol = ProtocolsEnum.SSH2;

            wodSSHTunnel1.Connect(wodSSHTunnel1.Hostname, wodSSHTunnel1.Port, wodSSHTunnel1.Protocol);
        }

        //Connected Event fires when wodSSHTunnel connects to a remote server.
        private static void wodSSHTunnel1_Connected()
        {
            //If a new channel is defined as RemoteListen, then SSH server will bind RemoteAddress on RemotePort (Note: RemoteAddress
            //as seen from the SSH server's end!!). When a new connection comes to the selected RemotePort, the SSH server will send
            //a notification to wodSSHTunnel, which will initiate a new connection towards the LocalAddress interface on LocalPort.
            //In this example we will forward port 5900. This is VNC server port. On remote server where your SSH server is you can
            //open VNC connection using 127.0.0.1 (localhost) and port 5900. You will be using encrypted connection instead insecure
            //Internet connection. To accept connections from a foreign address, use the UserConnecting event (below) and set the
            //SSH server option GatewayPorts to yes.
            wodSSHTunnel1.Channels.Add(wodSSHTunnelCOMLib.ForwardTypesEnum.RemoteListen, "127.0.0.1", 80, "0.0.0.0", 5900);
            wodSSHTunnel1.Channels.StartAll();

            Console.WriteLine("Remote listener added");
        }

        //Disconnected Event fires when wodSSHTunnel disconnects from the server.
        private static void wodSSHTunnel1_Disconnected(short ErrorCode, string ErrorText)
        {
            if (ErrorCode != 0)
            {
                Console.WriteLine("DISCONNECTED: " + ErrorText); //Connection error is received here
            }
        }

        private static void wodSSHTunnel1_ChannelStop(wodSSHTunnelCOMLib.Channel Chan, short ErrorCode, string ErrorText)
        {
            Console.WriteLine("Channel stop " + ErrorText); //Channel error is received here
        }

        //UserConnected Event fires when a user connects to the listening channel.
        private static void wodSSHTunnel1_UserConnected(wodSSHTunnelCOMLib.Channel Chan, wodSSHTunnelCOMLib.User User, short ErrorCode, string ErrorText)
        {
            Console.WriteLine("User from " + User.Hostname + " connected");
        }

        //UserDisconnected Event fires when a user disconnects from a channel.
        private static void wodSSHTunnel1_UserDisconnected(wodSSHTunnelCOMLib.Channel Chan, wodSSHTunnelCOMLib.User User, short ErrorCode, string ErrorText)
        {
            Console.WriteLine("User from " + User.Hostname + " left " + ErrorText);
        }

        //UserConnecting Event fires when a user wants to connect to the listening channel.
        //  By default all connections from localhost/127.0.0.1 are allowed, but connections from foreign
        //  addresses are not allowed. To accept these foreign connections, implement this event and set
        //  the Allow parameter to true.
        private static void wodSSHTunnel1_UserConnecting(wodSSHTunnelCOMLib.Channel Chan, string hostname, int port, ref bool allow)
        {
            Console.WriteLine("Allowing connection from " + hostname + ", port " + port);
            allow = true;
        }

        static void Main(string[] args)
        {
            if (args.Length == 4)
            {

                BuildTunnel(args[0], Convert.ToInt16(args[1]), args[2], args[3]);
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Usage: sshtunnel <hostname> <port> <user> <password>");
            }
        }
    }
}
