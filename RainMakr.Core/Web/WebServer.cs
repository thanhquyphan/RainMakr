using System;
using System.Collections;
using System.IO;
using Microsoft.SPOT;

namespace RainMakr.Core.Web
{
    using System.Text;
    using System.Threading;
    using System.Net;
    using System.Net.Sockets;

    using Microsoft.SPOT.Hardware;
    using Microsoft.SPOT.Net.NetworkInformation;

    using RainMakr.Core.Utilities;

    using SecretLabs.NETMF.Hardware.Netduino;

    class WebServer
    {
        private bool _cancel;
        private readonly Thread _serverThread;

        private readonly bool _enableLedStatus = true;

        private OutputPort _led;

        /// <summary>
        /// Instantiates a new webserver.
        /// </summary>
        /// <param name="port">Port number to listen on.</param>
        /// <param name="enableLedStatus"></param>
        public WebServer(int port, bool enableLedStatus = true)
        {
            Port = port;

            _serverThread = new Thread(StartServer);

            _enableLedStatus = enableLedStatus;

            Debug.Print("WebControl started on port " + port);
        }

        /// <summary>
        /// Delegate for the EndPointReceived event.
        /// </summary>
        public delegate void EndPointReceivedHandler(object source, EndPointEventArgs e);

        /// <summary>
        /// EndPointReceived event is triggered when a valid command (plus parameters) is received.
        /// Valid commands are defined in the AllowedEndPoints property.
        /// </summary>
        public event EndPointReceivedHandler EndPointReceived;
        
        /// <summary>
        /// Initialize the multithreaded server.
        /// </summary>
        public void Start()
        {
            // List ethernet interfaces, so we can determine the server's address
            ListInterfaces();

            // start server
            _cancel = false;
            _serverThread.Start();
            //var timer = new System.Threading.Timer((e) =>
            //{
            //    this.UpdateExternalIp();
            //}, null, TimeSpan.Zero, new TimeSpan(0, 0, 0, 5));
            

            Debug.Print("Started server in thread " + _serverThread.GetHashCode());
        }

        /// <summary>
        /// Parses a raw web request and filters out the command and arguments.
        /// </summary>
        /// <param name="rawData">The raw web request (including headers).</param>
        /// <returns>The parsed WebCommand if the request is valid, otherwise Null.</returns>
        private EndPoint InterpretRequest(string rawData)
        {
            if (rawData == null)
            {
                return null;
            }
            string commandData;

            // Remove GET/POST + Space
            if (rawData.Length > 5 || rawData.Length > 6)
                commandData = rawData.Substring(0, 3).ToLower() == "get" ? rawData.Substring(5, rawData.Length - 5) : rawData.Substring(6, rawData.Length - 6);
            else
                return null;

            // Remove everything after first space
            int idx = commandData.IndexOf("HTTP/1.1");
            commandData = commandData.Substring(0, idx - 1);

            // Split command and arguments
            string[] parts = commandData.Split('/');

            string command = null;
            if (parts.Length > 0)
            {
                // Parse first part to command
                command = parts[0].ToLower();
            }

            // http://url/foo/test
            // Check if this is a valid command
            EndPoint returnEndPoint = null;
            foreach (EndPoint endPoint in _allowedEndPoints)
            {
                if (command != null && endPoint.Name.ToLower() == command.ToLower())
                {
                    returnEndPoint = endPoint;
                    break;
                }
            }
            if (returnEndPoint == null)
            {
                return null;
            }

            var arguments = new string[parts.Length - 1];

            for (int i = 1; i < parts.Length; i++)
            {
                arguments[i - 1] = parts[i];
            }

            returnEndPoint.Arguments = arguments;

            return returnEndPoint;
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        private void StartServer()
        {
            using (var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                server.Bind(new IPEndPoint(IPAddress.Any, Port));

                server.Listen(1);

                while (!_cancel)
                {
                    var connection = server.Accept();
                    if (connection.Poll(-1, SelectMode.SelectRead))
                    {
                        // Create buffer and receive raw bytes.
                        var bytes = new byte[connection.Available];

                        connection.Receive(bytes);

                        // Convert to string, will include HTTP headers.
                        var rawData = new string(Encoding.UTF8.GetChars(bytes));

                        EndPoint endPoint = InterpretRequest(rawData);

                        if (endPoint != null)
                        {
                            if (_enableLedStatus)
                            {
                                PingLed();
                            }

                            // dispatch the endpoint
                            var e = new EndPointEventArgs(endPoint, connection);

                            if (EndPointReceived != null)
                            {
                                ThreadUtil.SafeQueueWorkItem(() =>
                                {
                                    EndPointReceived(null, e);

                                    if (e.ManualSent)
                                    {
                                        // the client should close the socket
                                    }
                                    else
                                    {
                                        var response = e.ReturnString;

                                        SendResponse(response, connection);
                                    }
                                });
                            }
                        }
                        else
                        {
                            SendResponse(GetApiList(), connection);
                        }
                    }

                }
            }
        }


        private void GetExternalIp()
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri("http://checkip.dyndns.org"));

