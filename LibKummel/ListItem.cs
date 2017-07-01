using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibKummel
{
    [Serializable]
    public class ListItem
    {
        public string Tag { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public override string ToString() => $"[{(Checked ? 'X' : ' ')}] {Tag}, {Text}";
    }
}
