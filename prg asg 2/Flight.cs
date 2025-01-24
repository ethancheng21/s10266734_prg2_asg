using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    public abstract class Flight
    {
        // Properties
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }

        // Constructor
        protected Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
        }

        // Abstract Method
        public abstract double CalculateFees();

        public override string ToString()
        {
            return $"Flight {FlightNumber}: {Origin} -> {Destination}, Status: {Status}, Expected: {ExpectedTime}";
        }
    }
}
