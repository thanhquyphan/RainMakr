using System;
using Microsoft.SPOT;
using RainMakr.Core.EndPoints;
using RainMakr.Core.Web;

namespace RainMakr.Netduino
{
    using System.Collections;

    using RainMakr.Core.Utilities;


    public class Program
    {
        public static void Main()
        {
            WebServerWrapper.InitializeWebEndPoints(new ArrayList
                                                {
                                                    new SprinklerEndPoint()
                                                });
            WebServerWrapper.StartWebServer(2456, false);

            RunUtil.KeepRunning();
        }
    }
}
