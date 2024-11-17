using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

// Go to Debug > WaccaCircle Debug Properties > Application > Change "Console Application" to "Windows Application"  and don't forget to set it back to console for other apps
// Go to Solution Explorer > References, then delete everything
class Program
{
    static string WaccaCircle = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WaccaCircle.exe");
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
                if (output == "\r\r\n\r\r\n")  // if no wacca circle, check if mercury is launched
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
                    if (output == "\r\r\n\r\r\n")  // mercury and waccacircle are not launched => We're on the menu!
                    {
                        Process.Start(WaccaCircle);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Thread.Sleep(10000); // Sleep for 10 seconds (10,000 milliseconds)
        }
    }
}
