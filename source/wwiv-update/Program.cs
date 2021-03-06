﻿using System;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security;

namespace WWIVUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            //Declare and Initilize Set Build Number Variables to 0
            string wwivBuild5_0 = "0";
            string wwivBuild5_1 = "0";

            // Initilize Console Input or Update
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            // Fetch Latest Build Number For WWIV 5.1
            WebClient wc = new WebClient();
            string htmlString1 = wc.DownloadString("http://build.wwivbbs.org/jenkins/job/wwiv/lastSuccessfulBuild/label=windows/");
            Match mTitle1 = Regex.Match(htmlString1, "(?:number.*?>)(?<buildNumber1>.*?)(?:<)");
            // Fetch Latest Build Number For WWIV 5.0
            string htmlString2 = wc.DownloadString("https://build.wwivbbs.org/jenkins/job/wwiv_5.0.0/lastSuccessfulBuild/label=windows/");
            Match mTitle2 = Regex.Match(htmlString2, "(?:number.*?>)(?<buildNumber2>.*?)(?:<)");
            if (mTitle1.Success)
            {
                // Initialze Build Numbers from Jenkins
                wwivBuild5_0 = mTitle2.Groups[1].Value;
                wwivBuild5_1 = mTitle1.Groups[1].Value;
            }
            Console.WriteLine(" ");
            Console.WriteLine("WWIV UPDATE v0.9.4 | ßeta");
            Console.WriteLine(" ");
            Console.WriteLine("WARNING! WWIV5TelNet, WWIV and WWIVnet MUST Be Closed Before Proceeding.");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("WWIV 5.1 Latest Successful Build Number: " + wwivBuild5_1);
            Console.WriteLine(" ");
            Console.WriteLine("WWIV 5.0 Latest Successful Build Number: " + wwivBuild5_0);
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.Write("Enter Build Number or Press Enter To Use Latest 5.1: ");
            string useBuild = Console.ReadLine();
            Console.Clear();

            // Check for Running Instances of WWIV Programs
            // bbs.exe
            if (Process.GetProcessesByName("bbs").Length >= 1)
            {
                Console.WriteLine(" ");
                Console.WriteLine("WWIV BBS.EXE is Currently Running! Please Close and Try Again.");
                Console.WriteLine(" ");
                Console.WriteLine("Press Any Key To Restart WWIV Update...");
                Console.ReadKey();
                Main(args);
            }
            // WWIV5TelnetServer.exe
            if (Process.GetProcessesByName("WWIV5TelnetServer").Length >= 1)
            {
                Console.WriteLine(" ");
                Console.WriteLine("WWIV5TelnetServer is Currently Running! Please Close and Try Again.");
                Console.WriteLine(" ");
                Console.WriteLine("Press Any Key To Restart WWIV Update...");
                Console.ReadKey();
                Main(args);
            }
            // binkp networkb.exe
            if (Process.GetProcessesByName("networkb").Length >= 1)
            {
                Console.WriteLine(" ");
                Console.WriteLine("WWIV BINKP.CMD (WWIVnet) is Currently Running! Please Close and Try Again.");
                Console.WriteLine(" ");
                Console.WriteLine("Press Any Key To Restart WWIV Update...");
                Console.ReadKey();
                Main(args);
            }

            // Search For bbs.exe In Default Install Path
            Console.WriteLine(" ");
            Console.WriteLine("Searching for WWIV Working Directory...");
            Console.WriteLine(" ");
            string[] files = Directory.GetFiles(@"C:\wwiv", "bbs.exe", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    Console.WriteLine(file + "  Was Located In The Default WWIV Install Directory");
                    Console.WriteLine(" ");
                }
                else
                {
                    Console.WriteLine(@"WWIV Is Not Installed In The Default Directory of C:\wwiv");
                    Console.WriteLine(" ");
                    Console.WriteLine("WWIV Update Cannot Proceed!");
                    Console.WriteLine(" ");
                    Console.WriteLine("Please Manually Update Your WWIV Install.");
                    Console.WriteLine(" ");
                    Console.WriteLine("Press Any Key To Exit WWIV Update.");
                    Console.ReadKey();
                    Environment.Exit(2);
                }
            }

            // Set Global Strings For Update
            string backupPath = @"C:\wwiv";
            string zipPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Documents\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_wwiv-backup.zip";
            string extractPath = @"C:\wwiv";
            string extractPath2 = Environment.GetEnvironmentVariable("SystemRoot") + @"\System32";
            string buildNumber3;
            {
                if (useBuild == null || string.IsNullOrWhiteSpace(useBuild))
                {
                    buildNumber3 = wwivBuild5_1;
                }
                else
                {
                    buildNumber3 = useBuild;
                }
            }
            string remoteUri;
            {
                if (buildNumber3 == wwivBuild5_1)
                {
                    remoteUri = "http://build.wwivbbs.org/jenkins/job/wwiv/lastSuccessfulBuild/label=windows/artifact/";
                }
                else
                {
                    remoteUri = "http://build.wwivbbs.org/jenkins/job/wwiv/" + buildNumber3 + "/label=windows/artifact/";
                }
            }
            string fileName = "wwiv-build-win-" + buildNumber3 + ".zip", myStringWebResource = null;
            string updatePath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\wwiv-build-win-" + buildNumber3 + ".zip";
            string wwivChanges;
            {
                if (buildNumber3 == wwivBuild5_1)
                {
                    wwivChanges = "http://build.wwivbbs.org/jenkins/job/wwiv/lastSuccessfulBuild/label=windows/changes";
                }
                else
                {
                    wwivChanges = "http://build.wwivbbs.org/jenkins/job/wwiv/" + buildNumber3 + "/label=windows/changes";
                }
            }

            // Create WWIV Backup File With Unique Name
            Console.WriteLine("Back Up WWIV Started...");
            Console.WriteLine(" ");
            ZipFile.CreateFromDirectory(backupPath, zipPath);
            Console.WriteLine(@"Successfully Backed Up WWIV to " + zipPath + "!");
            Console.WriteLine(" ");

            // Fetch Latest Sucessful Build
            WebClient myWebClient = new WebClient();
            myStringWebResource = remoteUri + fileName;
            Console.WriteLine("Begin Fetch for Update Build #" + buildNumber3 + "...");
            Console.WriteLine(" ");
            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
            myWebClient.DownloadFile(myStringWebResource, Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\" + fileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, myStringWebResource);
            Console.WriteLine(" ");
            Console.WriteLine("Fetch Update Complete!");
            Console.WriteLine(" ");

            // Patch Existing WWIV Install
            Console.WriteLine("Begin Update of WWIV to Build #" + buildNumber3 + "...");
            Console.WriteLine(" ");
            using (ZipArchive archive = ZipFile.OpenRead(updatePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        entry.ExtractToFile(Path.Combine(extractPath, entry.FullName), true);
                    }
                    if (entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        entry.ExtractToFile(Path.Combine(extractPath, entry.FullName), true);
                    }
                    if (entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        entry.ExtractToFile(Path.Combine(extractPath2, entry.FullName), true);
                    }
                }
            }
            Console.WriteLine("WWIV Update Complete! | Press Any Key to Launch WWIV and Exit Update...");
            Console.ReadKey();

            // Launch WWIV, WWIVnet and Latest Changes in Browser.
            Environment.CurrentDirectory = @"C:\wwiv";

            //Launch Telnet Server
            ProcessStartInfo telNet = new ProcessStartInfo("WWIV5TelnetServer.exe");
            telNet.WindowStyle = ProcessWindowStyle.Minimized;
            Process.Start(telNet);

            // Launch Local BBS Node 1 with Networking
            // TODO This Refuses to Load. Will Investigate.
            // May look at ways to launch Local Node Via Telnet Server.
            // Process.Start("bbs.exe -N1 -M");
            // Process.Start("bbs.exe", "-N1 -M");

            // Launch binkp.cmd for WWIVnet
            ProcessStartInfo binkP = new ProcessStartInfo("binkp.cmd");
            binkP.WindowStyle = ProcessWindowStyle.Minimized;
            Process.Start(binkP);

            //Launch Latest Realse Changes into Default Browser
            Process.Start(wwivChanges);
        }
    }
}
