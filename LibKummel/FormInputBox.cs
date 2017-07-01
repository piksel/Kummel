using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibKummel
{
    public partial class FormInputBox : Form
    {
        public string Result { get; private set; }

        public FormInputBox(string text, int height, int width, string initial = "", bool password = false)
        {
            InitializeComponent();

            lText.Text = text;
            Width = width * 7;
            Height = height * 14;
            tbInput.Text = initial;
            tbInput.UseSystemPasswordChar = password;
        }

        private void FormInputBox_Load(object sender, EventArgs e)
        {

        }

        private void FormInputBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            Result = tbInput.Text;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                DialogResult = e.KeyCode == Keys.Escape ? DialogResult.Cancel : DialogResult.OK;
                Close();
            }
        }

        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
