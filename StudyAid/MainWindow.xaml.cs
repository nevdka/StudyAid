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
        public int GoalTime { get; set; }
        public int PopUpTime { get; set; }

        private ConfigSettings config;
        private NameValueCollection appSettings;

        private List<string> messages;
        private Random rand = new Random();

        public ImageSource source { set; get; }


        public MainWindow()
        {
            appSettings = ConfigurationManager.AppSettings;
            config = new ConfigSettings()
            {
                ImageFileName = appSettings["ImageFilePath"],
                TextFileName = appSettings["TextFilePath"],
                GoalTime = appSettings["GoalTime"],
                PopUpTime = appSettings["PopUpTime"],
                UsePercentages = false,
                StudyMode = false,
                UseRandomText = true
            };
            InitializeComponent();

            tbPicturePath.Text = config.ImageFileName;
            tbTextFilePath.Text = config.TextFileName;
            tbTime.Text = config.GoalTime;
            loadImage(config.ImageFileName);
            checkIfEverythingIsValid();

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
            config.ImageFileName = tbPicturePath.Text;
            source = image;
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
                    config.TextFileName = tbTextFilePath.Text;
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
            settings["ImageFilePath"].Value = config.ImageFileName;
            settings["TextFilePath"].Value = config.TextFileName;
            settings["GoalTime"].Value = config.GoalTime;
            settings["PopUpTime"].Value = config.PopUpTime;
            configFile.Save( ConfigurationSaveMode.Modified );
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

            messages = new List<string>( File.ReadAllLines( config.TextFileName ) );

            Visibility = Visibility.Hidden;


            openWindow();
        }

        private void openWindow()
        {
            new Window1( this, config ).Show();
        }

        private void checkIfEverythingIsValid()
        {
            parseTimeToMinutes();
            ParsePopUpTime();
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
            if ( GoalTime > 0 )
                return true;
            else
                return false;
        }

        private void parseTimeToMinutes()
        {
            var match = Regex.Match( tbTime.Text, "^(?<hours>\\d+):(?<minutes>\\d+)$" );
            if ( match.Success )
            {
                GoalTime = Int32.Parse( match.Groups["hours"].Value ) * 60 + Int32.Parse( match.Groups["minutes"].Value );
                config.GoalTime = tbTime.Text;
                config.GoalTimeMins = GoalTime;
            }
            else
            {
                tbTime.Text = "h:mm";
                GoalTime = 0;
            }
            
        }

        public void revive()
        {
            Visibility = Visibility.Visible;
        }

        private void cbUseRandom_Checked( object sender, RoutedEventArgs e )
        {
            config.UseRandomText = cbUseRandom.IsChecked == true;
        }

        private void cbUsePercentages_Checked( object sender, RoutedEventArgs e )
        {
            config.UsePercentages = cbUsePercentages.IsChecked == true;
        }

        private void cbStudyMode_Checked( object sender, RoutedEventArgs e )
        {
            config.StudyMode = cbStudyMode.IsChecked == true;
        }

        private void tbPopUpTime_LostFocus( object sender, EventArgs e )
        {
            ParsePopUpTime();
        }

        private void ParsePopUpTime()
        {
            var match = Regex.Match( tbPopUpTime.Text, "^\\d+$" );
            if ( match.Success )
            {
                PopUpTime = Int32.Parse( tbPopUpTime.Text );
                config.PopUpTime = tbPopUpTime.Text;
            }
            else
            {
                tbPopUpTime.Text = "0";
                PopUpTime = 0;
            }
            PopUpTime = Int32.Parse( tbPopUpTime.Text );
            config.PopUpTimeMins = PopUpTime;
        }
    }
}
