using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program  // Go to Debug > WaccaCircle Debug Properties > Application > Change "Console Application" to "Windows Application"  and don't forget to set it back to console for other apps
{
    static string WaccaCircleStartup = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WaccaCircleStartup.exe");
    static void Main()
    {
        while (true)
        {
            try
            {
                // Create a new process to run WMIC command
                Process process = new Process();
                process.StartInfo.FileName = "wmic";
                process.StartInfo.Arguments = "process where \"name like '%WaccaCircle%'\" get name, processid";
                process.StartInfo.RedirectStandardOutput = true; // Capture the output
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                // Start the process
                process.Start();

                // Read and display the output
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (output == "\r\r\n\r\r\n")
                {

                    // Create a new process to run WMIC command
                    Process process2 = new Process();
                    process2.StartInfo.FileName = "wmic";
                    process2.StartInfo.Arguments = "process where \"name like '%Mercury%'\" get name, processid";
                    process2.StartInfo.RedirectStandardOutput = true; // Capture the output
                    process2.StartInfo.UseShellExecute = false;
                    process2.StartInfo.CreateNoWindow = true;

                    // Start the process
                    process2.Start();

                    // Read and display the output
                    output = process2.StandardOutput.ReadToEnd();
                    process2.WaitForExit();
                    if (output == "\r\r\n\r\r\n")
                    {
                        Process.Start(WaccaCircleStartup);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Thread.Sleep(20000); // Sleep for 20 seconds (20,000 milliseconds)
        }
    }
}
