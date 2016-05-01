﻿using System;
using System.Windows;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;

namespace DarkSouls3SaveGameBackupTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string saveGameLocation = ""; // Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +  @"\DarkSoulsIII\0110000104cb7884\";

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            btn_end.IsEnabled = false;

            SetDarkSouls3SaveGameLocation();

        }

        private void SetDarkSouls3SaveGameLocation()
        {
            try
            {
                string darkSoulsIIIBaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\DarkSoulsIII\";

                var darkSoulIIISubFolders = Directory.GetDirectories(darkSoulsIIIBaseFolder);

                saveGameLocation = darkSoulIIISubFolders[0] + "\\";

                txtBox_darkSouls3SaveGameLocation.Text = saveGameLocation;
            }
            catch (Exception ex)
            {
                CustomErrorMessageBox("Error! Could not find the Dark Souls 3 directory.");
                CustomErrorMessageBox(ex.ToString());
                txtBox_darkSouls3SaveGameLocation.Text = "Error!";
            }

        }

        private void btn_enableBackUpProcess_Click(object sender, RoutedEventArgs e)
        {
            txtBox_log.AppendText(Environment.NewLine + "Starting backup process..." + Environment.NewLine);
            dispatcherTimer.Tick += dispatcherTimer_Tick;

            int backupInterval = GetTimeIntervalValue();

            dispatcherTimer.Interval = new TimeSpan(0, backupInterval, 0);


            dispatcherTimer.Start();
            btn_startBackUpProcess.IsEnabled = false;
            btn_end.IsEnabled = true;
        }

        private void btn_endBackUpProcess_Click(object sender, RoutedEventArgs e)
        {
            txtBox_log.AppendText(Environment.NewLine + "Stopped backup process..." + Environment.NewLine);
            dispatcherTimer.Stop();

            //???? need to find a way to stop it from duplicating
            //dispatcherTimer.Dispatcher.DisableProcessing();
            btn_startBackUpProcess.IsEnabled = true;
            btn_end.IsEnabled = false;

        }


        // Specify what you want to happen when the Elapsed event is raised.
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            txtBox_log.AppendText("Created a new backup: " + DateTime.Now.ToString() + Environment.NewLine);

            try
            {
                File.Copy(saveGameLocation + "DS30000.sl2", saveGameLocation + DateTime.Now.ToFileTime() + "__DS30000.sl2.bak");
            }
            catch (Exception ex)
            {
                CustomErrorMessageBox(ex.Message);

                if (dispatcherTimer.IsEnabled)
                {
                    dispatcherTimer.Stop();
                }

            }

        }


        private int GetTimeIntervalValue()
        {
            int timeInterval = 0;

            try
            {
                timeInterval = Convert.ToInt32(txtBox_backupInterval.Text);

                if (timeInterval < 1)
                {
                    throw new Exception("Time interval cannot be less than 1 minute!");
                }
                else if (timeInterval > 59)
                {
                    throw new Exception("Time interval cannot be more than 59 minutes!");
                }

                return timeInterval;

            }
            catch (Exception ex)
            {
                //stop timer event
                CustomErrorMessageBox(ex.Message);

                if(dispatcherTimer.IsEnabled)
                {
                    dispatcherTimer.Stop();
                }

                //default back to 15
                txtBox_backupInterval.Text = "15";

                return 15;
            }
        }



        /// <summary>
        /// A custom Error MessageBox that has an Error Icon
        /// </summary>
        /// <param name="errorMessage"></param>
        public void CustomErrorMessageBox(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        /// <summary>
        /// A custom Notification MessageBox with an info icon
        /// </summary>
        /// <param name="notificationMessage"></param>
        public void CustomNotificationMessageBox(string notificationMessage)
        {
            MessageBox.Show(notificationMessage, "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btn_openDarkSouls3GameSavesLocation_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(saveGameLocation + "\\");
        }
    }
}