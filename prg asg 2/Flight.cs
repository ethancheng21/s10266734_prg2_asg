using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    public abstract class Flight : IComparable<Flight>
    {
        // Properties
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }
        public Airline Airline { get; set; } // New property to link flights to airlines

        // Constructor
        protected Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, Airline airline)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
            Airline = airline;
        }
        // Implement the CompareTo method
        public int CompareTo(Flight other)
        {
            if (other == null) return 1;

            // Compare flights based on their ExpectedTime
            return ExpectedTime.CompareTo(other.ExpectedTime);
        }


        // Abstract Method
        public virtual double CalculateFees()
        {
            return 0.0; // Default fee calculation (overridden in subclasses)
        }
        public override string ToString()
        {
            return $"Flight {FlightNumber}: {Origin} -> {Destination}, Airline: {Airline?.Name ?? "Unknown"}, Status: {Status}, Expected: {ExpectedTime}";
        }
    }
}
