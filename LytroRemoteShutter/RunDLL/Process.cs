using System;
using System.Collections.Generic;
using UAM.Optics.LightField.Lytro.Camera;


namespace RunDLL
{
    public class Process
    {
        HardwareInfo info = Program.lytronetclient.GetHardwareInfo();

        public void MatlabCall(string[] calib3)
        {
            string cf = "Calibration_dataset"; // set calibration folder

            string calib = calib3[0];
            string dim = calib3[1];
            string size = calib3[2];
            dim = dim.Replace(' ', ',');
            size = size.Replace(' ', ',');

            string sn = info.SerialNumber;
            string username = Environment.UserName;
            PlatformID os = Environment.OSVersion.Platform;
            string dir = Environment.CurrentDirectory;
            Console.WriteLine("Processing");

            string sourcePath = String.Format(@"C:\Users\{0}\AppData\Local\Lytro\cameras\sn-{1}",username,sn);
            string targetPath = String.Format(@"{0}\LFToolbox0.4\LFToolbox0.3_Samples1\Cameras\{1}\WhiteImages",dir,sn);

            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
                Console.WriteLine("White Image files not yet copied. Copying...");
                // Copy white image database if it hasn't already been copied
            }
            else
            {
                Console.WriteLine("White Image files detected");
            }
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string fileName = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            else
            {
                Console.WriteLine("Could not find White Image files."); 
                Console.WriteLine("Plug and unplug the camera into this computer and re-run the program");
                System.Threading.Thread.Sleep(3000);
                //System.Environment.Exit(0);
            }

            // ---- SET UP MATLAB TOOLBOX ----
            Console.WriteLine("Setting up Matlab Light Field Toolbox...");
            Console.WriteLine(Program.matlab.Execute(String.Format(@"cd {0}\LFToolbox0.4\", dir))); // Change to the directory where the function is located     
            Console.WriteLine(Program.matlab.Execute(@"run('LFMatlabPathSetup')")); // sets up path - must be re-run whenever Matlab restarts
       
            // --- BUILD A WHITE IMAGE DATABASE ----
            Console.WriteLine("Building White Image Database...");
            Console.WriteLine(Program.matlab.Execute(String.Format(@"cd {0}\LFToolbox0.4\LFToolbox0.3_Samples1\", dir))); // cd into samples folder
            Console.WriteLine(Program.matlab.Execute(@"run('LFUtilUnpackLytroArchive')")); // unpack Lytro white image data
            Console.WriteLine(Program.matlab.Execute(@"run('LFUtilProcessWhiteImages')")); // build a white image database
            //Console.WriteLine("...White Image Database built successfully");

            // ---- DECODE THE LIGHT FIELD FILES ----
            Console.WriteLine("Decoding light field files...");
            Console.WriteLine(Program.matlab.Execute(@"run('LFUtilDecodeLytroFolder')"));
            //Console.WriteLine("...Light field files decoded successfully");

            // ---- PERFORM CALIBRATION ----
            Console.WriteLine("Calibrating files...");
        
            Console.WriteLine(Program.matlab.Execute(String.Format(@"LFUtilDecodeLytroFolder('Cameras/{0}/{1}/')", sn, cf))); // decode calibration data
            Console.WriteLine(Program.matlab.Execute(String.Format(@"CalOptions.ExpectedCheckerSize = {0}",dim)));
            Console.WriteLine(Program.matlab.Execute(String.Format(@"CalOptions.ExpectedCheckerSpacing_m = 1e-3*{0}",size)));
            Console.WriteLine(Program.matlab.Execute(String.Format(@"LFUtilCalLensletCam('Cameras/{0}/{1}/', CalOptions)", sn, cf))); // run calibration
            Console.WriteLine("...Calibration completed successfully");

            // ---- VALIDATE & RECTIFY ----
            Console.WriteLine("Rectifying files...");
            Console.WriteLine(Program.matlab.Execute(@"LFUtilProcessCalibrations")); // validate calibration
            Console.WriteLine(Program.matlab.Execute(@"DecodeOptions.OptionalTasks = 'Rectify'")); // rectify based on calibration
            Console.WriteLine(Program.matlab.Execute(String.Format(@"LFUtilDecodeLytroFolder('Images/{0}',[], DecodeOptions); % re-decode",cf))); // re-decode
            Console.WriteLine("...Rectification completed successfully.");

            // ---- CLEAN UP ----
            //Console.WriteLine("Validating rectification...");
            //string rootFolderPath = String.Format(@"{0}\LFToolbox0.4\LFToolbox0.3_Samples1\Cameras\{1}\{2}\01", dir, sn,cf);
            //string filesToDelete = @".json";   // Only delete json files 
            //string[] fileList = System.IO.Directory.GetFiles(rootFolderPath, filesToDelete);
            //foreach (string file in fileList)
            //{
            //    Console.WriteLine(file);
             //   System.IO.File.Delete(file);
            //}
            //Console.WriteLine(matlab.Execute(@"LFUtilProcessCalibrations")); // re-run calibration
            //Console.WriteLine("...Rectification validated");

            Console.WriteLine(String.Format(@"Your decoded and calibrated files are available at: {0}\LFToolbox0.4\LFToolbox0.3_Samples1\Images\Lytro", dir));

            }
    }
}
