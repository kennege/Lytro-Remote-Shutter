using System;
using System.Text;
using UAM.Optics.LightField.Lytro.Net;
using UAM.Optics.LightField.Lytro.Camera;
using UAM.Optics.LightField.Lytro.Metadata;
using UAM.InformatiX.Text.Json;
using UAM.Optics.LightField.Lytro.IO;

namespace RunDLL
{
    class Download
    {
        HardwareInfo info = Program.lytronetclient.GetHardwareInfo();

        public void download(int count, string[] words, string delete, string[] calib3)
        {
            string pwd = Environment.CurrentDirectory;
            Console.WriteLine("Downloading...");
            PictureList picturelist = Program.lytronetclient.DownloadPictureList();
            int c = picturelist.Count;
            string sn = info.SerialNumber;

            string calib = calib3[0];

            // if creating new calibration data set, delete old sets if they exist
            if (calib.Equals("Y") || calib.Equals("y"))
            {
                string path = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Cameras/{1}/Calibration_dataset/01/", pwd);
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

                foreach (System.IO.FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (System.IO.DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }

            int a = 0;
            foreach (PictureListEntry entry in picturelist) // download files             
            {
                string str = picturelist[a].ToString();
                string path = picturelist[a].Path.ToString();

                string[] detail = str.Split();
                string ids = detail[0];
                string addres = detail[1];
                addres = addres.Replace("[", "");
                addres = addres.Replace("]", "");
                string[] splits = addres.Split('\\');

                string nameL = String.Format(@"IMG_{0}.LFR", a);
                string datanameL = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}", pwd, nameL);
                string nameR = String.Format(@"IMG_{0}.RAW", a);
                string datanameR = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}", pwd, nameR);
                string nameJ = String.Format(@"IMG_{0}.JPG", a);
                string datanameJ = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}", pwd, nameJ);
                string nameT = String.Format(@"IMG_{0}.TXT", a);
                string datanameT = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}", pwd, nameT);

