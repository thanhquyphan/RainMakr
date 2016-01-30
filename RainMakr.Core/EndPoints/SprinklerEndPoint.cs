using System;
using Microsoft.SPOT;

namespace RainMakr.Core.EndPoints
{
    using System.Collections;
    using System.Threading;

    using Microsoft.SPOT.Hardware;

    using RainMakr.Core.Web;

    using SecretLabs.NETMF.Hardware.Netduino;

    public class SprinklerEndPoint : IEndPointProvider
    {
        private OutputPort led;

        private bool sprinklerState;

        public SprinklerEndPoint()
        {
            this.led = new OutputPort(Pins.ONBOARD_LED, false);
            this.sprinklerState = false;
        }

        public void Initialize() { }

        public ArrayList AvailableEndPoints()
        {
            var list = new ArrayList
                {
                    new EndPoint
                        {
                            Action = this.StartSprinkler,
                            Name = "Start",
                            Description = "Turns on the sprinkler."
                        },
                        new EndPoint
                        {
                            Action = this.StopSprinkler,
                            Name = "Stop",
                            Description = "Turns off the sprinkler."
                        },
                        new EndPoint
                        {
                            Action = this.GetSprinklerStatus,
                            Name = "Status",
                            Description = "Gets current status of the sprinkler."
                        }
                };
            return list;
        }

        private string GetSprinklerStatus(EndPointActionArguments arguments, string[] items)
        {
            //String text = "";
            //if (items != null && items.Length > 0)
            //{
            //    foreach (var item in items)
            //    {
            //        text += item + " ";
            //    }
            //}
            //else
            //{
            //    text = "No arguments!";
            //}

            return this.sprinklerState ? "On" : "Off";
        }

        private string StartSprinkler(EndPointActionArguments misc, string[] items)
        {
            String text = "";
            if (items != null && items.Length > 0)
            {
                foreach (var item in items)
                {
                    text += item + " ";
                }
            }
            else
            {
                text = "No arguments!";
            }

            
            this.sprinklerState = true;
            this.led.Write(this.sprinklerState);
            if (items != null && items.Length > 0)
            {
                var seconds = int.Parse(items[0]);
                Thread.Sleep(1000 * seconds);
                this.led.Write(false);
            }
            
            
            //LcdWriter.Instance.Write(text);

            return "OK. Sprinkler is now on.";
        }

        private string StopSprinkler(EndPointActionArguments misc, string[] items)
        {
            String text = "";
            if (items != null && items.Length > 0)
            {
                foreach (var item in items)
                {
                    text += item + " ";
                }
            }
            else
            {
                text = "No arguments!";
            }

            this.sprinklerState = false;
            this.led.Write(this.sprinklerState);
            //LcdWriter.Instance.Write(text);

            return "OK. Sprinkler is now off.";
        }

    }
}
