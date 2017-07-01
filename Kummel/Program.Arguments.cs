using LibKummel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kummel
{
    partial class Program
    {

        static ArgParser<Options> _argParser;

        /*
         
                "\t--msgbox <text> <height> <width>\n" +
                "\t--yesno  <text> <height> <width>\n" +
                "\t--infobox <text> <height> <width>\n" +
                "\t--inputbox <text> <height> <width> [init] \n" +
                "\t--passwordbox <text> <height> <width> [init] \n" +
                "\t--textbox <file> <height> <width>\n" +
                "\t--menu <text> <height> <width> <listheight> [tag item] ...\n" +
                "\t--checklist <text> <height> <width> <listheight> [tag item status]...\n" +
                "\t--radiolist <text> <height> <width> <listheight> [tag item status]...\n" +
                "\t--gauge <text> <height> <width> <percent>\n" +


             */

        private static void InitArgs()
        {
            _argParser = new ArgParser<Options>("kummel");

            _argParser.AddArgument("MsgBox", "Message box")
                .WithLongKeys("msgbox")
                .WithParameters("text", "height", "width")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.Text = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.Type = BoxType.MsgBox;
                });

            _argParser.AddArgument("YesNo", "Yes/no box")
                .WithLongKeys("yesno")
                .WithParameters("text", "height", "width")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.Text = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.Type = BoxType.YesNoBox;
                });

            _argParser.AddArgument("InfoBox", "Message box")
                .WithLongKeys("infobox")
                .WithParameters("text", "height", "width")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.Text = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.Type = BoxType.InfoBox;
                });

            _argParser.AddArgument("TextBox", "Text box")
                .WithLongKeys("textbox")
                .WithParameters("file", "height", "width")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.TextFile = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.Type = BoxType.TextBox;
                });

            _argParser.AddArgument("InputBox", "Input box")
                .WithLongKeys("inputbox")
                .WithParameters("text", "height", "width")
                .WithOptional("initial")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.Text = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.Type = BoxType.InputBox;
                    if (p.Length >= 4)
                        o.Initial = p[3];
                });

            _argParser.AddArgument("PasswordBox", "Password box")
                .WithLongKeys("passwordbox")
                .WithParameters("text", "height", "width")
                .WithOptional("initial")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.Text = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.Type = BoxType.PasswordBox;
                    if (p.Length >= 4)
                        o.Initial = p[3];
                });


            _argParser.AddArgument("Gauge", "Percentage gauge")
                .WithLongKeys("gauge")
                .WithParameters("text", "height", "width", "percent")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.Text = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.Percent = int.Parse(p[3]);
                    o.Type = BoxType.Gauge;

                });

            _argParser.AddArgument("Menu", "Menu")
                .WithLongKeys("menu")
                .WithParameters("text", "height", "width", "listheight")
                .WithOptional(true, "tag", "item")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.Text = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.ListHeight = int.Parse(p[3]);
                    o.Items = GetOptGroups(p, 4, false);
                    o.Type = BoxType.MenuBox;
                });

            _argParser.AddArgument("CheckList", "CheckList")
                .WithLongKeys("checklist")
                .WithParameters("text", "height", "width", "listheight")
                .WithOptional(true, "tag", "item", "status")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.Text = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.ListHeight = int.Parse(p[3]);
                    o.Items = GetOptGroups(p, 4, true);
                    o.Type = BoxType.CheckListBox;
                });

            _argParser.AddArgument("RadioList", "RadioList")
                .WithLongKeys("radiolist")
                .WithParameters("text", "height", "width", "listheight")
                .WithOptional(true, "tag", "item", "status")
                .WithMode(true)
                .WithParser((o, p) =>
                {
                    o.Text = p[0];
                    o.Height = int.Parse(p[1]);
                    o.Width = int.Parse(p[2]);
                    o.ListHeight = int.Parse(p[3]);
                    o.Items = GetOptGroups(p, 4, true);
                    o.Type = BoxType.RadioListBox;
                });

        }

        public static readonly string[] Truthy = new[] { "1", "yes", "on" };

        private static ListItem[] GetOptGroups(string[] p, int start, bool status)
        {
            var items = new List<ListItem>();
            for(int i = start; i<p.Length; i += status ? 3 : 2)
            {
                items.Add(new ListItem()
                {
                    Tag = p[i],
                    Text = p[i + 1],
                    Checked = status && Truthy.Contains(p[i + 2])
                });
            }
            return items.ToArray();
        }
    }

    public class Options : ArgOptionsBase
    {
        [Argument("print this message", shortKey: 'h', longKey: "help")]
        public bool Help { get; set; }

        [Argument("print version", shortKey: 'v', longKey: "version")]
        public bool Version { get; set; }

        [Argument("disable cancel button", longKey: "nocancel")]
        public bool NoCancel { get; set; }

        [Argument("disable cancel button", LongKeys = new [] { "fullbuttons", "fb"})]
        public bool FullButtons { get; set; }

        [Argument("not used", longKey: "clear", Warning = "Clear is only provided for compability reasons")]
        public bool Clear { get; set; }

        [Argument("set default item", longKey: "default-item", ParameterName = "string")]
        public string DefaultItem { get; set; }

        [Argument("set yes button text", longKey: "yes-button", ParameterName = "text")]
        public string YesButton { get; set; }

        [Argument("set no button text", longKey: "no-button", ParameterName = "text")]
        public string NoButton { get; set; }

        [Argument("set OK button text", longKey: "ok-button", ParameterName = "text")]
        public string OkButton { get; set; }

        [Argument("set cancel button text", longKey: "cancel-button", ParameterName = "text")]
        public string CancelButton { get; set; }

        [Argument("don't display items", longKey: "noitems")]
        public bool NoItems { get; set; }

        [Argument("don't display tags", longKey: "notags")]
        public bool NoTags { get; set; }

        [Argument("output one line at a time", longKey: "separate-output")]
        public bool SeparateOutput { get; set; }

        [Argument("not used", longKey: "output-fd", Warning = "File Descriptor output is not supported!", ParameterName = "fd")]
        public int OutputFd { get; set; }

        [Argument("set dialog title text", longKey: "title", ParameterName = "text")]
        public string Title { get; set; }

        [Argument("set dialog back title text", longKey: "backtitle", ParameterName = "text")]
        public string BackTitle { get; set; }

        [Argument("not used", longKey: "scrolltext", Warning = "Scroll Text is only provided for compability reasons")]
        public bool ScrollText { get; set; }

        [Argument("not used", longKey: "topleft", Warning = "Top Left is only provided for compability reasons")]
        public bool TopLeft { get; set; }

        public BoxType Type { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int ListHeight { get; set; }
        
        public ListItem[] Items { get; set; }

        public int Percent { get; set; }
        public string Text { get; set; }
        public string Initial { get; set; }
        public string TextFile { get; set; }
    }


    [Flags]
    public enum BoxType
    {
        None = 0,
        Input = 1,
        Message = 2,
        Password = 4,
        NonBlocking = 8,
        YesNo = 16,
        TextFromFile = 32,
        Gauge = 64,
        List = 128,
        CheckBoxes = 256,
        RadioBoxes = 512,
        InstantSelect = 1024,


        MsgBox = Message,
        TextBox = Message | TextFromFile,
        InfoBox = Message | NonBlocking,
        YesNoBox = Message | YesNo,
        InputBox = Message | Input,
        PasswordBox = InputBox | Password,
        MenuBox = Message | List | InstantSelect,
        CheckListBox = Message | List | CheckBoxes,
        RadioListBox = Message | List | RadioBoxes,
        GaugeBox = Message | Gauge
    }
}
