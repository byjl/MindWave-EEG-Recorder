using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Algorithms;


namespace MindWaveEEGRecorder
{
    class Program
    {
        static Connector connector;

        string sourcePath;
        string targetPath;
        string sourceFile;
        string destFile;

        static string testName;
        static string testDate;
        static string testComments;
        static string portName;

        static string path;
        static uint EegPowerMax = 0;
        static decimal TaskFamiliarityMax = 0;
        static decimal TaskFamiliarityMin = -1000;
        static decimal MentalEffortMax = 0;
        static decimal MentalEffortMin = 1000;
        static ulong AttentionTot = 0;
        static ulong MeditationTot = 0;
        static uint AttentionCount = 0;
        static uint MeditationCount = 0;
        static decimal MentalEffortTot = 0;
        static decimal TaskFamiliarityTot = 0;
        static uint DeltaTot = 0;
        static uint ThetaTot = 0;
        static uint Alpha1Tot = 0;
        static uint Alpha2Tot = 0;
        static uint Beta1Tot = 0;
        static uint Beta2Tot = 0;
        static uint Gamma1Tot = 0;
        static uint Gamma2Tot = 0;
        static uint MentalEffortCount = 0;
        static uint TaskFamiliarityCount = 0;
        static uint EegPowerCount = 0;


        public static void Main(string[] args)
        {
            Console.Write("This program connects to your NeuroSky MindWave EEG headset to record and display the data. Turn your headset on, ensure the USB dongle is plugged in, and make sure the ThinkGear Connector is running. Also ensure that the MindWave Manager recognizes your headset. " + Environment.NewLine + Environment.NewLine);

            Console.Write("Enter Test Name: ");
            testName = Console.ReadLine();
            testDate = DateTime.Now.ToString("M/d/yyyy");
            path = "Tests\\" + testName + "-" + DateTime.Now.ToString("M-d-yyyy") + "\\";

            

            Console.Write("Enter summary of the test you are performing: ");
            testComments = Console.ReadLine();

            Console.WriteLine(Environment.NewLine + "When ready, enter the port name below and press Enter. If you're unsure, leave it blank and all ports will be scanned." + Environment.NewLine);
            Console.Write("Enter port name (e.g. COM4): ");
            portName = Console.ReadLine();

            // Initialize a new Connector and add event handlers
            connector = new Connector();
            connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            connector.DeviceConnectFail += new EventHandler(OnDeviceFail);
            connector.DeviceValidating += new EventHandler(OnDeviceValidating);

            // Scan for devices across COM ports
            // The COM port named will be the first COM port that is checked.
            connector.ConnectScan(portName);

            connector.setBlinkDetectionEnabled(false);
            connector.setMentalEffortEnable(true);
            connector.setMentalEffortRunContinuous(true);
            connector.setTaskFamiliarityEnable(true);
            connector.setTaskFamiliarityRunContinuous(true);

            while (true)
            {
                Console.Write("Comment: ");
                string runComment = Console.ReadLine();
                if (runComment != null)
                {
                    if (runComment == "end")
                    {
                        File.AppendAllText(path + "info.txt", "|" + EegPowerMax);
                        connector.Close();
                        Environment.Exit(0);
                    }
                    else
                    {
                        TimeSpan tt = DateTime.UtcNow - new DateTime(1970, 1, 1);
                        int secondsSinceEpoch = (int)tt.TotalSeconds;
                        File.AppendAllText(path + "comments.txt", secondsSinceEpoch + "|" + runComment + "\n");
                    }
                }
            }

            Thread.Sleep(43200000);

            connector.Close();
            Environment.Exit(0);

        }

