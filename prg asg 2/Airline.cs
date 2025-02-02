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
            int totalFlights = Flights.Count;

            // Base fees from flights
            foreach (var flight in Flights.Values)
            {
                totalFees += flight.CalculateFees();
            }

            // Apply promotions in correct order
            double discount = 0.0;

            // 1. Apply the 3% off total bill if eligible (Before other discounts)
            if (totalFlights > 5)
            {
                discount += totalFees * 0.03; // 3% of total fees before other deductions
            }

            // 2. Apply other stackable discounts
            discount += (totalFlights / 3) * 350; // $350 for every 3 flights
            foreach (var flight in Flights.Values)
            {
                if (flight.ExpectedTime.Hour < 11 || flight.ExpectedTime.Hour > 21)
                {
                    discount += 110; // $110 for early/late flights
                }

                if (flight.Origin == "DXB" || flight.Origin == "BKK" || flight.Origin == "NRT")
                {
                    discount += 25; // $25 for flights from DXB, BKK, NRT
                }

                if (flight is NORMFlight)
                {
                    discount += 50; // $50 for normal flights with no special request
                }
            }

            return Math.Max(0, totalFees - discount); // Ensure total cannot be negative
        }

        public override string ToString()
        {
            return $"Airline: {Name} ({Code}), Flights: {Flights.Count}";
        }
    }
}