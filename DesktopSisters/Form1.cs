using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters;

namespace DesktopSistersCSharpForm
{
    public partial class Form1 : Form
    {
        private Sisters _sistersApp;
        public Form1()
        {
            InitializeComponent();

            _sistersApp = new Sisters(TryLoadConfig(), this);

        }

        private void useNewArtCheckbox_CheckedChanged(object sender, EventArgs e)
        {

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
}
