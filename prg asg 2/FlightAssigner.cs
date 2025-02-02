using System;
using System.Collections.Generic;
using System.Linq;
//ethan//
namespace prg_asg_2
{
    public class FlightAssigner
    {
        private Terminal terminal;

        public FlightAssigner(Terminal terminal)
        {
            this.terminal = terminal;
        }

        public void ProcessUnassignedFlights()
        {
            Queue<Flight> unassignedFlights = new Queue<Flight>();
            List<BoardingGate> availableGates = terminal.BoardingGates.Values.Where(g => g.Flight == null).ToList();

            // Count unassigned flights and gates before processing
            int totalUnassignedFlights = terminal.Flights.Values.Count(f => !terminal.BoardingGates.Values.Any(g => g.Flight == f));
            int totalUnassignedGates = terminal.BoardingGates.Values.Count(g => g.Flight == null);

            Console.WriteLine($"Total unassigned flights: {totalUnassignedFlights}");
            Console.WriteLine($"Total available gates: {totalUnassignedGates}");

            // Identify flights without assigned gates
            foreach (var flight in terminal.Flights.Values)
            {
                if (!terminal.BoardingGates.Values.Any(g => g.Flight?.FlightNumber == flight.FlightNumber))
                {
                    unassignedFlights.Enqueue(flight);
                }
            }

            int assignedFlights = 0;
            while (unassignedFlights.Count > 0 && availableGates.Count > 0)
            {
                Flight flight = unassignedFlights.Dequeue();
                BoardingGate assignedGate = null;

                // Prioritize Special Request Code Matching
                if (flight is DDJBFlight)
                {
                    assignedGate = availableGates.FirstOrDefault(g => g.SupportsDDJB);
                }
                else if (flight is CFFTFlight)
                {
                    assignedGate = availableGates.FirstOrDefault(g => g.SupportsCFFT);
                }
                else if (flight is LWTTFlight)
                {
                    assignedGate = availableGates.FirstOrDefault(g => g.SupportsLWTT);
                }

                // If no special request, assign any available gate
                if (assignedGate == null)
                {
                    assignedGate = availableGates.FirstOrDefault();
                }

                if (assignedGate != null)
                {
                    assignedGate.Flight = flight;
                    availableGates.Remove(assignedGate);
                    assignedFlights++;

                    // Display full flight details after assignment
                    Console.WriteLine("=============================================");
                    Console.WriteLine($"Assigned Flight {flight.FlightNumber} to Gate {assignedGate.GateName}");
                    Console.WriteLine($"Flight Number: {flight.FlightNumber}");
                    Console.WriteLine($"Origin: {flight.Origin}");
                    Console.WriteLine($"Destination: {flight.Destination}");
                    Console.WriteLine($"Expected Time: {flight.ExpectedTime:dd/MM/yyyy hh:mm tt}");
                    Console.WriteLine($"Special Request Code: {(flight is CFFTFlight ? "CFFT" : flight is DDJBFlight ? "DDJB" : flight is LWTTFlight ? "LWTT" : "None")}");
                    Console.WriteLine($"Assigned Boarding Gate: {assignedGate.GateName}");
                }
            }

            // Display the total number of Flights and Boarding Gates processed
            Console.WriteLine("=============================================");
            Console.WriteLine($"Total flights assigned: {assignedFlights}");
            Console.WriteLine($"Remaining unassigned flights: {unassignedFlights.Count}");

            // Calculate and display percentage of flights and gates processed
            double flightPercentage = (assignedFlights / (double)totalUnassignedFlights) * 100;
            double gatePercentage = ((totalUnassignedGates - availableGates.Count) / (double)totalUnassignedGates) * 100;
            Console.WriteLine($"Flights Processed: {flightPercentage:F2}%");
            Console.WriteLine($"Gates Utilized: {gatePercentage:F2}%");
        }
    }
}