                if (!System.IO.File.Exists(datanameL) && !System.IO.File.Exists(datanameR) && 
                    !System.IO.File.Exists(datanameT) && !System.IO.File.Exists(datanameJ))
                {
                    foreach (string word in words)
                    {

                        if (word.Equals("j"))
                        {
                            string[] details = str.Split();
                            string id = details[0];
                            string address = details[1];
                            address = address.Replace("[", "");
                            address = address.Replace("]", "");
                            string[] split = address.Split('\\');
                            string name = split[3];
                            byte[] data = Program.lytronetclient.DownloadPicture(id, LoadPictureFormat.Jpeg);
                            if (calib.Equals("Y") || calib.Equals("y"))
                            {
                                string dataname = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Cameras/{1}/Calibration_dataset/01/{2}.jpg", pwd, sn, name);
                                System.IO.File.WriteAllBytes(dataname, data);
                            }
                            else
                            {
                                string dataname = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}.jpg", pwd, name);
                                System.IO.File.WriteAllBytes(dataname, data);
                            }

                        }
                        else if (word.Equals("r"))
                        {
                            string[] details = str.Split();
                            string id = details[0];
                            string address = details[1];
                            address = address.Replace("[", "");
                            address = address.Replace("]", "");
                            string[] split = address.Split('\\');
                            string name = split[3];
                            name = name.Replace(".RAW", "");
                            byte[] data = Program.lytronetclient.DownloadPicture(id, LoadPictureFormat.Raw);
                            if (calib.Equals("Y") || calib.Equals("y"))
                            {
                                string dataname = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Cameras/{1}/Calibration_dataset/01/{2}.raw", pwd, sn, name);
                                System.IO.File.WriteAllBytes(dataname, data);
                            }
                            else
                            {
                                string dataname = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}.raw", pwd, name);
                                System.IO.File.WriteAllBytes(dataname, data);
                            }
                        }
                        else if (word.Equals("m"))
                        {
                            string[] details = str.Split();
                            string id = details[0];
                            string address = details[1];
                            address = address.Replace("[", "");
                            address = address.Replace("]", "");
                            string[] split = address.Split('\\');
                            string name = split[3];
                            name = name.Replace(".RAW", "");
                            byte[] data = Program.lytronetclient.DownloadPicture(id, LoadPictureFormat.Metadata);
                            if (calib.Equals("Y") || calib.Equals("y"))
                            {
                                string dataname = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Cameras/{1}/Calibration_dataset/01/{2}.txt", pwd, sn, name);
                                System.IO.File.WriteAllBytes(dataname, data);
                            }
                            else
                            {
                                string dataname = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}.txt", pwd, name);
                                System.IO.File.WriteAllBytes(dataname, data);
                            }
                        }
                        else if (word.Equals("s"))
                        {
                            string[] details = str.Split();
                            string id = details[0];
                            string address = details[1];
                            address = address.Replace("[", "");
                            address = address.Replace("]", "");
                            string[] split = address.Split('\\');
                            string name = split[3];
                            name = name.Replace(".RAW", "");
                            name = String.Format("{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}.raw", pwd, name);
                            int offset = 0x1C;
                            byte[] data = Program.lytronetclient.DownloadPicture(id, LoadPictureFormat.Stack);
                            string ext = System.IO.Path.GetExtension(path);
                            int index = 1;

                            while (offset + 4 < data.Length)
                            {
                                int length = BitConverter.ToInt32(data, offset); offset += 4;
                                if (offset + length > data.Length)
                                    break;

                                string pathstack = System.IO.Path.ChangeExtension(name, index + ext);
                                using (System.IO.FileStream file = System.IO.File.Create(pathstack))
                                    file.Write(data, offset, length);

                                offset += length;
                                index++;
                            }
                        }
                        else if (word.Equals("L"))
                        {
                            byte[] rawData = Program.lytronetclient.DownloadFile(path);
                            string npath = System.IO.Path.ChangeExtension(path, "TXT");
                            byte[] metaData = Program.lytronetclient.DownloadFile(npath);
                            Json.Root root = new Json.Root();
                            root.LoadFromJson(Encoding.UTF8.GetString(metaData));
                            string name = String.Format(@"IMG_{0}.LFR", a);
                            if (calib.Equals("Y") || calib.Equals("y"))
                            {
                                string savepath = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Cameras/{1}/Calibration_dataset/01/{2}", pwd, sn, name);
                                using (System.IO.FileStream file = System.IO.File.Create(savepath))
                                    LightFieldPackage.FromCameraFiles(root, rawData).WriteTo(file);
                            }
                            else
                            {
                                string savepath = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}", pwd, name);
                                using (System.IO.FileStream file = System.IO.File.Create(savepath))
                                    LightFieldPackage.FromCameraFiles(root, rawData).WriteTo(file);
                            }
                        }
                        else if (word.Equals("J"))
                        {
                            string name = System.IO.Path.ChangeExtension(path, "rawCompressed.jpg");
                            byte[] rawData = Program.lytronetclient.DownloadPictureRawJpeg(path);
                            if (calib.Equals("Y") || calib.Equals("y"))
                            {
                                string savepath = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Cameras/{1}/Calibration_dataset/01/{2}", pwd, sn, name);
                                System.IO.File.WriteAllBytes(savepath, rawData);
                            }
                            else
                            {
                                string savepath = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}", pwd, name);
                                System.IO.File.WriteAllBytes(savepath, rawData);
                            }
                        }
                        else if (word.Equals("R"))
                        {
                            byte[] image = Program.lytronetclient.DownloadFile(path);
                            string[] directory = path.Split('\\');
                            string name = directory[3];
                            if (calib.Equals("Y") || calib.Equals("y"))
                            {
                                string savepath = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Cameras/{1}/Calibration_dataset/01/{2}", pwd, sn, name);
                                System.IO.File.WriteAllBytes(savepath, image);
                            }
                            else
                            {
                                string savepath = String.Format(@"{0}/LFToolbox0.4/LFToolbox0.3_Samples1/Images/Lytro/{1}", pwd, name);
                                System.IO.File.WriteAllBytes(savepath, image);
                            }
                        }
                    }
                    int b = a + 1;
                    string timestamp = picturelist[a].DateTaken.ToString();
                    Console.WriteLine("timestamp: " + timestamp);
                    Console.WriteLine("Downloaded Photo " + b + "/" + c);

                    if (delete.Equals("Y") || delete.Equals("y"))
                    {
                        Program.lytronetclient.DeletePicture(picturelist[a]); // delete photo from camera
                    }
                }
                a = a + 1;
            }
            Console.WriteLine("Download Complete.");
        }
    }
}

