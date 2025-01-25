using prg_asg_2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program   
{
    static void Main(string[] args)
    {
        Console.WriteLine("Loading Airlines...");
        var airlines = LoadAirlines("airlines.csv");
        Console.WriteLine($"{airlines.Count} Airlines Loaded!");

        Console.WriteLine("Loading Boarding Gates...");
        var boardingGates = LoadBoardingGates("boardinggates.csv");
        Console.WriteLine($"{boardingGates.Count} Boarding Gates Loaded!");

        Console.WriteLine("Loading Flights...");
        var flights = LoadFlights("flights.csv", airlines);
        Console.WriteLine($"{flights.Count} Flights Loaded!");

        while (true)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Welcome to Changi Airport Terminal 5");
            Console.WriteLine("=============================================");
            Console.WriteLine("1. List All Flights");
            Console.WriteLine("2. List Boarding Gates");
            Console.WriteLine("0. Exit");
            Console.Write("Please select your option: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    ListAllFlights(flights);
                    break;
                case "2":
                    ListBoardingGates(boardingGates);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static List<Airline> LoadAirlines(string filePath)
    {
        var airlines = new List<Airline>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: Airlines file not found at {filePath}");
            return airlines;
        }

        foreach (var line in File.ReadLines(filePath).Skip(1))
        {
            var data = line.Split(',');
            airlines.Add(new Airline(data[0].Trim(), data[1].Trim()));
        }

        return airlines;
    }

    static List<BoardingGate> LoadBoardingGates(string filePath)
    {
        var boardingGates = new List<BoardingGate>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: Boarding gates file not found at {filePath}");
            return boardingGates;
        }

        foreach (var line in File.ReadLines(filePath).Skip(1))
        {
            var data = line.Split(',');
            boardingGates.Add(new BoardingGate(
                data[0].Trim(),
                bool.Parse(data[1].Trim()),
                bool.Parse(data[2].Trim()),
                bool.Parse(data[3].Trim())
            ));
        }

        return boardingGates;
    }

    static List<Flight> LoadFlights(string filePath, List<Airline> airlines)
    {
        var flights = new List<Flight>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: Flights file not found at {filePath}");
            return flights;
        }

        int lineNumber = 0;

        foreach (var line in File.ReadLines(filePath).Skip(1)) // Skip header row
        {
            lineNumber++;
            try
            {
                var data = line.Split(',');

                if (data.Length < 5) // Minimum columns required
                {
                    Console.WriteLine($"Skipping invalid line {lineNumber}: {line}");
                    continue;
                }

                string flightNumber = data[0].Trim();
                string airlineCode = flightNumber.Substring(0, 2); // Extract airline code from flight number
                string origin = data[1].Trim();
                string destination = data[2].Trim();
                DateTime expectedTime = DateTime.ParseExact(data[3].Trim(), "h:mm tt", null); // Handle time format
                string status = data.Length > 4 ? data[4].Trim() : "On Time";
                string specialRequest = data.Length > 5 ? data[5].Trim() : null;
                double requestFee = data.Length > 6 && double.TryParse(data[6].Trim(), out double fee) ? fee : 0.0;

                var airline = airlines.FirstOrDefault(a => a.Code == airlineCode);
                if (airline == null)
                {
                    Console.WriteLine($"Warning: Airline with code {airlineCode} not found. Skipping line {lineNumber}.");
                    continue;
                }

                // Determine flight type
                Flight flight = specialRequest switch
                {
                    "CFFT" => new CFFTFlight(flightNumber, origin, destination, expectedTime, status, requestFee, airline),
                    "DDJB" => new DDJBFlight(flightNumber, origin, destination, expectedTime, status, requestFee, airline),
                    "LWTT" => new LWTTFlight(flightNumber, origin, destination, expectedTime, status, requestFee, airline),
                    _ => new NORMFlight(flightNumber, origin, destination, expectedTime, status, airline),
                };

                flights.Add(flight);
                airline.Flights.Add(flight.FlightNumber, flight); // Link to airline
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on line {lineNumber}: {line}");
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
        return flights;
    }
    static void ListAllFlights(List<Flight> flights)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Flights for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Flight Number",-15}{"Airline Name",-25}{"Origin",-20}{"Destination",-20}{"Expected Departure/Arrival Time",-30}");

        foreach (var flight in flights)
        {
            Console.WriteLine($"{flight.FlightNumber,-15}{flight.Airline?.Name,-25}{flight.Origin,-20}{flight.Destination,-20}{flight.ExpectedTime,-30:dd/MM/yyyy hh:mm tt}");
        }
    }
    static void ListBoardingGates(List<BoardingGate> boardingGates)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Gate Name",-15}{"DDJB",-20}{"CFFT",-20}{"LWTT",-20}");

        foreach (var gate in boardingGates)
        {
            Console.WriteLine($"{gate.GateName,-15}{gate.SupportsDDJB,-20}{gate.SupportsCFFT,-20}{gate.SupportsLWTT,-20}");
        }
    }
}
