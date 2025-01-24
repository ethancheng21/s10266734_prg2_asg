using prg_asg_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    public class Airline
    {
        // Properties
        public string Name { get; set; }
        public string Code { get; set; }
        public Dictionary<string, Flight> Flights { get; set; }

        // Constructor
        public Airline(string name, string code)
        {
            Name = name;
            Code = code;
            Flights = new Dictionary<string, Flight>();
        }

        // Methods
        public bool AddFlight(Flight flight)
        {
            // Check if the flight already exists by iterating through the flights
            foreach (var existingFlight in Flights.Values)
            {
                if (existingFlight.FlightNumber == flight.FlightNumber)
                {
                    return false; // Flight already exists
                }
            }

            // Add the flight
            Flights.Add(flight.FlightNumber, flight);
            return true;
        }

        public bool RemoveFlight(Flight flight)
        {
            // Find the flight to remove by iterating through the flights
            foreach (var existingFlight in Flights.Values)
            {
                if (existingFlight.FlightNumber == flight.FlightNumber)
                {
                    Flights.Remove(flight.FlightNumber);
                    return true; // Flight removed successfully
                }
            }

            return false; // Flight not found
        }

        public double CalculateFees()
        {
            double totalFees = 0.0;

            // Iterate through the flights and sum their fees
            foreach (var flight in Flights.Values)
            {
                totalFees += flight.CalculateFees();
            }

            return totalFees;
        }

        public override string ToString()
        {
            return $"Airline: {Name} ({Code}), Flights: {Flights.Count}";
        }
    }
}