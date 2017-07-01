using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibKummel
{
    public class ArgParser<T> where T : IArgOptions, new()
    {
        public List<Argument<T>> Arguments { get; internal set; }
            = new List<Argument<T>>();

        string _execName;

        public ArgParser(string execName)
        {
            _execName = execName;

            foreach(var prop in typeof(T).GetProperties())
            {
                foreach(ArgumentAttribute attrib in prop.GetCustomAttributes(typeof(ArgumentAttribute), false))
                {
                    var arg = new Argument<T>()
                    {
                        Name = prop.Name,
                        Long = attrib.LongKeys,
                        Short = attrib.ShortKeys,
                        HelpText = attrib.HelpText,
                    };
                    
                    
                    if(prop.PropertyType == typeof(bool))
                    {
                        arg.Parse = (o, p) => prop.SetValue(o, p[0] != "no");
                        arg.Flag = true;
                    }
                    else if(prop.PropertyType == typeof(string))
                    {
                        arg.Parameters = new[] { attrib.ParameterName };
                        arg.Parse = (o, p) => prop.SetValue(o, p[0]);
                    }

                    if (!string.IsNullOrEmpty(attrib.Warning))
                    {
                        var realParse = arg.Parse;
                        arg.Parse = (o, p) =>
                        {
                            Console.WriteLine("Warning: " + attrib.Warning);
                            realParse(o, p);
                        };
                    }

                    Arguments.Add(arg);
                    
                }
            }
        }

        public T TryParseArgs(string[] args)
        {
            try
            {
                return ParseArgs(args);
            }
            catch (Exception x)
            {
                return new T()
                {
                    ParseError = $"{x.GetType().Name}: {x.Message}",
                    Valid = false
                };
            }
        }

        public T ParseArgs(string[] args) 
        {

            if (args.Length < 1)
                return new T()
                {
                    Valid = true,
                    IsEmpty = true
                };

            var options = new T()
            {
                Valid = true
            };

            int argPos = 0;
            Argument<T> actionArg = null;

            while (argPos < args.Length)
            {
                var arg = GetArg(args[argPos], out bool inverted);

                if (arg.Mode && actionArg != null)
                    throw new ArgumentException($"Cannot combine multiple action arguments \"{actionArg.Name}\" and \"{arg.Name}\".");

                var opts = 0;
                for (int i = 0; i < args.Length; i++)
                {
                    var offset = argPos + 1 + arg.Parameters.Length + i;
                    if (offset >= args.Length || args[offset][0] == '-' || (!arg.OptionalGrouping && i >= arg.Optional.Length))
                        break;

                    opts++;
                }

                if (arg.OptionalGrouping && (opts % arg.Optional.Length != 0))
                    throw new ArgumentException($"Invalid optional argument count for \"{arg.Name}\"");

                var pars = arg.Flag 
                    ? new[] { inverted ? "no" : " " } 
                    : args.Skip(argPos + 1).Take(arg.Parameters.Length + opts).ToArray();
                arg.Parse(options, pars);
                argPos += arg.Parameters.Length + opts + 1;
            }


            return options;
        }

        private Argument<T> GetArg(string input, out bool inverted)
        {
            if (input[0] != '-')
                throw new ArgumentException($"Invalid argument syntax \"{input}\"");

            char inputchar = '\0';
            string inputstring = "";

            if (input[1] != '-')
                inputchar = input[(inverted = input[1] == '!') ? 2 : 1];
            else
                inputstring = input.Substring((inverted = input.StartsWith("--no-")) ? 5 : 2);
            

            foreach (var arg in Arguments)
            {
                if ((inputchar != '\0' && arg.Short.Contains(inputchar))
                || (!string.IsNullOrEmpty(inputstring) && arg.Long.Contains(inputstring)))
                {
                    return arg;
                }

            }

            throw new ArgumentException($"Invalid argument \"{input}\"");
        }


        public void PrintUsage()
        {
            Console.WriteLine($"Usage: {_execName} <options> <action>\n\nActions:");
            var leftWidth = Console.BufferWidth / 2;

            foreach (var arg in Arguments.Where(a => a.Mode).OrderBy(a => a.Name))
                Console.WriteLine(arg.ToString(leftWidth));

            Console.WriteLine("\nOptions:");

            foreach (var arg in Arguments.Where(a => !a.Mode).OrderBy(a => a.Name))
                Console.WriteLine(arg.ToString(leftWidth));
        }

        public ArgumentBuilder AddArgument(string name, string helpText = "")
        {
            var ab = new ArgumentBuilder(name, helpText);
            Arguments.Add(ab.Argument);
            return ab;
        }

        public class ArgumentBuilder
        {
            public Argument<T> Argument { get; internal set; }

            public ArgumentBuilder(string name, string helpText = "")
            {
                Argument = new Argument<T>()
                {
                    Name = name,
                    HelpText = helpText
                };
            }

            public ArgumentBuilder WithShortKeys(params char[] shortkeys)
            {
                Argument.Short = shortkeys;
                return this;
            }

            public ArgumentBuilder WithLongKeys(params string[] longkeys)
            {
                Argument.Long = longkeys;
                return this;
            }

            public ArgumentBuilder WithParameters(params string[] parameters)
            {
                Argument.Parameters = parameters;
                return this;
            }

            public ArgumentBuilder WithOptional(bool grouped, params string[] optional)
            {
                Argument.OptionalGrouping = grouped;
                Argument.Optional = optional;
                return this;
            }

            public ArgumentBuilder WithOptional(params string[] optional)
                => WithOptional(false, optional);

            public ArgumentBuilder WithParser(Action<T, string[]> parserAction)
            {
                Argument.Parse = parserAction;
                return this;
            }

            public ArgumentBuilder WithFlag(bool flag)
            {
                Argument.Flag = flag;
                return this;
            }

            public ArgumentBuilder WithMode(bool mode)
            {
                Argument.Mode = mode;
                return this;
            }
        }
    }

    public class ArgOptionsBase: IArgOptions
    {
        public bool IsEmpty { get; set; }

        public string ParseError { get; set; }
        public bool Valid { get; set; }
    }

    public interface IArgOptions
    {
        bool IsEmpty { get; set; }

        string ParseError { get; set; }
        bool Valid { get; set; }
    }

    public class Argument<T>
    {
        /// <summary>
        /// Short name for error messages
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Long argument names, following --
        /// </summary>
        public string[] Long { get; set; } = new string[0];

        /// <summary>
        /// Short argument names, single character following -
        /// </summary>
        public char[] Short { get; set; } = new char[0];

        /// <summary>
        /// Required parameters
        /// </summary>
        public string[] Parameters { get; set; } = new string[0];

        /// <summary>
        /// Optional parameters
        /// </summary>
        public string[] Optional { get; set; } = new string[0];

        /// <summary>
        /// Group optional arguments
        /// </summary>
        public bool OptionalGrouping { get; set; } = false;

        /// <summary>
        /// Help text
        /// </summary>
        public string HelpText { get; set; } = "";

        /// <summary>
        /// Whether the argument sets the running action
        /// </summary>
        public bool Mode { get; set; } = false;

        /// <summary>
        /// Whether the argument is a toggleable flag 
        /// </summary>
        public bool Flag { get; set; } = false;

        /// <summary>
        /// Function for setting the options from passed arguments
        /// </summary>
        public Action<T, string[]> Parse { get; set; }

        public override string ToString() => ToString(40);

        public string ToString(int argWidth)
        {
            var keys = string.Join(", ", Long.Select(l => "--" + l).Concat(Short.Select(s => "-" + s))) + " ";

            var parms = string.Join(" ", Parameters.Select(p => "<" + p + ">"));
            if (!string.IsNullOrEmpty(parms)) parms += " ";

            var opts = OptionalGrouping
                ? "[" + (string.Join(" ", Optional)) + "] ..."
                : string.Join(" ", Optional.Select(o => "[" + o + "]"));

            return ("  " + keys + parms + opts).PadRight(argWidth) + HelpText;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ArgumentAttribute: Attribute
    {
        public char[] ShortKeys { get; set; } = new char[0];
        public string[] LongKeys { get; set; } = new string[0];
        public string HelpText { get; set; } = "";
        public string ParameterName { get; set; } = "value";

        public string Warning { get; set; }

        public ArgumentAttribute(string helpText, char shortKey) : this(helpText)
            => ShortKeys = new[] { shortKey };

        public ArgumentAttribute(string helpText, string longKey) : this(helpText)
            => LongKeys = new[] { longKey };

        public ArgumentAttribute(string helpText, char shortKey, string longKey) : this(helpText, shortKey)
            => LongKeys = new[] { longKey };

        public ArgumentAttribute(string helpText)
            => HelpText = helpText;
    }
}
