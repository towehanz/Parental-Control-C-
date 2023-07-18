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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DigitalWellbeing.Core;
using System.IO;
using System.Speech;
using System.Speech.Synthesis;
using MessageBox = System.Windows.Forms.MessageBox;
using DigitalWellbeingWPF.Helpers;
using Path = System.IO.Path;

namespace DigitalWellbeingWPF.Views
{
    /// <summary>
    /// Interaction logic for JournalPage.xaml
    /// </summary>
    public partial class JournalPage : Page
    {

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hwnd, StringBuilder ss, int count);

        Timer tmr = new Timer();


        private string folderPath;

        SpeechSynthesizer spsyn  = new SpeechSynthesizer();
        public JournalPage()
        {
            InitializeComponent();

            folderPath = ApplicationPath.DetailedLogsFolder;
            tmr.Interval = 3000;
            // tmr.Tick += Tmr_Tick;
            
           
          //  tmr.Stop();
         //   DetailedLogsTimer();
            
           tmr.Tick += LoadDetailedLogs;
            btn_speakPause.IsEnabled = false;
            btn_speakResume.IsEnabled = false;
            btn_speakStop.IsEnabled = false;
            tmr.Start();
           // LoadDetailedLogs();
        }

       

        private void Tmr_Tick(object sender, EventArgs e)
        {
            string title = ActiveWindowTitle();
            string filePath = folderPath + @"detailedLogs.log";
            

            try
            {
               
                if (title != null)
                {
                    string[] detailedLogs = new string[] { DateTime.Now.ToString("dddd, MMMM dd yyyy" + " " + "hh:mm:ss") + " - " + title };
                    //detailedLogs.Items.Add(DateTime.Now.ToString("dddd, MMMM dd yyyy" + " " + "hh:mm:ss") + " - " + title);

               

                  //  string lines = DateTime.Now.ToString("dddd, MMMM, dd yyyy" + " " + "hh:mm:ss") + " - " + title;
                    
                    //string[] dLines = new string[] {  lines };

                   
                    File.AppendAllLines(filePath, detailedLogs);                   
                }
            }
            catch (FileNotFoundException)
            {
                File.Create(filePath);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(folderPath);
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
            

        }
        private void DetailedLogsTimer()
        {
          
            if(spsyn.State == SynthesizerState.Ready || spsyn.State == SynthesizerState.Paused)
            {
                tmr.Start();
            }

            else
            {
                tmr.Stop();
            }
        }

      private void LoadDetailedLogs(object sender, EventArgs e)
        {
            string dLogsToLoad = folderPath + @"detailedLogs.log";
            string title = ActiveWindowTitle();
            List<string> dLogsLines = new List<string>();
            

            try
            {
                detailedLogs.Items.Clear();



                if (title != null)
                {
                    string[] detailedLogs = new string[] { $"{ DateTime.Now.ToString("dddd, MMMM dd yyyy" + " " + "hh:mm:ss") + " - " + title}" };
                   dLogsLines.Add(detailedLogs.ToString());
                    File.AppendAllLines(dLogsToLoad, detailedLogs);
                    
                }


                dLogsLines = File.ReadAllLines(dLogsToLoad).ToList();

              



                foreach (string line in dLogsLines)
                {
                      detailedLogs.Items.Add(line);
                  //  detailedLogs.ItemsSource = line;


                    // detailedLogs.ItemsSource = line.;
                    //   File.WriteAllLines(dLogsToLoad, dLogsLines);

                }
            }
            catch (FileNotFoundException)
            {
                File.Create(folderPath + @"detailedLogs.log");
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(folderPath);
            }
            catch (UnauthorizedAccessException)
            {
                FileAttributes attributes = File.GetAttributes(dLogsToLoad);
                if ((attributes = FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(dLogsToLoad, attributes);
                    File.Delete(dLogsToLoad);
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

        private string ActiveWindowTitle()
        {
            const int nChar = 256;
            StringBuilder sb = new StringBuilder(nChar);

            IntPtr handle = IntPtr.Zero;
            handle = GetForegroundWindow();


            if (GetWindowText(handle, sb, nChar) > 0) return sb.ToString();
            else return "";
        }

        private void btn_Speak_Click(object sender, RoutedEventArgs e)
        {
            string speechdLogs = folderPath + @"detailedLogs.log";
            string speechdLogsToLoad = File.ReadAllText(speechdLogs);
         
            
            

            if (detailedLogs!= null)
            {   

                btn_Speak.IsEnabled = false;
                btn_speakPause.IsEnabled = true;
                btn_speakStop.IsEnabled = true;
                btn_speakResume.IsEnabled = false;
                gridClear.IsEnabled = false;
                spsyn.Dispose();
                spsyn = new SpeechSynthesizer();              
                spsyn.SpeakAsync(speechdLogsToLoad);

           
                //spsyn.Speak((string)detailedLogs.SelectedItem);
            }
            else
            {
                MessageBox.Show("detailedLogs are empty");
            }
        }

        private void btn_speakPause_Click(object sender, RoutedEventArgs e)
        {
            if (spsyn!= null)
            {
                if (spsyn.State == SynthesizerState.Speaking)
                {
                    btn_Speak.IsEnabled = false;
                    btn_speakPause.IsEnabled = false;
                    btn_speakStop.IsEnabled = true;
                    btn_speakResume.IsEnabled = true;
                    spsyn.Pause();
                }
            }
        }

        private void btn_speakResume_Click(object sender, RoutedEventArgs e)
        {
            if (spsyn != null)
            {
                if (spsyn.State == SynthesizerState.Paused)
                {
                    btn_Speak.IsEnabled = false;
                    btn_speakPause.IsEnabled = true;
                    btn_speakStop.IsEnabled = true;
                    btn_speakResume.IsEnabled = false;
                    spsyn.Resume();
                }
            }
        }

        private void btn_speakStop_Click(object sender, RoutedEventArgs e)
        {
            if (spsyn != null)
            {
                if (spsyn.State == SynthesizerState.Speaking || spsyn.State == SynthesizerState.Paused || spsyn.State == SynthesizerState.Ready)
                {
                    btn_Speak.IsEnabled = true;
                    btn_speakPause.IsEnabled = false;
                    btn_speakStop.IsEnabled = false;
                    btn_speakResume.IsEnabled = false;
                    spsyn.Dispose();
                    gridClear.IsEnabled = true;
                }
            }
        }

        
        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string temp = folderPath + @"temp.log";
                string[] temp = { "" };
                string filePath = folderPath + @"detailedLogs.log";
                string linesToKeep = File.ReadAllLines(filePath).ToString();

                File.WriteAllLines(filePath, temp);
                // File.Delete(temp);
                // File.Move(temp, filePath);

            }
            catch (FileNotFoundException)
            {
                File.Create(folderPath + @"temp.log");
            }
        }



        /*  private void btn_Search(object sender, RoutedEventArgs e)
          {
              string filePath = folderPath + @"detailedLogs.log";
              try
              {
                  if (textSearch.Text != null)
                  {
                      string read = File.ReadAllText(textSearch.Text);
                      if (filePath.Contains(read))
                      {
                          detailedLogs.Items.Clear();
                          tmr.Stop();
                          detailedLogs.Items.Add(read);

                      }
                      else
                      {
                          tmr.Start();
                      }
                  }
              } catch (IOException ex)
              {
                  Console.WriteLine(ex.ToString());   
              }

          } */



        /*   private void btn_speakClearLogs_Click(object sender, RoutedEventArgs e)
           {
               List<string> s = new List<string>();
               string dLogsToClear = folderPath + @"detailedLogs.log";
               s = File.ReadAllLines(dLogsToClear).ToList();

               foreach (string line in s)
               {
                   dLogsToClear.Replace(line, "");

                   detailedLogs.Items.Remove(line);
               }
               File.AppendAllLines(dLogsToClear, s);

           }*/



    }
}
