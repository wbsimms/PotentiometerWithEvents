using System;
using Microsoft.SPOT;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.SocketInterfaces;

namespace Gadgeteer.Modules.WBSimms
{

    public class PotentiometerEventArgs : EventArgs
    {
        public PotentiometerEventArgs(double percent, double voltage)
        {
            this.Percent = percent;
            this.Voltage = voltage;
        }

        public double Percent { get; set; }
        public double Voltage { get; set; }
    }

    /// <summary>
    /// A Potentiometer module for Microsoft .NET Gadgeteer
    /// </summary>
    public class PotentiometerWithEvents : GTM.Module, IDisposable
    {
        public delegate void PotentiometerTickHandler(object potentiometer, PotentiometerEventArgs args);
        public event PotentiometerTickHandler PotentiometerTick;

        private GT.Timer eventTimer;


        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public PotentiometerWithEvents(int socketNumber)
        {
            Socket socket = Socket.GetSocket(socketNumber, true, this, null);
            this.input = GTI.AnalogInputFactory.Create(socket, GT.Socket.Pin.Three, this);
        }


        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        /// <param name="timerTicks">The milliseconds between event firings.</param>
        public PotentiometerWithEvents(int socketNumber, int timerTicks)
        {
            Socket socket = Socket.GetSocket(socketNumber, true, this, null);
            this.input = GTI.AnalogInputFactory.Create(socket, GT.Socket.Pin.Three, this);
            this.eventTimer = new Timer(timerTicks);
            this.eventTimer.Tick += eventTimer_Tick;
            this.eventTimer.Start();
        }

        void eventTimer_Tick(Timer timer)
        {
            if (PotentiometerTick != null)
            {
                PotentiometerTick(this, new PotentiometerEventArgs(this.ReadPotentiometerPercentage(), this.ReadPotentiometerVoltage()));
            }
        }


        /// <summary>
        /// Returns the current voltage reading of the potentiometer
        /// </summary>
        public double ReadPotentiometerVoltage()
        {
            return input.ReadVoltage();
        }

        /// <summary>
        /// Returns the current position of the potentiometer relative to its maximum: range 0.0 to 1.0
        /// </summary>
        public double ReadPotentiometerPercentage()
        {
            return input.ReadProportion();
        }

        private GTI.AnalogInput input;


        public void Dispose()
        {
            this.eventTimer.Stop();
        }
    }
}
