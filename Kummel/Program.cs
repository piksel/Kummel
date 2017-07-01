using LibKummel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kummel
{
    partial class Program
    {
        static void Main(string[] args)
        {
            InitArgs();
            var options = _argParser.TryParseArgs(args);

            if(!options.Valid)
            {
                Console.WriteLine(string.Join(" ", args));
                Console.WriteLine(options.ParseError);
            }

            if (options.IsEmpty || options.Help)
                _argParser.PrintUsage();
            else if(options.Version)
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                var lv = typeof(LibKummel.Dialogs).Assembly.GetName().Version;

                Console.WriteLine($"Kummel v{v.Major}.{v.Minor}.{v.Revision}, build {v.Build}");
                Console.WriteLine($"LibKummel v{lv.Major}.{lv.Minor}.{lv.Revision}, build {lv.Build}");
            }
            else
            {
                Console.WriteLine(options.Type.ToString());

                if (options.Type.HasFlag(BoxType.Input))
                {
                    var result = Dialogs.ShowInputDialog(options.Text, options.Width, options.Height, options.Initial,
                        options.Type.HasFlag(BoxType.Password), options.Title);
                    Console.Write(result);
                }
                else if (options.Type.HasFlag(BoxType.List))
                {
                    Console.WriteLine(string.Join("\n", options.Items.Select(li => li.ToString())));
                    var result = Dialogs.ShowListDialog(options.Text, options.Width, options.Height, 
                        options.ListHeight, options.Items, options.Type.HasFlag(BoxType.InstantSelect), 
                        options.Type.HasFlag(BoxType.RadioBoxes), options.Title);
                    Console.Write(result);

                }

            }

            Console.ReadKey();
        }


    }
}
