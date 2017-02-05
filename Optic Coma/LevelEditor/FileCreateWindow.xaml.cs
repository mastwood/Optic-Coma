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

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for FileCreateWindow.xaml
    /// </summary>
    public partial class FileCreateWindow : Window
    {
        public FileCreateWindow()
        {
            InitializeComponent();
        }

        private void frmFileCreate_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.Instance.Focusable = true;
            MainWindow.Instance.Focus();
            MainWindow.Instance.State = UserWindowState.Idle;
        }

        private void txtSizeX_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSizeX.Text == "X")
                txtSizeX.Text = "";
        }
        private void txtSizeY_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSizeY.Text == "Y")
                txtSizeY.Text = "";
        }
        private void txtSizeX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSizeX.Text == "")
                txtSizeX.Text = "X";
        }
        private void txtSizeY_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSizeY.Text == "")
                txtSizeY.Text = "Y";
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            uint x = 0;
            uint.TryParse(txtSizeX.Text, out x);
            uint y = 0;
            uint.TryParse(txtSizeY.Text, out y);
            if (x > 0 && y > 0 && IsAcceptableFileName(txtNameInput.Text) == 1)
            {
                GenerateLevel(new Vector(x, y), txtNameInput.Text);
                MainWindow.Instance.Focusable = true;
                MainWindow.Instance.Focus();
                MainWindow.Instance.State = UserWindowState.Idle;
            }
            else
            {
                MessageBox.Show("Error: Invalid name or value. Name must consist of only numbers, letters, \nor \"-\" and \"_\". Values must consist of positive integers");
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            frmFileCreate.Close();
            MainWindow.Instance.Focusable = true;
            MainWindow.Instance.Focus();
            MainWindow.Instance.State = UserWindowState.Idle;
        }
        private void GenerateLevel(Vector size, string name)
        {
            //TODO
        }
        public static short IsAcceptableFileName(string s)
        {
            char[] c = s.ToCharArray();
            foreach (char x in c)
            {
                if (!"1234567890-_qwertyuiopasdfghjklzxcvbnm".Contains(x))
                {
                    return 1;
                }
                else return 0;
            }
            return -1;
        }
    }
}