            request.Method = "Get";
            var streamReader = new StreamReader(request.GetResponse().GetResponseStream());

            var publicIp = streamReader.ReadToEnd().Split(':')[1].Substring(1).Split('<')[0];

            Debug.Print(publicIp);
        }

        private void UpdateExternalIp()
        {
            try
            {
                //WebProxy proxy = new WebProxy("10.0.0.?", 8080);
                Debug.Print("Updating ip address");
                using (HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("http://rainmakrweb-test.azurewebsites.net/api/devices/updateIpAddress")))
                {
                    string postData = "macAddress=5C-86-4A-00-CE-40";
                    byte[] buffer = Encoding.UTF8.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = buffer.Length;
                    //request.KeepAlive = false;
                    //request.Timeout = 100;
                    //request.ReadWriteTimeout = 100;

                    // request body
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        Debug.Print("HTTP Status:" + response.StatusCode + " : " + response.StatusDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        private string GetApiList()
        {
            try
            {
                string returnString = @"
<body>
<head>
<style type=""text/css"">"
                                      + GetStylings() +
                                      @"</style>
</head>
<div class=""container"">
	<div class=""title"">Netduino Api List</div>
    <div class=""main"">
        <ul>
";
                foreach (EndPoint endpoint in _allowedEndPoints)
                {
                    returnString +=
                        @"           <li><a href=""" + endpoint.Name + "\">" + endpoint.Name +
                        "</a><span class=\"description\">(" + endpoint.Description + ")</span></li>";
                    returnString += "\r\n";
                }
                returnString += "</ul></body>";
                return returnString;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string GetStylings()
        {
            return
                @"
body{
    
}

ul { 
    list-style-type: circle; 
    font-size:20px;
} 

.container
{
	height:100%;
}                

.description{
	font-size:12px;
	padding:10px;
}	

.main{
	height:100%;
	padding:5px;
	border-bottom-left-radius: 15px;
	border-bottom-right-radius: 15px;
	-moz-border-botom-left-radius: 15px;
	-moz-border-botom-right-radius: 15px;
	
}

.title{
	font-size:50px;
	font-variant: small-caps;
	padding:20px;
	border-top-left-radius: 15px;
	border-top-right-radius: 15px;
	-moz-border-botom-left-radius: 15px;
	-moz-border-botom-right-radius: 15px;
}


a:link {color: black;}
a:visited {color: #998700;}
a:active {color: black;}
a:hover {color: #0F00B8;}
a {
    text-decoration: underline;
    font-variant:small-caps;
}
";

        }

        private static void WriteBytes(byte[] bytes, Socket connection)
        {
            try
            {
                connection.Send(bytes, 0, bytes.Length, SocketFlags.None);
                using (connection)
                {
                }
            }
            catch (Exception)
            {

            }
        }

        private static void SendResponse(string response, Socket connection)
        {
            try
            {
                var header = "HTTP/1.1 200 OK\r\nConnection: close\r\nContent-Length: " + response.Length + "\r\nContent-Type: text/plain\r\n\r\n" + response;
                byte[] returnBytes = Encoding.UTF8.GetBytes(header);
                WriteBytes(returnBytes, connection);
            }
            catch (Exception)
            {

            }
        }

        private void PingLed()
        {
            if (_led == null)
            {
                _led = new OutputPort(Pins.ONBOARD_LED, false);
            }

            _led.Write(true);
            Thread.Sleep(50);
            _led.Write(false);
        }

        private static void ListInterfaces()
        {
            NetworkInterface[] ifaces = NetworkInterface.GetAllNetworkInterfaces();
            Debug.Print("Number of Interfaces: " + ifaces.Length);
            foreach (NetworkInterface iface in ifaces)
            {
                Debug.Print("IP:  " + iface.IPAddress + "/" + iface.SubnetMask);
            }
        }

        /// <summary>
        /// Gets or sets the port the server listens on.
        /// </summary>
        private int Port { get; set; }

        /// <summary>
        /// List of commands that can be handled by the server.
        /// </summary>
        private readonly ArrayList _allowedEndPoints = new ArrayList();
        
        public void RegisterEndPoint(EndPoint endPoint)
        {
            _allowedEndPoints.Add(endPoint);
        }
    }
}
