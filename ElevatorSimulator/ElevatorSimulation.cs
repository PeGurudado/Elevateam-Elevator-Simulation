using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSimulator
{
    class ElevatorSimulation
    {
        static void Main(string[] args)
        {
            var config = new SimulationConfiguration(
                25, // Number of simulation days
                86400, // Number of ticks per day. Here one tick equals one second
                3, // Number of elevators
                0, // Energy per tick
                "OPTIMIZED", // AI type (options are 'REGULAR', 'OPTIMIZED', 'BENCHMARK')
                true, // Enable smart relocation
                "DAY_CYCLES", // Request generator types (options are 'DAY_CYCLES', 'UNIFORM')
                20, // Number of building floors
                1, // Elevator speed in floors per tick
                9, // Ticks needed to load or unload a passenger
                new uint[] { 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24},
                // Residents on each floor
                new uint[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                // Interest per floor
                );

            Console.WriteLine("Configuration:\n" + config);

            var averageRequestsPerResidentPerDayList = new uint[]{ 2, 3 }; //Average Requests Per Resident
            var simReports = new MetricsReport[averageRequestsPerResidentPerDayList.Count()];

            for (int i = 0; i < simReports.Count(); i++)
            {
                var averageRequestsPerResidentPerDay = averageRequestsPerResidentPerDayList[i];

                Console.WriteLine("Average requests per residents per day: " + averageRequestsPerResidentPerDay);

                config.AverageRequestsPerResidentPerDay = averageRequestsPerResidentPerDay;

                var report = new Simulation(config).StartSimulation();
                var reportPerDay = report / new MetricsReport(config.SimulationDays, config.SimulationDays, config.SimulationDays);
                var reportPerPersonPerDay = reportPerDay / new MetricsReport(config.TotalResidents, config.TotalResidents, config.TotalResidents);
                var reportPerPersonPerDayPerElevatorUse = reportPerPersonPerDay / new MetricsReport(config.AverageRequestsPerResidentPerDay, config.AverageRequestsPerResidentPerDay, config.AverageRequestsPerResidentPerDay);

                simReports[i] = reportPerPersonPerDayPerElevatorUse;

                Console.WriteLine("\nResults: ");
                Console.WriteLine(report + " total");
                Console.WriteLine(reportPerDay + " per day");
                Console.WriteLine(reportPerPersonPerDay + " per person per day");
                Console.WriteLine(reportPerPersonPerDayPerElevatorUse + " per person per day per elevator use");

                Console.WriteLine("\n\n");

            }

            StringBuilder b = new StringBuilder();
            b.Append("AI: " + config.AIType + ", Smart relocation: " + config.SmartRelocation + "\n");
            b.Append("Average requests per person per day, Waiting time, Travel time, Total time, Energy used\n");
            for (int i = 0; i < simReports.Count(); i++)
            {
                b.Append(averageRequestsPerResidentPerDayList[i] + ", " + simReports[i].WaitingTime + ", " + simReports[i].TravelTime + ", " + simReports[i].TotalTime + ", " + simReports[i].EnergyUsed + "\n");
            }

            string filePath = @"results.csv";
            File.WriteAllText(filePath, b.ToString());

            Console.ReadKey();
        }
    }
}
