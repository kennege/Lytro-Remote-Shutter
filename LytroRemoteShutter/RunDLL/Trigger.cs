using System;
using System.Diagnostics;

namespace RunDLL
{
    class Trigger
    {

        public int trigger(int use, int length, string calib)
        {

            // initiate timer
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int count = 0;

            if (use == 1) // user has selected continuous video 
            {

                // take photos while time limit is not reached
                while (timer.Elapsed.TotalSeconds < length)
                {
                    // call TakePicture function
                    if (timer.ElapsedMilliseconds % 1500 == 0)
                    {
                        Program.lytronetclient.TakePicture();
                        long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        Console.WriteLine("Timestamp: " + milliseconds);
                        //Thread.Sleep(1500); // sleep for 1.5s (alternative method)
                        count = count + 1;
                    }
                }
                timer.Stop();

                Console.WriteLine("Video Completed. " + count + " photos taken.");
                return count;
            }
            else if (use == 2) // user has selected triggered photos
            {
                while (true)
                {
                    Console.WriteLine("Press SPACE BAR to trigger camera. Press q to finish\n");
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Spacebar)
                    {
                        Program.lytronetclient.TakePicture();
                        long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        //Console.WriteLine("Timestamp: " + milliseconds);
                        count = count + 1;
                    }
                    else if (key == ConsoleKey.Q)
                    {
                        Console.WriteLine("Video Completed. " + count + " photos taken.");
                        break;
                        
                    }
                }
                return count;
            }
            else if (use == 3) // user has selected synchronised use with robot arm
            {
                if (calib.Equals("N") || calib.Equals("n"))
                {
                    count = URCommunication.Communicate(calib);
                }
                return count;
            }
            return count;
        }
    }
}