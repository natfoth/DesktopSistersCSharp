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

            Configuration.LoadConfig("config.ini");

            _sistersApp = new Sisters(this);

            useNewArtCheckbox.Checked = Configuration.Instance.UseNewArtStyle;
            textBox1.Text = Configuration.Instance.Coordinates;
            toolStripMenuItem1.Click += ToolStripMenuItem1OnClick;
            toolStripMenuItem2.Click += ToolStripMenuItem2OnClick;
            intervalBox.Value = Configuration.Instance.UpdateInterval;
        }

        private void ToolStripMenuItem2OnClick(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private void useNewArtCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Instance.UseNewArtStyle = useNewArtCheckbox.Checked;
            Configuration.Instance.Save();

            _sistersApp.UpdateConfig();
            _sistersApp.UpdateScene();
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
            System.Diagnostics.Process.Start("https://www.google.com/#q=New%20York%20City%20Latitude");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void ToolStripMenuItem1OnClick(object sender, EventArgs eventArgs)
        {
            Show();
            Focus();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            Configuration.Instance.Coordinates = textBox1.Text;
            Configuration.Instance.UpdateLatLong();

            _sistersApp.UpdateConfig();
            _sistersApp.UpdateScene();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            //not needed atm
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            Focus();
        }

        private void Form1_Resize(object sender, EventArgs e) {
            if (FormWindowState.Minimized == this.WindowState) {
                //notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            } else if (FormWindowState.Normal == this.WindowState) {
                //notifyIcon1.Visible = false;
            }
        }

        private void intervalBox_ValueChanged(object sender, EventArgs e)
        {
            Configuration.Instance.UpdateInterval = Math.Max((int) Math.Ceiling(intervalBox.Value), 1);
            Configuration.Instance.Save();

            intervalBox.Value = Configuration.Instance.UpdateInterval;

            _sistersApp.UpdateTimer();
        }
    }
}
