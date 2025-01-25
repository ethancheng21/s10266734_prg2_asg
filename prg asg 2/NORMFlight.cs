using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    public class NORMFlight : Flight
    {
        // Constructor
        public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, Airline airline)
            : base(flightNumber, origin, destination, expectedTime, status, airline) { }

        // Implement CalculateFees
        public override double CalculateFees()
        {
            return 100.0; // Example fixed fee for normal flights
        }

        public override string ToString()
        {
            return base.ToString() + " (Normal Flight)";
        }
    }
}