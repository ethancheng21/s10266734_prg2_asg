using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    public class CFFTFlight : Flight
    {
        // Additional Property
        public double RequestFee { get; set; }

        // Constructor
        public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            RequestFee = requestFee;
        }

        // Implement CalculateFees
        public override double CalculateFees()
        {
            return RequestFee + 150.0; // Base fee + request fee
        }

        public override string ToString()
        {
            return base.ToString() + $" (CFFT Flight, Request Fee: {RequestFee})";
        }
    }
}
