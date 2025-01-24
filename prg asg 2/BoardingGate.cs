using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg_asg_2
{
    public class BoardingGate
    {
        // Properties
        public string GateName { get; set; }
        public bool SupportsCFFT { get; set; }
        public bool SupportsDDJB { get; set; }
        public bool SupportsLWTT { get; set; }
        public Flight Flight { get; set; }

        // Constructor
        public BoardingGate(string gateName, bool supportsCFFT, bool supportsDDJB, bool supportsLWTT)
        {
            GateName = gateName;
            SupportsCFFT = supportsCFFT;
            SupportsDDJB = supportsDDJB;
            SupportsLWTT = supportsLWTT;
            Flight = null; // Initially, no flight is assigned
        }

        // Methods
        public double CalculateFees()
        {
            // Calculate fees based on the flight type
            if (Flight != null)
            {
                return Flight.CalculateFees();
            }
            return 0.0; // No fees if no flight is assigned
        }

        public override string ToString()
        {
            return $"Gate: {GateName}, Supports - CFFT: {SupportsCFFT}, DDJB: {SupportsDDJB}, LWTT: {SupportsLWTT}, Assigned Flight: {(Flight != null ? Flight.FlightNumber : "None")}";
        }
    }

}
