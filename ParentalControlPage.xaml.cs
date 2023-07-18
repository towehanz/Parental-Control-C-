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
using System.Windows.Resources;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Diagnostics;

namespace DigitalWellbeingWPF.Views

{
    /// <summary>
    /// Interaction logic for ParentalControlPage.xaml
    /// </summary>
    public partial class ParentalControlPage : Page
    {

        public static string todo;
        public static int timeLeft;
        public static int hour;
        public static int min;
        public static int sec;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer lockTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer shutdownTimer = new System.Windows.Forms.Timer();

        [DllImport("user32")]
        public static extern void LockWorkStation();
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint Flags, uint Reason);
        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);




        public ParentalControlPage()
        {

            InitializeComponent();
            grid_Timer.IsEnabled = false;
            timer.Interval = 1000;
            lockTimer.Interval = 1000;
            shutdownTimer.Interval = 1000;
            lockTimer.Tick += new EventHandler(timer_tickLock);
            shutdownTimer.Tick += new EventHandler(timer_tickShutdown);




        }



        private void timer_tickShutdown (object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft = timeLeft - 1;
                hour = timeLeft / 3600;
                min = (timeLeft - (hour * 3600)) / 60;
                sec = timeLeft - (hour * 3600) - (min * 60);
                hours.Text = hour.ToString();
                minutes.Text = min.ToString();
                seconds.Text = sec.ToString();
                hours.IsEnabled = seconds.IsEnabled = minutes.IsEnabled = false;
            }
            else
            {
                shutdownTimer.Stop();
                Process.Start("shutdown", "/s /f /t 0");
            }
        }
        private void timer_tickLock(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                
                timeLeft = timeLeft - 1;
                hour = timeLeft / 3600;
                min = (timeLeft - (hour * 3600)) / 60;
                sec = timeLeft - (hour * 3600) - (min * 60);
                hours.Text = hour.ToString();
                minutes.Text = min.ToString();
                seconds.Text = sec.ToString();
                hours.IsEnabled = seconds.IsEnabled = minutes.IsEnabled = false;
            }
            else
            {
                lockTimer.Stop();
                LockWorkStation();
                hours.IsEnabled = seconds.IsEnabled = minutes.IsEnabled = true;


            }
        } 



        private void shutdown_Selected(object sender, RoutedEventArgs e)
        {
            todo = "shutdown";
        }

        private void lock_Selected (object sender, RoutedEventArgs e)
        {
            todo = "lock";
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
           BlockApps blockApps = new BlockApps();

        blockApps.Show();
        }

    
        private void btn_blockApps_Click(object sender, RoutedEventArgs e)
        {
            BlockApps blockApps = new BlockApps();
            blockApps.Show();
        }

       

        private void ToggleSetTimer_Toggled(object sender, RoutedEventArgs e)
        {


            if (ToggleSetTimer.IsOn)
            {
                grid_Timer.IsEnabled = true;
              hours.IsEnabled = ToggleSetTimer.IsOn;
                minutes.IsEnabled = ToggleSetTimer.IsOn;
                  seconds.IsEnabled = ToggleSetTimer.IsOn;
            }
            else
            {
                grid_Timer.IsEnabled = false;
                lockTimer.Stop();
                shutdownTimer.Stop();
            }
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if(hours.Text == "")
                {
                    hours.Text = "0";
                }
                if (minutes.Text == "")
                {
                    minutes.Text = "0";
                }
                if (seconds.Text == "")
                {
                    seconds.Text = "0";
                }
                if (todo.Equals("shutdown"))
                {
                     try
                    {
                        hour = Int16.Parse(hours.Text);
                        min = Int16.Parse(minutes.Text);
                        sec = Int16.Parse(seconds.Text);
                        timeLeft = hour * 3600 + min * 60 + sec;
                        shutdownTimer.Start();
                        comboBox_options.IsEnabled = false;
                        btn_Start.IsEnabled = false;



                    }
                    catch
                    {
                        MessageBox.Show("Incorrect Time Format!");
                    }
                }

                if (todo.Equals("lock"))
                {
                   
                    try
                    {
                        hour = Int16.Parse(hours.Text);
                        min = Int16.Parse(minutes.Text);
                        sec = Int16.Parse(seconds.Text);
                        timeLeft = hour * 3600 + min * 60 + sec;
                        lockTimer.Start();
                        comboBox_options.IsEnabled = false;
                        btn_Start.IsEnabled = false;
                    }
                    catch
                    {
                        MessageBox.Show("Incorrect Time Format!");
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                Console.Write(ex.Message);
            }
        
            
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            lockTimer.Stop();
            shutdownTimer.Stop();
            comboBox_options.IsEnabled = true;
            btn_Start.IsEnabled = true;
            hours.IsEnabled = seconds.IsEnabled = minutes.IsEnabled = true;

        }
        private void timer_tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {

                timeLeft = timeLeft - 1;
                hour = timeLeft / 360;
                min = (timeLeft - (hour * 3600)) / 60;
                sec = timeLeft - (hour * 3600) - (min * 60);
                hours.Text = hour.ToString();
                minutes.Text = min.ToString();
                seconds.Text = sec.ToString();
                hours.IsEnabled = seconds.IsEnabled = minutes.IsEnabled = false;

            }
            else
            {

                timer.Stop();

                Process.Start("shutdown", "/s /f /t 0");


            }

        }
    }
}
