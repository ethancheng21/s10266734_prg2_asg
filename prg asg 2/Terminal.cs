using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    internal class Terminal
    {
        public string TerminalName { get; set; }
        public Dictionary<string, Airline> Airlines { get; set; } = new Dictionary<string, Airline>();
        public Dictionary<string, Flight> Flights { get; set; } = new Dictionary<string, Flight>();
        public Dictionary<string, BoardingGate> BoardingGates { get; set; } = new Dictionary<string, BoardingGate>();
        public Dictionary<string, double> GateFees { get; set; } = new Dictionary<string, double>();
        public Terminal() { }
        public Terminal(string terminalName)
        {
            TerminalName = terminalName;
            Airlines = new Dictionary<string, Airline>();
            Flights = new Dictionary<string, Flight>();
            BoardingGates = new Dictionary<string, BoardingGate>();
            GateFees = new Dictionary<string, double>();
        }

        public bool AddAirline(Airline airline)
        {
            // Check if the airline already exists using iteration
            foreach (var existingAirline in Airlines.Values)
            {
                if (existingAirline.Code == airline.Code)
                {
                    return false; // Airline already exists
                }
            }
            Airlines.Add(airline.Code, airline);
            return true;
        }
        public bool AddBoardingGate(BoardingGate gate)
        {
            // Check if the boarding gate already exists using iteration
            foreach (var existingGate in BoardingGates.Values)
            {
                if (existingGate.GateName == gate.GateName)
                {
                    return false; // Boarding gate already exists
                }
            }

            // Add the boarding gate
            BoardingGates.Add(gate.GateName, gate);
            return true;
        }
        public Airline GetAirlineFromFlight(Flight flight)
        {
            // Find the airline that contains the given flight
            foreach (var airline in Airlines.Values)
            {
                foreach (var existingFlight in airline.Flights.Values)
                {
                    if (existingFlight.FlightNumber == flight.FlightNumber)
                    {
                        return airline;
                    }
                }
            }
            return null; // Flight not associated with any airline
        }

        public void PrintAirlineFees()
        {
            foreach (var airline in Airlines.Values)
            {
                Console.WriteLine($"{airline.Name}: {airline.CalculateFees()}");
            }
        }
        public override string ToString()
        {
            return $"Terminal: {TerminalName}, Airlines: {Airlines.Count}, Flights: {Flights.Count}, Boarding Gates: {BoardingGates.Count}";
        }
    }
}