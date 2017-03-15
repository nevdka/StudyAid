using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Win32;
using System.Configuration;
using System.Collections.Specialized;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Threading;

namespace StudyAid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private int time;

        private ConfigSettings config;
        private NameValueCollection appSettings;
        private DispatcherTimer dispatcherTimer;

        private int percentComplete;
        private List<string> messages;
        private Random rand = new Random();


        public MainWindow()
        {
            InitializeComponent();

            appSettings = ConfigurationManager.AppSettings;

            config = new ConfigSettings()
            {
                imageFileName = appSettings["ImageFilePath"],
                textFileName = appSettings["TextFilePath"],
                time = appSettings["Time"]
            };

            tbPicturePath.Text = config.imageFileName;
            tbTextFilePath.Text = config.textFileName;
            tbTime.Text = config.time;
            loadImage(config.imageFileName);
            checkIfEverythingIsValid();

            percentComplete = 0;
        }

        private void btnBrowsePicture_Click( object sender, RoutedEventArgs e )
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".jpg";
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            openFileDialog.RestoreDirectory = true;

            if(openFileDialog.ShowDialog() == true)
            {
                try
                {
                    loadImage( openFileDialog.FileName );
                }
                catch (Exception ex)
                {
                    imPicture.Source = null;
                    MessageBox.Show( $"Error: Could not read image from disk. Original error: {ex.Message}" );
                }
            }

            checkIfEverythingIsValid();
        }

        private void loadImage( string fileName )
        {
            // Check if image can load successfully
            if ( !File.Exists( fileName ) )
                return;

            var image = new BitmapImage( new Uri( fileName ) );

            // All good. Set the path.
            tbPicturePath.Text = fileName;
            imPicture.Source = image;
            config.imageFileName = tbPicturePath.Text;
        }

        private void btnBrowseTextFile_Click( object sender, RoutedEventArgs e )
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text Files (*.txt)|*.txt";
            openFileDialog.RestoreDirectory = true;

            if(openFileDialog.ShowDialog() == true)
            {
                try
                {
                    tbTextFilePath.Text = openFileDialog.FileName;
                    config.textFileName = tbTextFilePath.Text;
                    messages = new List<string>( File.ReadAllLines( config.textFileName ) );
                }
                catch (Exception ex)
                {
                    MessageBox.Show( $"Error: Could not read text file from disk. Original error: {ex.Message}" );
                }
            }

            checkIfEverythingIsValid();
        }

        private void btnStart_Click( object sender, RoutedEventArgs e )
        {
            // update config
            var configFile = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None );
            var settings = configFile.AppSettings.Settings;
            settings["ImageFilePath"].Value = config.imageFileName;
            settings["TextFilePath"].Value = config.textFileName;
            settings["Time"].Value = config.time;
            configFile.Save( ConfigurationSaveMode.Modified );
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

            messages = new List<string>( File.ReadAllLines( config.textFileName ) );

            Visibility = Visibility.Hidden;
            ShowInTaskbar = false;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler( dispatcherTimer_Tick );
            dispatcherTimer.Interval = new TimeSpan( 0, time, 0);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e )
        {
            bool last = false;
            string messageText = "";

            if (cbUseText.IsChecked == true )
            {
                int index = rand.Next( messages.Count );
                messageText = messages[index];
            }
            else
            {
                percentComplete += 25;
                if (percentComplete >= 100 )
                {
                    last = true;
                    dispatcherTimer.Stop();
                }
                messageText = $"{percentComplete}% done!";
            }
            new Window1( imPicture.Source, messageText, last).Show();
        }

        private void checkIfEverythingIsValid()
        {
            parseTimeToMinutes();
            if(validateTime() && validateImage() && validateTextFile() )
            {
                btnStart.IsEnabled = true;
            }
            else
            {
                btnStart.IsEnabled = false;
            }
        }

        private bool validateTextFile()
        {
            return File.Exists( tbTextFilePath.Text );
        }

        private bool validateImage()
        {
            return imPicture.Source != null;
        }

        private void tbTime_LostFocus( object sender, EventArgs e )
        {
            parseTimeToMinutes();

            checkIfEverythingIsValid();
        }

        private bool validateTime()
        {
            if ( time > 0 )
                return true;
            else
                return false;
        }

        private void parseTimeToMinutes()
        {
            var match = Regex.Match( tbTime.Text, "^(?<hours>\\d+):(?<minutes>[0-5]?\\d)$" );
            if ( match.Success )
            {
                time = Int32.Parse( match.Groups["hours"].Value ) * 60 + Int32.Parse( match.Groups["minutes"].Value );
                config.time = tbTime.Text;
            }
            else
            {
                tbTime.Text = "h:mm";
                time = 0;
            }
            
        }
    }
}
