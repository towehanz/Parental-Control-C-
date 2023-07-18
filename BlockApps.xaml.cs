using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.IO;
using DigitalWellbeing.Core;
using DigitalWellbeingWPF.Helpers;


namespace DigitalWellbeingWPF.Views
{
    /// <summary>
    /// Interaction logic for BlockApps.xaml
    /// </summary>
    public partial class BlockApps : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        private string folderPath;

        public BlockApps()
        {
            folderPath = ApplicationPath.ActivityLogsFolder;
           
            Debug.WriteLine(folderPath);

            InitializeComponent();
            timer.Tick += new EventHandler(KillProcess);
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Start();
            LoadBlockedApps();

        }
        public void KillProcess(object Source, EventArgs e)
        {
            string filePath = folderPath + @"blocklist.log";



            try
            {
               
                string readText = File.ReadAllText(filePath);
                for (int i = 0; i < readText.Length; i++)
                {
                    foreach (Process process in Process.GetProcesses())
                    {
                        string name = process.ProcessName.ToLower();
                        if (readText.Contains(name))
                        {

                            process.Kill();
                        }
                       
                    }
                }


            } catch(FileNotFoundException )
            {

                File.Create(filePath);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(folderPath + @"blocklist.log");
            }
           

            catch (UnauthorizedAccessException)
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                if ((attributes = FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(filePath, attributes);
                    File.Delete(filePath);
                }
                else
                {
                    throw;
                }

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }







        }

      
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string files = folderPath + @"blocklist.log";
            try
            {
                string[] lines = { applist.Text };
                
                File.AppendAllLines(files, lines);
                applist.Clear();
                LoadBlockedApps();
            }
            catch (FileNotFoundException)
            {
                File.Create(files);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(folderPath + @"blocklist.log");
            }

           catch (UnauthorizedAccessException)
            {
                FileAttributes attributes = File.GetAttributes(files);
                if ((attributes = FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(files, attributes);
                    File.Delete(files);
                }
                else
                {
                    throw;
                }

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        private void LoadBlockedApps()
        {
            string blockapps = folderPath + @"blocklist.log";
            try
            {
                //blockList.Items.Clear();        
                List<string>  lines = new List<string>();
                lines = File.ReadAllLines(blockapps).ToList();
                // string readblock = File.ReadAllText(blockapps);
                //for (int i =0; i < readblock.Length; i++)
                //{
                //  blockList.ItemsSource = readblock;
                //}
                
                foreach (string line in lines)
                {
                    blockList.ItemsSource = lines;
                }
            }
            catch (FileNotFoundException)
            {
                File.Create(folderPath + @"blocklist.log");
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(folderPath);
            }
            catch (UnauthorizedAccessException)
            {
                FileAttributes attributes = File.GetAttributes(blockapps);
                if ((attributes = FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(blockapps, attributes);
                    File.Delete(blockapps);
                }
                else
                {
                    throw;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void clearBlocked_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string temp = folderPath + @"temp.log";
                string[] temp = { "" };
                string filePath = folderPath + @"blocklist.log";
                string linesToKeep = File.ReadAllLines(filePath).ToString();

                File.WriteAllLines(filePath, temp);
                LoadBlockedApps();
                
                // File.Delete(temp);
                // File.Move(temp, filePath);

            }
            catch (FileNotFoundException)
            {
                File.Create(folderPath + @"temp.log");
            }
        }
    }
}
