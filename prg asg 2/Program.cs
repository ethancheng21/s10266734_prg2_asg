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
                break;
            }
            else if (option == "6")
            {
                break;
            }
            else if (option == "7")
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

                string status = data[4];
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
                $"{flight.ExpectedTime:dd/MM/yyyy hh:mm tt}");
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

        // Prompt the user for the Boarding Gate immediately after entering the Flight Number
        Console.Write("Enter Boarding Gate Name: ");
        string gateName = Console.ReadLine()?.ToUpper();

        if (flights.TryGetValue(flightNumber, out var selectedFlight))
        {
            // Display the basic information of the selected Flight
            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime:dd/MM/yyyy hh:mm tt}");
            Console.WriteLine($"Special Request Code: {(selectedFlight is CFFTFlight ? "CFFT" :
                                                            selectedFlight is DDJBFlight ? "DDJB" :
                                                            selectedFlight is LWTTFlight ? "LWTT" : "None")}");

            if (boardingGates.TryGetValue(gateName, out var selectedGate))
            {
                if (selectedGate.Flight == null)
                {
                    // Display Boarding Gate Details
                    Console.WriteLine($"Boarding Gate Name: {gateName}");
                    Console.WriteLine($"Supports DDJB: {selectedGate.SupportsDDJB}");
                    Console.WriteLine($"Supports CFFT: {selectedGate.SupportsCFFT}");
                    Console.WriteLine($"Supports LWTT: {selectedGate.SupportsLWTT}");

                    // Assign the gate to the flight
                    selectedGate.Flight = selectedFlight;

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
            else
            {
                Console.WriteLine("Invalid Boarding Gate. Please try again.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Flight Number. Please try again.");
        }
    }






}
