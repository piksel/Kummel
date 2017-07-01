using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibKummel
{
    public static class Dialogs
    {
        public static string ShowInputDialog(string text, int height, int width, string initial = "", bool password = false, string title = "")
        {
            var fib = new FormInputBox(text, height, width, initial, password);

            if (!string.IsNullOrEmpty(title))
                fib.Text = title;

            return (fib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ? fib.Result
                : "";
        }

        public static string ShowListDialog(string text, int height, int width, int listHeight, ListItem[] items, 
            bool singleClick = true, bool multiSelect = false, string title = "")
        {
            var flb = new FormListBox(text, height, width, listHeight, items, singleClick, multiSelect);

            if (!string.IsNullOrEmpty(title))
                flb.Text = title;

            return (flb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ? flb.Result
                : "";
        }
    }
}
