using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    public class LWTTFlight : Flight
    {
        public double RequestFee { get; set; }

        // Constructor
        public LWTTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee, Airline airline)
            : base(flightNumber, origin, destination, expectedTime, status, airline)
        {
            RequestFee = requestFee;
        }

        public override double CalculateFees()
        {
            return RequestFee + 200.0; // Base fee + request fee
        }

        public override string ToString()
        {
            return base.ToString() + $" (LWTT Flight, Request Fee: {RequestFee})";
        }
    }

}
