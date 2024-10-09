using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

// Go to Debug > WaccaCircle Debug Properties > Application > Change "Console Application" to "Windows Application"  and don't forget to set it back to console for other apps
// Go to Solution Explorer > References, then delete everything
class Program
{
    static string WaccaCircleStartup = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WaccaCircleStartup.exe");
    static string WaccaCircle32 = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WaccaCircle32.exe");
    static string WaccaCircleSDVX = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WaccaCircleSDVX.exe");
    static string WaccaCircleOsu = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "WaccaCircleOsu.exe");
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
                    if (output == "\r\r\n\r\r\n")   // this means we need to launch WaccaCircle => Check if dolphin is launched
                    {

                        // Create a new process to run WMIC command
                        Process process3 = new Process();
                        process3.StartInfo.FileName = "wmic";
                        process3.StartInfo.Arguments = "process where \"name like '%Dolphin%'\" get name, processid";
                        process3.StartInfo.RedirectStandardOutput = true; // Capture the output
                        process3.StartInfo.UseShellExecute = false;
                        process3.StartInfo.CreateNoWindow = true;

                        // Start the process
                        process3.Start();

                        // Read and display the output
                        output = process3.StandardOutput.ReadToEnd();
                        process3.WaitForExit();
                        if (output == "\r\r\n\r\r\n")   // this means we need to launch WaccaCircle => Check if spice is launched
                        {
                            // Create a new process to run WMIC command
                            Process process4 = new Process();
                            process4.StartInfo.FileName = "wmic";
                            process4.StartInfo.Arguments = "process where \"name like '%spice%'\" get name, processid";
                            process4.StartInfo.RedirectStandardOutput = true; // Capture the output
                            process4.StartInfo.UseShellExecute = false;
                            process4.StartInfo.CreateNoWindow = true;

                            // Start the process
                            process4.Start();

                            // Read and display the output
                            output = process4.StandardOutput.ReadToEnd();
                            process4.WaitForExit();
                            if (output == "\r\r\n\r\r\n")  // this means we need to launch WaccaCircle => Check if sdvx is launched
                            {
                                // Create a new process to run WMIC command
                                Process process5 = new Process();
                                process5.StartInfo.FileName = "wmic";
                                process5.StartInfo.Arguments = "process where \"name like '%SOUND VOLTE%'\" get name, processid";
                                process5.StartInfo.RedirectStandardOutput = true; // Capture the output
                                process5.StartInfo.UseShellExecute = false;
                                process5.StartInfo.CreateNoWindow = true;

                                // Start the process
                                process5.Start();

                                // Read and display the output
                                output = process5.StandardOutput.ReadToEnd();
                                process5.WaitForExit();
                                if (output == "\r\r\n\r\r\n")  // this means we need to launch WaccaCircle => Check if osu is launched
                                {
                                    // Create a new process to run WMIC command
                                    Process process6 = new Process();
                                    process6.StartInfo.FileName = "wmic";
                                    process6.StartInfo.Arguments = "process where \"name like '%osu%'\" get name, processid";
                                    process6.StartInfo.RedirectStandardOutput = true; // Capture the output
                                    process6.StartInfo.UseShellExecute = false;
                                    process6.StartInfo.CreateNoWindow = true;

                                    // Start the process
                                    process6.Start();

                                    // Read and display the output
                                    output = process6.StandardOutput.ReadToEnd();
                                    process6.WaitForExit();
                                    if (output == "\r\r\n\r\r\n")  // mercury, dolphin, spice, sdvx, and osu are not launched => We're on the menu!
                                    {
                                        Process.Start(WaccaCircleStartup);
                                    }
                                    else  // osu is launched! launch joystick
                                    {
                                        Process.Start(WaccaCircleOsu);
                                    }
                                }
                                else  // sdvx is launched! launch joystick
                                {
                                    Process.Start(WaccaCircleSDVX);
                                }
                            }
                            else  // spice is launched! launch joystick
                            {
                                Process.Start(WaccaCircleSDVX);
                            }
                        }
                        else  // dolphin is launched! launch joystick
                        {
                            Process.Start(WaccaCircle32);
                        }
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
