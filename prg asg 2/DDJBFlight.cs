using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    public class DDJBFlight : Flight
    {
        // Additional Property
        public double RequestFee { get; set; }

        // Constructor
        public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            RequestFee = requestFee;
        }

        // Implement CalculateFees
        public override double CalculateFees()
        {
            return RequestFee + 250.0; // Base fee + request fee
        }

        public override string ToString()
        {
            return base.ToString() + $" (DDJB Flight, Request Fee: {RequestFee})";
        }
    }
}
