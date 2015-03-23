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
        private readonly Configuration _config;
        public Form1()
        {
            InitializeComponent();

            _config = TryLoadConfig();
            _sistersApp = new Sisters(_config, this);

            useNewArtCheckbox.Checked = _config.UseNewArtStyle;
        }

        private void useNewArtCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            _config.UseNewArtStyle = useNewArtCheckbox.Checked;
            _config.Save();

            _sistersApp.UpdateConfig(_config);
            _sistersApp.Update();
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("https://www.google.de/#q=New%20York%20City%20Latitude");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _config.Coordinates = textBox1.Text;
            _config.Save();

            _sistersApp.UpdateConfig(_config);
            _sistersApp.Update();
        }
    }
}
