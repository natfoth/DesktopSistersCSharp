using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopSisters
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var app = new Sisters(TryLoadConfig());
                app.Start();
                Console.WriteLine();
                Console.WriteLine("The Program has finished");
                Console.WriteLine("Press enter to close....");
                Console.ReadLine();
            }
            catch (HandledFatalException ex)
            {
                WriteExceptionFailureMessage(ex.Exception, ex.Messages);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                WriteExceptionFailureMessage(ex, "There was an Unhandled Fatal Error!");
            }

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
