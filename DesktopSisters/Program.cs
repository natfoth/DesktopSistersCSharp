using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters;

namespace DesktopSistersCSharpForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var app = new Sisters(TryLoadConfig());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            
        }

        private static Configuration TryLoadConfig()
        {
            try
            {
                return Configuration.LoadConfig("config.ini");
            }
            catch (Exception ex)
            {
                throw new HandledFatalException(ex, "There was a Fatal Error Reading your Configuration File!");
            }
        }

        public static void WriteExceptionFailureMessage(Exception ex, params string[] messages)
        {
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
            Console.WriteLine("Please read the error below,");
            Console.WriteLine("If you feel it is a bug please report to:");
            Console.WriteLine("http://www.github.com/natfoth/DesktopSistersCSharp/issues");
            Console.WriteLine("=========================================================");
            Console.WriteLine("");
            Console.WriteLine("Excecption details:");
            Console.WriteLine("");
            Console.WriteLine(ex.ToString());
        }
    }

    public class HandledFatalException : Exception
    {
        public HandledFatalException(Exception ex, params string[] messages)
        {
            Exception = ex;
            Messages = messages;
        }

        public Exception Exception { get; set; }

        public string[] Messages { get; set; }
    }
}
