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
    public partial class FormListBox : Form
    {
        public string Result { get; set; }

        public FormListBox(string text, int width, int height, int listHeight, ListItem[] items, bool singleClick = true, bool multiSelect = false)
        {

            InitializeComponent();

            lText.Text = text;



            menuButtonList2.Items = items;//.Select(li => li.Text).ToArray();
                
            Width = width * 7;
            Height = height * 14;

            menuButtonList2.Height = listHeight * 52;
            menuButtonList2.Top = Height - menuButtonList2.Height - 100;

        }

        private void FormListBox_Load(object sender, EventArgs e)
        {

        }

    }
}
