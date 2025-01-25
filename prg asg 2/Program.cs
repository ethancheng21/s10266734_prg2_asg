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

        Console.WriteLine("=============================================");
        Console.WriteLine("Welcome to Changi Airport Terminal 5");
        Console.WriteLine("=============================================");

        while (true)
        {
            Console.WriteLine("1. List All Flights");
            Console.WriteLine("2. List Boarding Gates");
            Console.WriteLine("3. Assign a Boarding Gate to a Flight");
            Console.WriteLine("4. Create Flight");
            Console.WriteLine("5. Display Airline Flights");
            Console.WriteLine("6. Modify Flight Details");
            Console.WriteLine("7. Display Flight Schedule");
            Console.WriteLine("0. Exit");
            Console.Write("Please select your option: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    ListAllFlights(flights);
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

        foreach (var line in File.ReadLines(filePath).Skip(1))
        {
            var data = line.Split(',');
            airlines.Add(new Airline(data[0], data[1]));
        }

        return airlines;
    }

    static List<BoardingGate> LoadBoardingGates(string filePath)
    {
        var boardingGates = new List<BoardingGate>();
        var uniqueGateIds = new HashSet<string>();

        foreach (var line in File.ReadLines(filePath).Skip(1))
        {
            var data = line.Split(',');
            if (uniqueGateIds.Add(data[0]))
            {
                boardingGates.Add(new BoardingGate(data[0], bool.Parse(data[1]), bool.Parse(data[2]), bool.Parse(data[3])));
            }
        }

        return boardingGates;
    }

    static List<Flight> LoadFlights(string filePath, List<Airline> airlines)
    {
        var flights = new List<Flight>();

        foreach (var line in File.ReadLines(filePath).Skip(1))
        {
            var data = line.Split(',');

            var airline = airlines.FirstOrDefault(a => a.Code == data[1]);
            if (airline != null)
            {
                flights.Add(new Flight(data[0], airline, data[2], data[3], DateTime.Parse(data[4])));
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
            Console.WriteLine($"{flight.FlightNumber,-15}{flight.Airline.Name,-25}{flight.Origin,-20}{flight.Destination,-20}{flight.ExpectedDateTime,-30:dd/M/yyyy hh:mm:ss tt}");
        }
    }
}
