using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Level_Editor
{
    public partial class LevelNameInputDialog : Form
    {
        public LevelNameInputDialog()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }
        public string GetLevelName()
        {
            if (txtInput.Text != null)
                return txtInput.Text;
            else return "newLevel";
        }
    }
}
