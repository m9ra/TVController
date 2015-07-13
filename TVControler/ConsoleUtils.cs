using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace TVControler
{

    class Wr
    {
        public readonly ConsoleColor Color;
        public readonly ConsoleColor BackgroundColor;
        public readonly String Message;


        public Wr(ConsoleColor consoleColor, string format,params object[] formatArgs)
        {
            this.Color = consoleColor;
            this.BackgroundColor = ConsoleColor.Black;            
            
            this.Message = string.Format(format, formatArgs);
        }

        public Wr(Exception ex)
        {
            this.Message = ex.ToString();
            this.Color = ConsoleColor.Red;
            this.BackgroundColor = ConsoleColor.DarkGray ;
        }
    }

    static class ConsoleUtils
    {
        private static object _LConsole = new object();
        private static StreamWriter _writer = new StreamWriter("console.log");

        public static void WriteLn(params Wr[] writeItems)
        {
            lock (_LConsole)
            {
                var oldC = Console.ForegroundColor;
                var oldBC = Console.BackgroundColor;
                foreach (var wr in writeItems)
                {
                    Console.BackgroundColor = wr.BackgroundColor;
                    Console.ForegroundColor = wr.Color;
                    Console.WriteLine(wr.Message);
                    _writer.WriteLine(wr.Message);
                }

                _writer.Flush();
                Console.ForegroundColor = oldC;
                Console.BackgroundColor = oldBC;
            }
        }

        internal static void WriteLn(Exception ex)
        {
            var wr = new Wr(ex);

            WriteLn(wr);
        }
    }
}
