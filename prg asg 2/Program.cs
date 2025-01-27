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
            Console.WriteLine("3. Assign a Boarding Gate to a Flight");
            Console.WriteLine("4. Create Flight");
            Console.WriteLine("5. Display Airline Flights");
            Console.WriteLine("6. Modify Flight Details");
            Console.WriteLine("7. Display Flight Schedule");
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
            else if (option == "3")
            {
                AssignBoardingGate(flights.ToDictionary(f => f.FlightNumber, f => f),
                                   boardingGates.ToDictionary(g => g.GateName, g => g));
            }
            else if (option == "4")
            {
                break;
            }
            else if (option == "5")
            {
                DisplayAirlineFlights(airlines.ToDictionary(a => a.Code, a => a));
            }
            else if (option == "6")
            {
                break;
            }
            else if (option == "7")
            {
                DisplayScheduledFlights(flights, boardingGates.ToDictionary(g => g.GateName, g => g));
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


    static void ListBoardingGates(List<BoardingGate> boardingGates)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("Gate Name       DDJB                   CFFT                   LWTT");

        foreach (var gate in boardingGates)
        {
            Console.WriteLine(
                $"{gate.GateName,-15}" +
                $"{gate.SupportsDDJB,-20}" +
                $"{gate.SupportsCFFT,-20}" +
                $"{gate.SupportsLWTT,-20}");
        }
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
                string airlineCode = flightNumber.Length >= 2 ? flightNumber.Substring(0, 2) : "";
                string origin = data[1];
                string destination = data[2];
                DateTime expectedTime;

                // Attempt to parse the date/time; skip if invalid
                if (!DateTime.TryParse(data[3], out expectedTime))
                {
                    Console.WriteLine($"Skipping invalid date/time for flight: {flightNumber}");
                    continue;
                }

                string status = "Scheduled"; // Default status for all flights
                string specialRequest = data.Length > 5 ? data[5] : "";

                var airline = airlines.Find(a => a.Code == airlineCode);

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
                $"{flight.FlightNumber,-15}" +
                $"{flight.Airline.Name,-25}" +
                $"{flight.Origin,-20}" +
                $"{flight.Destination,-20}" +
                $"{flight.ExpectedTime:dd/MM/yyyy hh:mm:ss tt}");
        }
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
                string gateName = data[0].Trim();
                bool supportsDDJB = string.Equals(data[1].Trim(), "True", StringComparison.OrdinalIgnoreCase);
                bool supportsCFFT = string.Equals(data[2].Trim(), "True", StringComparison.OrdinalIgnoreCase);
                bool supportsLWTT = string.Equals(data[3].Trim(), "True", StringComparison.OrdinalIgnoreCase);

                boardingGates.Add(new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT));
            }
        }

        return boardingGates;
    }





    static void AssignBoardingGate(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Assign a Boarding Gate to a Flight");
        Console.WriteLine("=============================================");

        // Prompt the user for the Flight Number
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine()?.ToUpper();

        // Check if the flight exists
        if (!flights.ContainsKey(flightNumber))
        {
            Console.WriteLine("Invalid Flight Number. Please try again.");
            return;
        }

        var selectedFlight = flights[flightNumber];

        // Prompt the user for the Boarding Gate
        Console.Write("Enter Boarding Gate Name: ");
        string gateName = Console.ReadLine()?.ToUpper();

        // Check if the boarding gate exists
        if (!boardingGates.ContainsKey(gateName))
        {
            Console.WriteLine("Invalid Boarding Gate. Please try again.");
            return;
        }

        var selectedGate = boardingGates[gateName];

        if (selectedGate.Flight == null)
        {
            // Assign the gate to the flight
            selectedGate.Flight = selectedFlight;

            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime:hh:mm:ss tt}");
            Console.WriteLine($"Special Request Code: {(selectedFlight is CFFTFlight ? "CFFT" :
                                                        selectedFlight is DDJBFlight ? "DDJB" :
                                                        selectedFlight is LWTTFlight ? "LWTT" : "None")}");
            Console.WriteLine($"Boarding Gate Name: {gateName}");
            Console.WriteLine($"Supports DDJB: {selectedGate.SupportsDDJB}");
            Console.WriteLine($"Supports CFFT: {selectedGate.SupportsCFFT}");
            Console.WriteLine($"Supports LWTT: {selectedGate.SupportsLWTT}");

            // Prompt to update flight status
            Console.Write("Would you like to update the status of the flight? (Y/N): ");
            string updateStatus = Console.ReadLine()?.ToUpper();

            if (updateStatus == "Y")
            {
                Console.WriteLine("1. Delayed");
                Console.WriteLine("2. Boarding");
                Console.WriteLine("3. On Time");
                Console.Write("Please select the new status of the flight: ");
                string statusOption = Console.ReadLine();

                if (statusOption == "1")
                    selectedFlight.Status = "Delayed";
                else if (statusOption == "2")
                    selectedFlight.Status = "Boarding";
                else if (statusOption == "3")
                    selectedFlight.Status = "On Time";
                else
                    Console.WriteLine("Invalid option. Status set to default: On Time.");
            }
            else
            {
                selectedFlight.Status = "On Time";
            }

            Console.WriteLine($"Flight {selectedFlight.FlightNumber} has been assigned to Boarding Gate {gateName}!");
        }
        else
        {
            Console.WriteLine($"Boarding Gate {gateName} is already assigned to Flight {selectedGate.Flight.FlightNumber}.");
        }
    }


    static void DisplayAirlineFlights(Dictionary<string, Airline> airlines)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Airline Code",-15}{"Airline Name",-30}");

        // List all airlines
        foreach (var airline in airlines.Values)
        {
            Console.WriteLine($"{airline.Code,-15}{airline.Name,-30}");
        }

        // Prompt the user to enter the airline code
        Console.Write("Enter Airline Code: ");
        string airlineCode = Console.ReadLine()?.ToUpper();

        if (!airlines.ContainsKey(airlineCode))
        {
            Console.WriteLine("Invalid Airline Code. Please try again.");
            return;
        }

        var selectedAirline = airlines[airlineCode];

        Console.WriteLine("=============================================");
        Console.WriteLine($"List of Flights for {selectedAirline.Name}");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Flight Number",-15}{"Airline Name",-25}{"Origin",-20}{"Destination",-20}{"Expected Departure/Arrival Time",-30}");

        // List all flights for the selected airline
        foreach (var flight in selectedAirline.Flights.Values)
        {
            Console.WriteLine($"{flight.FlightNumber,-15}" +
                              $"{flight.Airline.Name,-25}" +
                              $"{flight.Origin,-20}" +
                              $"{flight.Destination,-20}" +
                              $"{flight.ExpectedTime:dd/MM/yyyy hh:mm tt}");
        }
    }

    static void DisplayScheduledFlights(List<Flight> flights, Dictionary<string, BoardingGate> boardingGates)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Flight Schedule for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine(string.Format("{0,-15}{1,-25}{2,-20}{3,-20}{4,-30}{5,-15}{6,-15}",
                          "Flight Number", "Airline Name", "Origin", "Destination",
                          "Expected Departure/Arrival Time", "Status", "Boarding Gate"));

        // Sort flights by ExpectedTime
        flights.Sort();

        foreach (var flight in flights)
        {
            // Default boarding gate is "Unassigned"
            string boardingGateName = "Unassigned";

            // Check if the flight is assigned to a boarding gate
            foreach (var gate in boardingGates.Values)
            {
                if (gate.Flight != null && gate.Flight.FlightNumber == flight.FlightNumber)
                {
                    boardingGateName = gate.GateName;
                    break;
                }
            }

            // Normalize the status to "Scheduled" if it's the default
            string status = flight.Status.Equals("Scheduled", StringComparison.OrdinalIgnoreCase) ? "Scheduled" : flight.Status;

            // Print aligned flight details using string.Format
            Console.WriteLine(string.Format("{0,-15}{1,-25}{2,-20}{3,-20}{4,-30}{5,-15}{6,-15}",
                              flight.FlightNumber,
                              flight.Airline?.Name ?? "Unknown",
                              flight.Origin,
                              flight.Destination,
                              flight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt"),
                              status,
                              boardingGateName));
        }
    }



}
