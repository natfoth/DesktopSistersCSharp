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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            

            
            Application.Run(new Form1());

            
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
