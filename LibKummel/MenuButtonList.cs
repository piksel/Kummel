using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibKummel
{
    public partial class MenuButtonList : UserControl
    {

        [Browsable(true), DefaultValue(true), Category("Menu")]
        public bool MultiSelect { get; set; } = true;

        [Browsable(true), DefaultValue(false), Category("Menu")]
        public bool Checkable { get; set; } = false;


        ListItem[] _items = new ListItem[0];

        [Browsable(true), Category("Menu")]
        public ListItem[] Items {
            get => _items;
            set
            {
                _items = value;
                UpdateButtons();
            }
        }

        List<Button> buttons = new List<Button>();

        public MenuButtonList()
        {
            InitializeComponent();

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Controls.Remove(buttons[i]);
            }
            buttons.Clear();

            for (int i = 0; i < Items.Length; i++)
            {
                var b = new Button();
                //var b = new CheckBox();
                
                b.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                b.BackColor = SystemColors.Window;
                b.FlatAppearance.BorderColor = SystemColors.Window;
                b.FlatAppearance.MouseOverBackColor = SystemColors.Window;
                b.FlatStyle = FlatStyle.Flat;
                b.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                b.ForeColor = SystemColors.HotTrack;
                b.ImageAlign = ContentAlignment.MiddleLeft;
                b.ImageKey = Checkable ? (MultiSelect ? "checkitem_off": "radioitem_off") : "menuitem";
                b.ImageList = imageList1;
                b.Location = new Point(0, (52 * i));
                b.Size = new Size(Width, 48);
                b.Text = Items[i].Text;
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.TextImageRelation = TextImageRelation.ImageBeforeText;
                b.UseVisualStyleBackColor = false;
                b.MouseEnter += button1_MouseEnter;
                b.MouseLeave += button1_MouseLeave;
                b.Click += B_Click;
                

                Controls.Add(b);
                buttons.Add(b);
            }
        }

        private void B_Click(object sender, EventArgs e)
        {
            var b = (sender as ButtonBase);

            for (int i = 0; i < Items.Length; i++)
            {
                var clicked = buttons[i] == b;
                if (clicked || !MultiSelect)
                {
                    _items[i].Checked = clicked;
                    if (Checkable)
                    {
                        buttons[i].ImageKey = $"{(MultiSelect ? "check" : "radio")}item_{(clicked ? "on" : "off")}";
                    }
                    else
                    {
                        // Return Tag
                    }
                }
            }
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            (sender as ButtonBase).FlatAppearance.BorderColor = SystemColors.HotTrack;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            (sender as ButtonBase).FlatAppearance.BorderColor = SystemColors.Window;
        }
    

    private void MenuButtonList_Load(object sender, EventArgs e)
        {

        }
    }
}
