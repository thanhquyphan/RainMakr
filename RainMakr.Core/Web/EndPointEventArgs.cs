using System;
using Microsoft.SPOT;

namespace RainMakr.Core.Web
{
    using System.Net.Sockets;

    public class EndPointEventArgs
    {
        /// <summary>
        /// Allows us to tell the web server that we manually replied back
        /// via the socket. If false, the server will reply back with our string response
        /// This lets us write things other than the generic response around (for example if you want
        /// to stream custom binary)
        /// </summary>
        public bool ManualSent { get; set; }

        public EndPointEventArgs()
        {
        }

        public EndPointEventArgs(EndPoint command)
        {
            Command = command;
        }

        public EndPointEventArgs(EndPoint command, Socket connection)
        {
            Command = command;
            Connection = connection;
            Connection.SendTimeout = 5000;
        }

        public EndPoint Command { get; set; }
        public string ReturnString { get; set; }
        public Socket Connection { get; set; }
    }
}
