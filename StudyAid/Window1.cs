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

namespace StudyAid
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private bool _complete;

        public Window1()
        {
            InitializeComponent();
        }


        public Window1( ImageSource source, string captionText, bool complete )
        {
            _complete = complete;
            InitializeComponent();
            imgPicture.Source = source;
            txCaption.Text = captionText;
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            if (_complete )
            {
                Application.Current.Shutdown();
            }
            else
            {
                Close();
            }
        }
    }
}
