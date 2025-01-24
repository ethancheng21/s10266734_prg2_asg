using prg_asg_2;

string flightsFile = "flights.csv";

try
{
    var flights = ReadCsv(flightsFile);
    Console.WriteLine("Flights Data:");
    foreach (var flight in flights)
    {
        Console.WriteLine(string.Join(", ", flight));
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

static string[][] ReadCsv(string filePath)
{
    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException($"The file '{filePath}' does not exist.");
    }

    var lines = File.ReadAllLines(filePath);
    var data = new string[lines.Length][];

    for (int i = 0; i < lines.Length; i++)
    {
        data[i] = lines[i].Split(',');
    }

    return data;
}
