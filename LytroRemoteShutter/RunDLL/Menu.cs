using System;

namespace RunDLL
{
    class Menu
    {
        public Tuple<string, int, string[], int, string, string, string> menu() // home screen
        {

            Console.WriteLine("Lytro Remote Shutter\n");

            if (!System.IO.File.Exists("config.txt"))
            {
                Console.WriteLine("Please create a 'config.txt' then re-run.");
                Console.WriteLine("Press 'H' for an example 'config' file, or press any other key to exit");
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.H)
                {
                    // help function
                }
                else
                {
                    Environment.Exit(1);
                }
            }

            string[] lines = System.IO.File.ReadAllLines(@"config.txt");
            int use = Convert.ToInt32(lines[8]); // 9th line is use
            string download = lines[13]; // 14th line is download (Y/N)
            string choices = lines[28]; // 29th line is output file choices
            string delete = lines[19]; // 20th line is delete choice
            int length = Convert.ToInt32(lines[34]); // 35th line is video length   
            string calib = lines[39]; // 40th line is Lytro-UR5 calibration choice
            string calib2 = lines[45]; // 46th line is Lytro calibration choice
            
            string[] words = choices.Split(',');
            Console.WriteLine("A presets file has been detected. Press ENTER to begin");
            Console.ReadLine();

            return Tuple.Create(download, use, words, length, delete, calib, calib2);
        }
    }
}
