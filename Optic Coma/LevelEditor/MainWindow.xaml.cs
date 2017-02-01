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

namespace LevelEditor
{
    enum UserWindowState
    {
        JustStarted,
        Idle,
        InFileMenu
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserWindowState State = UserWindowState.JustStarted;
        public MainWindow()
        {
            InitializeComponent();
        }

        #region File, Edit, View, etc. Toolbar

        //File Button Mouseover Start
        #region File
        private void btnFile_MouseEnter(object sender, MouseEventArgs e)
        {
            //btnFile.Background = ToolBarStyleColors.ButtonHover;
        }
        //File Button Mouseover End
        private void btnFile_MouseLeave(object sender, MouseEventArgs e)
        {
            //btnFile.Background = ToolBarStyleColors.ButtonIdle;
        }
        //File Button Hover while clicking
        private void btnFile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //btnFile.Background = ToolBarStyleColors.ButtonClickHold;
        }
        //File Button Click
        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            if ((State == UserWindowState.JustStarted)||(State == UserWindowState.InFileMenu))
            {
                CollapseFileMenu();
            }
            else if (State == UserWindowState.Idle)
            {
                ExpandFileMenu();
            }
        }
        private void CollapseFileMenu()
        {
            rectFileMenuBox.IsEnabled = false;
            rectFileMenuBox.Visibility = Visibility.Collapsed;
            State = UserWindowState.Idle;
        }
        private void ExpandFileMenu()
        {
            rectFileMenuBox.IsEnabled = true;
            rectFileMenuBox.Visibility = Visibility.Visible;
            State = UserWindowState.InFileMenu;
        }

        #endregion

        #endregion
    }
    struct ToolBarStyleColors
    {
        public static SolidColorBrush ButtonIdle = new SolidColorBrush(new Color());
        public static SolidColorBrush ButtonHover = new SolidColorBrush(new Color());
        public static SolidColorBrush ButtonClickHold = new SolidColorBrush(new Color());
    }
}
