using prg_asg_2;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Loading Airlines...");
        var airlines = LoadAirlines("airlines.csv");
        Console.WriteLine(airlines.Count + " Airlines Loaded!");

        Console.WriteLine("Loading Boarding Gates...");
        var boardingGates = LoadBoardingGates("boardinggates.csv");
        Console.WriteLine(boardingGates.Count + " Boarding Gates Loaded!");

        Console.WriteLine("Loading Flights...");
        var flights = LoadFlights("flights.csv", airlines);
        Console.WriteLine(flights.Count + " Flights Loaded!");

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

            if (option == "1")
            {
                ListAllFlights(flights);
            }
            else if (option == "2")
            {
                ListBoardingGates(boardingGates);
            }
            else if (option == "0")
            {
                return;
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }

    static List<Airline> LoadAirlines(string filePath)
    {
        var airlines = new List<Airline>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Error: Airlines file not found.");
            return airlines;
        }

        bool isFirstRow = true; // Skip the header row
        foreach (var line in File.ReadLines(filePath))
        {
            if (isFirstRow)
            {
                isFirstRow = false;
                continue;
            }

            var data = line.Split(',');
            if (data.Length >= 2)
            {
                string code = data[0];
                string name = data[1];
                airlines.Add(new Airline(code, name));
            }
        }

        return airlines;
    }

    static List<BoardingGate> LoadBoardingGates(string filePath)
    {
        var boardingGates = new List<BoardingGate>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Error: Boarding gates file not found.");
            return boardingGates;
        }

        bool isFirstRow = true; // Skip the header row
        foreach (var line in File.ReadLines(filePath))
        {
            if (isFirstRow)
            {
                isFirstRow = false;
                continue;
            }

            var data = line.Split(',');
            if (data.Length >= 4)
            {
                string gateName = data[0];
                bool supportsDDJB = data[1] == "true";
                bool supportsCFFT = data[2] == "true";
                bool supportsLWTT = data[3] == "true";

                boardingGates.Add(new BoardingGate(gateName, supportsDDJB, supportsCFFT, supportsLWTT));
            }
        }

        return boardingGates;
    }

    static List<Flight> LoadFlights(string filePath, List<Airline> airlines)
    {
        var flights = new List<Flight>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Error: Flights file not found.");
            return flights;
        }

        bool isFirstRow = true; // Skip the header row
        foreach (var line in File.ReadLines(filePath))
        {
            if (isFirstRow)
            {
                isFirstRow = false;
                continue;
            }

            var data = line.Split(',');
            if (data.Length >= 5)
            {
                string flightNumber = data[0];
                string airlineCode = "";
                if (flightNumber.Length >= 2)
                {
                    airlineCode = flightNumber.Substring(0, 2);
                }

                string origin = data[1];
                string destination = data[2];
                DateTime expectedTime;

                // Attempt to parse the date/time; skip if invalid
                if (!DateTime.TryParse(data[3], out expectedTime))
                {
                    Console.WriteLine($"Skipping invalid date/time for flight: {flightNumber}");
                    continue;
                }

                string status = data[4];
                string specialRequest = "";
                if (data.Length > 5)
                {
                    specialRequest = data[5];
                }

                Airline airline = null;
                foreach (var a in airlines)
                {
                    if (a.Code == airlineCode)
                    {
                        airline = a;
                        break;
                    }
                }

                if (airline != null)
                {
                    Flight flight;
                    if (specialRequest == "CFFT")
                    {
                        flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, status, 0, airline);
                    }
                    else if (specialRequest == "DDJB")
                    {
                        flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, status, 0, airline);
                    }
                    else if (specialRequest == "LWTT")
                    {
                        flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, status, 0, airline);
                    }
                    else
                    {
                        flight = new NORMFlight(flightNumber, origin, destination, expectedTime, status, airline);
                    }

                    flights.Add(flight);
                    airline.Flights.Add(flight.FlightNumber, flight);
                }
            }
        }

        return flights;
    }

    static void ListAllFlights(List<Flight> flights)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Flights for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("Flight Number   Airline Name           Origin                 Destination            Expected Departure/Arrival Time");

        foreach (var flight in flights)
        {
            Console.WriteLine(
                flight.FlightNumber + new string(' ', 15 - flight.FlightNumber.Length) +
                flight.Airline.Name + new string(' ', 25 - flight.Airline.Name.Length) +
                flight.Origin + new string(' ', 20 - flight.Origin.Length) +
                flight.Destination + new string(' ', 20 - flight.Destination.Length) +
                flight.ExpectedTime.ToString("dd/MM/yyyy hh:mm tt"));
        }
    }

    static void ListBoardingGates(List<BoardingGate> boardingGates)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("Gate Name       DDJB                   CFFT                   LWTT");

        foreach (var gate in boardingGates)
        {
            Console.WriteLine(
                gate.GateName + new string(' ', 15 - gate.GateName.Length) +
                gate.SupportsDDJB.ToString() + new string(' ', 20 - gate.SupportsDDJB.ToString().Length) +
                gate.SupportsCFFT.ToString() + new string(' ', 20 - gate.SupportsCFFT.ToString().Length) +
                gate.SupportsLWTT.ToString());
        }
    }
}
