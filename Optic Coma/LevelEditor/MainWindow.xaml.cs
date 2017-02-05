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
using System.IO;
using Microsoft.Win32;

namespace LevelEditor
{
    public enum UserWindowState
    {
        JustStarted,
        Idle,
        InFileMenu,
        InFileCreateDialog,
        InOpenDialog,
        InEditMenu,
        InViewMenu
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public UserWindowState State = UserWindowState.JustStarted;
        static MainWindow()
        {
            Instance = new MainWindow();
        }

        private MainWindow()
        {
            InitializeComponent();
        }

        private void frmMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            rectTopBarDeco.Width = frmMain.Width;
            rectToolBarBackgroundDeco.Width = frmMain.Width; 
        }

        #region File, Edit, View, etc. Toolbar

        #region File

        #region File Itself
        //File Button Mouseover Start
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
            switch (State)
            {
                case UserWindowState.Idle:
                    ExpandFileMenu();
                    break;
                case UserWindowState.InViewMenu:
                    CollapseViewMenu();
                    ExpandFileMenu();
                    break;
                case UserWindowState.InFileMenu:
                    CollapseFileMenu();
                    break;
                case UserWindowState.InEditMenu:
                    CollapseEditMenu();
                    ExpandFileMenu();
                    break;
                case UserWindowState.JustStarted:
                    goto case UserWindowState.Idle;
            }
        }
        #endregion
        #region New
        private void btnFile_NewLevel_Click(object sender, RoutedEventArgs e)
        { 
            CollapseFileMenu();
            State = UserWindowState.InFileCreateDialog;
            Window newWindow = new FileCreateWindow();
            newWindow.Show();
            frmMain.Focusable = false;
        }

        private void btnFile_NewLevel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnFile_NewLevel_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnFile_NewLevel_MouseLeave(object sender, MouseEventArgs e)
        {

        }
        #endregion
        #region exit
        private void btnFile_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnFile_Exit_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnFile_Exit_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnFile_Exit_MouseLeave(object sender, MouseEventArgs e)
        {

        }
        #endregion
        #region open
        private void btnFile_Open_Click(object sender, RoutedEventArgs e)
        {
            CollapseFileMenu();
            State = UserWindowState.InOpenDialog;
            LevelFileReader reader = new LevelFileReader();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if ((bool)openFileDialog.ShowDialog())
            {
                reader.OpenFile(openFileDialog.FileName);
            }
            State = UserWindowState.Idle;
        }


        private void btnFile_Open_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnFile_Open_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnFile_Open_MouseLeave(object sender, MouseEventArgs e)
        {

        }
        #endregion
        private void CollapseFileMenu()
        {
            rectFileMenuBox.IsEnabled = false;
            rectFileMenuBox.Visibility = Visibility.Collapsed;
            rectFileMenuLeft.IsEnabled = false;
            rectFileMenuLeft.Visibility = Visibility.Collapsed;
            btnFile_NewLevel.IsEnabled = false;
            btnFile_NewLevel.Visibility = Visibility.Collapsed;
            btnFile_Exit.IsEnabled = false;
            btnFile_Exit.Visibility = Visibility.Collapsed;
            btnFile_Open.IsEnabled = false;
            btnFile_Open.Visibility = Visibility.Collapsed;
            State = UserWindowState.Idle;
        }
        private void ExpandFileMenu()
        {
            rectFileMenuBox.IsEnabled = true;
            rectFileMenuBox.Visibility = Visibility.Visible;
            rectFileMenuLeft.IsEnabled = true;
            rectFileMenuLeft.Visibility = Visibility.Visible;
            btnFile_NewLevel.IsEnabled = true;
            btnFile_NewLevel.Visibility = Visibility.Visible;
            btnFile_Exit.IsEnabled = true;
            btnFile_Exit.Visibility = Visibility.Visible;
            btnFile_Open.IsEnabled = true;
            btnFile_Open.Visibility = Visibility.Visible;
            State = UserWindowState.InFileMenu;
        }

        #endregion
        #region Edit menu (undo, redo, etc)
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            switch (State)
            {
                case UserWindowState.Idle:
                    ExpandEditMenu();
                    break;
                case UserWindowState.InViewMenu:
                    CollapseViewMenu();
                    ExpandEditMenu();
                    break;
                case UserWindowState.InFileMenu:
                    CollapseFileMenu();
                    ExpandEditMenu();
                    break;
                case UserWindowState.InEditMenu:
                    CollapseEditMenu();
                    break;
                case UserWindowState.JustStarted:
                    goto case UserWindowState.Idle;
            }
        }

        private void btnEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnEdit_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnEdit_MouseLeave(object sender, MouseEventArgs e)
        {

        }
        private void btnEdit_Undo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Redo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Cut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Copy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Paste_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CollapseEditMenu()
        {
            rectEditMenuBox.IsEnabled = false;
            rectEditMenuBox.Visibility = Visibility.Collapsed;
            rectEditMenuLeft.IsEnabled = false;
            rectEditMenuLeft.Visibility = Visibility.Collapsed;
            btnEdit_Undo.IsEnabled = false;
            btnEdit_Undo.Visibility = Visibility.Collapsed;
            btnEdit_Redo.IsEnabled = false;
            btnEdit_Redo.Visibility = Visibility.Collapsed;
            btnEdit_Cut.IsEnabled = false;
            btnEdit_Cut.Visibility = Visibility.Collapsed;
            btnEdit_Copy.IsEnabled = false;
            btnEdit_Copy.Visibility = Visibility.Collapsed;
            btnEdit_Paste.IsEnabled = false;
            btnEdit_Paste.Visibility = Visibility.Collapsed;
            State = UserWindowState.Idle;
        }
        private void ExpandEditMenu()
        {
            rectEditMenuBox.IsEnabled = true;
            rectEditMenuBox.Visibility = Visibility.Visible;
            rectEditMenuLeft.IsEnabled = true;
            rectEditMenuLeft.Visibility = Visibility.Visible;
            btnEdit_Undo.IsEnabled = true;
            btnEdit_Undo.Visibility = Visibility.Visible;
            btnEdit_Redo.IsEnabled = true;
            btnEdit_Redo.Visibility = Visibility.Visible;
            btnEdit_Cut.IsEnabled = true;
            btnEdit_Cut.Visibility = Visibility.Visible;
            btnEdit_Copy.IsEnabled = true;
            btnEdit_Copy.Visibility = Visibility.Visible;
            btnEdit_Paste.IsEnabled = true;
            btnEdit_Paste.Visibility = Visibility.Visible;
            State = UserWindowState.InEditMenu;
        }
        #endregion
        #region View menu (show gridlines, etc)
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            switch (State)
            {
                case UserWindowState.Idle:
                    ExpandViewMenu();
                    break;
                case UserWindowState.InEditMenu:
                    CollapseEditMenu();
                    ExpandViewMenu();
                    break;
                case UserWindowState.InFileMenu:
                    CollapseFileMenu();
                    ExpandViewMenu();
                    break;
                case UserWindowState.InViewMenu:
                    CollapseViewMenu();
                    break;
                case UserWindowState.JustStarted:
                    goto case UserWindowState.Idle;
            }
        }
        private void btnView_ShowGridlines_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CollapseViewMenu()
        {
            rectViewMenuBox.IsEnabled = false;
            rectViewMenuBox.Visibility = Visibility.Collapsed;
            rectViewMenuLeft.IsEnabled = false;
            rectViewMenuLeft.Visibility = Visibility.Collapsed;
            btnView_ShowGridlines.IsEnabled = false;
            btnView_ShowGridlines.Visibility = Visibility.Collapsed;

            State = UserWindowState.Idle;
        }
        private void ExpandViewMenu()
        {
            rectViewMenuBox.IsEnabled = true;
            rectViewMenuBox.Visibility = Visibility.Visible;
            rectViewMenuLeft.IsEnabled = true;
            rectViewMenuLeft.Visibility = Visibility.Visible;
            btnView_ShowGridlines.IsEnabled = true;
            btnView_ShowGridlines.Visibility = Visibility.Visible;

            State = UserWindowState.InViewMenu;
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
    public class LevelFileReader
    {
        public LevelFileReader(){ }
        
        public void OpenFile(string path)
        {
            ParseFile(/*params go here*/);
        }
        public void ParseFile()
        {

        }
    }
}
