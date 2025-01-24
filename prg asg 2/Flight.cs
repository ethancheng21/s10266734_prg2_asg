using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    abstract class Flight
    {
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }

        // Abstract constructor (you can't have an abstract constructor in C#)
        // Consider removing the abstract modifier for the constructor.
        public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
        }

        public override string ToString()
        {
            return $"Flight {FlightNumber}: {Origin} to {Destination}, Status: {Status}, Expected Time: {ExpectedTime}";
        }

        // Abstract method for calculating the fee
        public abstract decimal CalculateFee();
    }
}