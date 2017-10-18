using System;
using UAM.Optics.LightField.Lytro.Net;

namespace RunDLL
{

    public class Program
    {

        private static string download;
        private static int use;
        private static string[] words;
        private static int length;
        private static string delete;
        private static string calib;
        private static string calib2;
        public static LytroNetClient lytronetclient = new LytroNetClient();
        public static MLApp.MLApp matlab = new MLApp.MLApp();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {

            
            Menu m = new Menu();
            Tuple<string, int, string[], int, string, string, string > t = m.menu(); // call Menu function

            download = t.Item1;
            use = t.Item2;
            words = t.Item3;
            length = t.Item4;
            delete = t.Item5;
            calib = t.Item6;
            calib2 = t.Item7;

            string[] calib3 = calib2.Split(',');

            Trigger trig = new Trigger();
            int count = trig.trigger(use, length, calib); // take photos

            if (download.Equals("Y") || download.Equals("y"))
            {
                //if (count > 0)
                //{
                    Download d = new Download();
                    d.download(count, words, delete, calib3);
                //}

                Process p = new Process();
                p.MatlabCall(calib3);
            }

            Console.WriteLine("Program Finished. ENTER to exit.");
            Console.ReadLine();
        }
    }
}
