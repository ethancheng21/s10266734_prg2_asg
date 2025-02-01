using prg_asg_2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Globalization;


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
                CreateFlight(flights, airlines.ToDictionary(a => a.Code, a => a), "flights.csv");
            }
            else if (option == "5")
            {
                DisplayAirlineFlights(airlines.ToDictionary(a => a.Code, a => a));
            }
            else if (option == "6")
            {
                ModifyFlightDetails(flights,
                                    airlines.ToDictionary(a => a.Code, a => a),
                                    boardingGates.ToDictionary(g => g.GateName, g => g));
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

        Console.Write("\nEnter Airline Code: ");
        string airlineCode = Console.ReadLine()?.ToUpper();

        if (!airlines.ContainsKey(airlineCode))
        {
            Console.WriteLine("Invalid Airline Code. Please try again.");
            return;
        }

        var selectedAirline = airlines[airlineCode];

        Console.WriteLine("\n=============================================");
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

        // Sort flights by ExpectedTime (earliest first)
        flights.Sort((f1, f2) => f1.ExpectedTime.CompareTo(f2.ExpectedTime));

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

            // Ensure status is always displayed correctly
            string status = string.IsNullOrEmpty(flight.Status) ? "Scheduled" : flight.Status;

            // Determine Special Request Code based on flight type
            string specialRequest = "Scheduled"; // Default to scheduled if no special request
            if (flight is CFFTFlight) specialRequest = "CFFT";
            else if (flight is DDJBFlight) specialRequest = "DDJB";
            else if (flight is LWTTFlight) specialRequest = "LWTT";

            // Print flight details using `.Format()`
            Console.WriteLine(string.Format("{0,-15}{1,-25}{2,-20}{3,-20}{4,-30}{5,-15}{6,-15}",
                              flight.FlightNumber,
                              flight.Airline?.Name ?? "Unknown",
                              flight.Origin,
                              flight.Destination,
                              flight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt"),
                              specialRequest, // Status or Special Request
                              boardingGateName));
        }
    }



    static void CreateFlight(List<Flight> flights, Dictionary<string, Airline> airlines, string filePath)
    {
        while (true)
        {
            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine().ToUpper();

            // Check if flight already exists
            if (flights.Any(f => f.FlightNumber == flightNumber))
            {
                Console.WriteLine("Flight with this number already exists. Please try again.");
                continue;
            }

            Console.Write("Enter Origin: ");
            string origin = Console.ReadLine();

            Console.Write("Enter Destination: ");
            string destination = Console.ReadLine();

            Console.Write("Enter Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
            string dateInput = Console.ReadLine();
            DateTime expectedTime;
            if (!DateTime.TryParseExact(dateInput, "d/M/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out expectedTime))
            {
                Console.WriteLine("Invalid date/time format. Please try again.");
                continue;
            }

            string airlineCode = flightNumber.Substring(0, 2);
            if (!airlines.ContainsKey(airlineCode))
            {
                Console.WriteLine($"No airline found for the code {airlineCode}. Please enter a valid flight number.");
                continue;
            }

            Airline airline = airlines[airlineCode];

            Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
            string specialRequest = Console.ReadLine().ToUpper();

            Flight newFlight;
            switch (specialRequest)
            {
                case "CFFT":
                    newFlight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 150.0, airline);
                    break;
                case "DDJB":
                    newFlight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 300.0, airline);
                    break;
                case "LWTT":
                    newFlight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 500.0, airline);
                    break;
                default:
                    newFlight = new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled", airline);
                    break;
            }

            flights.Add(newFlight);
            airline.Flights[flightNumber] = newFlight;

            try
            {
                using (var writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{flightNumber},{origin},{destination},{expectedTime:dd/MM/yyyy HH:mm},Scheduled,{specialRequest}");
                }
                Console.WriteLine($"\nFlight {flightNumber} has been added!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to file: " + ex.Message);
            }

            Console.Write("Would you like to add another flight? (Y/N): ");
            if (Console.ReadLine().Trim().ToUpper() != "Y")
                break;
        }
    }


    static void ModifyFlightDetails(List<Flight> flights, Dictionary<string, Airline> airlines, Dictionary<string, BoardingGate> boardingGates)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Airline Code",-15}{"Airline Name",-30}");

        // Display all airlines
        foreach (var airline in airlines.Values)
        {
            Console.WriteLine($"{airline.Code,-15}{airline.Name,-30}");
        }

        Console.Write("\nEnter Airline Code: ");
        string airlineCode = Console.ReadLine()?.ToUpper();

        if (!airlines.ContainsKey(airlineCode))
        {
            Console.WriteLine("Invalid Airline Code. Please try again.");
            return;
        }

        var selectedAirline = airlines[airlineCode];

        Console.WriteLine("\n=============================================");
        Console.WriteLine($"List of Flights for {selectedAirline.Name}");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Flight Number",-15}{"Airline Name",-25}{"Origin",-20}{"Destination",-20}{"Expected Departure/Arrival Time",-30}");

        // Display flights for selected airline
        foreach (var flight in selectedAirline.Flights.Values)
        {
            Console.WriteLine($"{flight.FlightNumber,-15}" +
                              $"{flight.Airline.Name,-25}" +
                              $"{flight.Origin,-20}" +
                              $"{flight.Destination,-20}" +
                              $"{flight.ExpectedTime:dd/MM/yyyy h:mm:ss tt}");
        }

        Console.Write("\nChoose an existing Flight to modify or delete: ");
        string flightNumber = Console.ReadLine()?.ToUpper();

        if (!selectedAirline.Flights.ContainsKey(flightNumber))
        {
            Console.WriteLine("Invalid Flight Number. Please try again.");
            return;
        }

        var selectedFlight = selectedAirline.Flights[flightNumber];

        Console.WriteLine("\n1. Modify Flight");
        Console.WriteLine("2. Delete Flight");
        Console.Write("Choose an option: ");
        string choice = Console.ReadLine();

        if (choice == "2")
        {
            selectedAirline.Flights.Remove(flightNumber);
            flights.Remove(selectedFlight);
            Console.WriteLine("\nFlight deleted successfully.");
            return;
        }

        Console.WriteLine("\n1. Modify Basic Information");
        Console.WriteLine("2. Modify Status");
        Console.WriteLine("3. Modify Special Request Code");
        Console.WriteLine("4. Modify Boarding Gate");
        Console.Write("Choose an option: ");
        string modifyChoice = Console.ReadLine();

        if (modifyChoice == "1")  // Modify Flight Details
        {
            Console.Write("Enter new Origin: ");
            string newOrigin = Console.ReadLine();
            if (!string.IsNullOrEmpty(newOrigin)) selectedFlight.Origin = newOrigin;

            Console.Write("Enter new Destination: ");
            string newDestination = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDestination)) selectedFlight.Destination = newDestination;

            Console.Write("Enter new Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
            if (DateTime.TryParseExact(Console.ReadLine(), "d/M/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newTime))
            {
                selectedFlight.ExpectedTime = newTime;
            }
            else
            {
                Console.WriteLine("Invalid date/time format. Keeping the current value.");
            }
        }
        else if (modifyChoice == "2")  // Modify Flight Status
        {
            Console.WriteLine("1. Delayed");
            Console.WriteLine("2. Boarding");
            Console.WriteLine("3. On Time");
            Console.Write("Choose the new status: ");
            string statusOption = Console.ReadLine();

            selectedFlight.Status = statusOption switch
            {
                "1" => "Delayed",
                "2" => "Boarding",
                "3" => "On Time",
                _ => selectedFlight.Status
            };
        }
        else if (modifyChoice == "3")  // Modify Special Request Code
        {
            Console.Write("Enter new Special Request Code (CFFT/DDJB/LWTT/None): ");
            string newRequest = Console.ReadLine()?.ToUpper();

            if (newRequest == "CFFT" || newRequest == "DDJB" || newRequest == "LWTT" || newRequest == "NONE")
            {
                // Create a new Flight object with the correct special request type
                Flight newFlight;
                switch (newRequest)
                {
                    case "CFFT":
                        newFlight = new CFFTFlight(selectedFlight.FlightNumber, selectedFlight.Origin, selectedFlight.Destination, selectedFlight.ExpectedTime, selectedFlight.Status, 150.0, selectedFlight.Airline);
                        break;
                    case "DDJB":
                        newFlight = new DDJBFlight(selectedFlight.FlightNumber, selectedFlight.Origin, selectedFlight.Destination, selectedFlight.ExpectedTime, selectedFlight.Status, 300.0, selectedFlight.Airline);
                        break;
                    case "LWTT":
                        newFlight = new LWTTFlight(selectedFlight.FlightNumber, selectedFlight.Origin, selectedFlight.Destination, selectedFlight.ExpectedTime, selectedFlight.Status, 500.0, selectedFlight.Airline);
                        break;
                    default:
                        newFlight = new NORMFlight(selectedFlight.FlightNumber, selectedFlight.Origin, selectedFlight.Destination, selectedFlight.ExpectedTime, selectedFlight.Status, selectedFlight.Airline);
                        break;
                }

                // Replace the old flight object in both the flights list and airline's dictionary
                flights.Remove(selectedFlight);
                flights.Add(newFlight);
                selectedAirline.Flights[selectedFlight.FlightNumber] = newFlight;
                selectedFlight = newFlight; // Update reference for display
            }
            else
            {
                Console.WriteLine("Invalid input. Keeping the current value.");
            }
        }
        else if (modifyChoice == "4")  // Modify Boarding Gate
        {
            Console.Write("Enter new Boarding Gate Name: ");
            string newGate = Console.ReadLine()?.ToUpper();

            if (boardingGates.ContainsKey(newGate))
            {
                var gate = boardingGates[newGate];
                if (gate.Flight == null)
                {
                    gate.Flight = selectedFlight;
                    Console.WriteLine($"Boarding Gate changed to {newGate}");
                }
                else
                {
                    Console.WriteLine("Selected Boarding Gate is already assigned.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Boarding Gate. Keeping the current value.");
            }
        }

        // Determine Special Request Code based on flight type
        string specialRequest = "None";
        if (selectedFlight is CFFTFlight) specialRequest = "CFFT";
        else if (selectedFlight is DDJBFlight) specialRequest = "DDJB";
        else if (selectedFlight is LWTTFlight) specialRequest = "LWTT";

        // Ensure status is "Scheduled" unless explicitly changed
        if (selectedFlight.Status != "Delayed" && selectedFlight.Status != "Boarding" && selectedFlight.Status != "On Time")
        {
            selectedFlight.Status = "Scheduled";
        }

        // Display updated flight details
        Console.WriteLine("\nFlight updated!");
        Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
        Console.WriteLine($"Airline Name: {selectedFlight.Airline.Name}");
        Console.WriteLine($"Origin: {selectedFlight.Origin}");
        Console.WriteLine($"Destination: {selectedFlight.Destination}");
        Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime:dd/MM/yyyy h:mm:ss tt}");
        Console.WriteLine($"Status: {selectedFlight.Status}");
        Console.WriteLine($"Special Request Code: {specialRequest}");
        Console.WriteLine($"Boarding Gate: {(boardingGates.Values.Any(g => g.Flight == selectedFlight) ? boardingGates.Values.First(g => g.Flight == selectedFlight).GateName : "Unassigned")}");
    }


}
