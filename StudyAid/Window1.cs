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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace StudyAid
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private bool delayed = false;
        private int percentComplete = 0;
        private TextProvider textProvider;
        private MainWindow mainWindow;
        private DispatcherTimer randomTimer;
        private DispatcherTimer percentageTimer;
        private ConfigSettings _config;

        public Window1()
        {
            InitializeComponent();
        }


        public Window1( MainWindow mw, ConfigSettings config )
        {
            _config = config;
            mainWindow = mw;
            var source = mw.source;
            InitializeComponent();
            imgPicture.Source = source;

            innerGrid.Width = source.Width * .4;
            innerGrid.Height = source.Height * .4;

            vbText.Margin = new Thickness( source.Width * .04, source.Height * .4 * .11, source.Width * .04, source.Height * .4 * .3 );
            txCaption.MaxWidth = source.Width * .32;

            textProvider = new TextProvider(config);


            if ( config.StudyMode )
            {
                percentageTimer = new DispatcherTimer();
                percentageTimer.Tick += new EventHandler( updatePercentage );
                percentageTimer.Interval = GetTimeSpan(config.GoalTimeMins, config.UsePercentages ? 100 : 4);
                percentageTimer.Start();

                txCaption.Text = textProvider.GetNextText(0);
            }

            if ( !config.StudyMode || config.UseRandomText )
            {
                var intervalTimeSpan = new TimeSpan( 0, 0, config.PopUpTimeMins, 0, 25 );
                randomTimer = new DispatcherTimer();
                randomTimer.Tick += new EventHandler( updateText );
                randomTimer.Interval = intervalTimeSpan;
                randomTimer.Start();
                txCaption.Text = textProvider.GetNextText();
            }
        }

        private TimeSpan GetTimeSpan(int totalMinutes, int parts )
        {
            int minutes = totalMinutes / parts;
            int remainingSeconds = 60 * (totalMinutes % parts);
            int seconds = (int)((float)remainingSeconds / parts);
            int remainingMiliseconds = 1000 * (remainingSeconds % parts);
            int miliseconds = (int)((float)remainingMiliseconds / parts);
            return new TimeSpan( 0, 0, minutes, seconds, miliseconds );
            //return new TimeSpan( 0, 0, minutes );
        }

        private void updatePercentage( object sender, EventArgs e )
        {
            if ( _config.UsePercentages )
            {
                percentComplete++;
            }
            else
            {
                percentComplete += 25;
            }
            
            if (percentComplete >= 100 )
            {
                percentageTimer.Stop();
            }

            txCaption.Text = textProvider.GetNextText( percentComplete );
            delayed = true;
        }

        private void updateText( object sender, EventArgs e )
        {
            if ( delayed )
            {
                delayed = false;
                return;
            }
            txCaption.Text = textProvider.GetNextText();
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            mainWindow.revive();
            Close();
        }
    }
}