        // Called when a device is connected 
        static void OnDeviceConnected(object sender, EventArgs e)
        {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;
            Console.WriteLine("Device found on: " + de.Device.PortName);

            try
            {
                // Determine whether the directory exists. 
                if (Directory.Exists(path))
                {
                    Console.WriteLine("That path exists already. Restart the program and enter a different Test Name.");
                    Thread.Sleep(2000);
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine("The process failed: {0}", ex.ToString());
            }
            finally { }


            File.AppendAllText(path + "info.txt", testName + "|" + testDate + "|" + testComments);
            
            try
            {
                string fileName = "results.html";
                string sourcePath = "";
                string targetPath = path;
                string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileName);
                System.IO.File.Copy(sourceFile, destFile, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong!");
            }
            finally { }


            Console.WriteLine("Recording Started.");

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("To end the test, type \"end\" into the comment prompt and press Enter.");
            Console.ResetColor();

            Console.Write("Comment: ");
            de.Device.DataReceived += new EventHandler(OnDataReceived);
        }

        // Called when scanning fails
        static void OnDeviceFail(object sender, EventArgs e)
        {
            Console.WriteLine("No devices found! :(");
        }

        // Called when each port is being validated
        static void OnDeviceValidating(object sender, EventArgs e)
        {
            Console.WriteLine("Validating: ");
        }

        // Called when data is received from a device
        static void OnDataReceived(object sender, EventArgs e)
        {
            
            Device.DataEventArgs de = (Device.DataEventArgs)e;
            DataRow[] tempDataRowArray = de.DataRowArray;

            TGParser tgParser = new TGParser();
            tgParser.Read(de.DataRowArray);

            string[] keys = { "PoorSignal", "Attention", "Meditation", "MentalEffort", "TaskFamiliarity", "EegPowerDelta", "EegPowerTheta", "EegPowerAlpha1", "EegPowerAlpha2", "EegPowerBeta1", "EegPowerBeta2", "EegPowerGamma1", "EegPowerGamma2" };
            for (int i = 0; i < tgParser.ParsedData.Length; i++)
            {
                string row = tgParser.ParsedData[i]["Time"] + " ";
                bool trip = false;
                foreach (string key in keys)
                {
                    if (tgParser.ParsedData[i].ContainsKey(key))
                    {
                        trip = true;
                        row += tgParser.ParsedData[i][key] + " ";

                        if (key == "PoorSignal" && tgParser.ParsedData[i][key] > 0)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("There is a poor signal. Please adjust headset or check battery.");
                            Console.Write("Comment:");
                        }
                        if (key == "Attention")
                        {
                            AttentionTot += (uint)tgParser.ParsedData[i][key];
                            AttentionCount++;
                        }
                        if (key == "Meditation")
                        {
                            MeditationTot += (uint)tgParser.ParsedData[i][key];
                            MeditationCount++;
                        }
                        string subStr = key.Substring(0, 8);
                        if (subStr == "EegPower")
                        {
                            if (EegPowerMax < tgParser.ParsedData[i][key])
                            {
                                EegPowerMax = (uint)tgParser.ParsedData[i][key];
                            }
                            if (key == "EegPowerDelta")
                            {
                                EegPowerCount++;
                                DeltaTot += (uint)tgParser.ParsedData[i][key];
                                ThetaTot += (uint)tgParser.ParsedData[i]["EegPowerTheta"];
                                Alpha1Tot += (uint)tgParser.ParsedData[i]["EegPowerAlpha1"];
                                Alpha2Tot += (uint)tgParser.ParsedData[i]["EegPowerAlpha2"];
                                Beta1Tot += (uint)tgParser.ParsedData[i]["EegPowerBeta1"];
                                Beta2Tot += (uint)tgParser.ParsedData[i]["EegPowerBeta2"];
                                Gamma1Tot += (uint)tgParser.ParsedData[i]["EegPowerGamma1"];
                                Gamma2Tot += (uint)tgParser.ParsedData[i]["EegPowerGamma2"];
                            }
                        }
                    }
                    else
                    {
                        row += "- ";
                    }
                }
                if (trip == true)
                {
                    File.AppendAllText(path + "data.txt", row + "\n");
                }
            }
        }
    }
}
