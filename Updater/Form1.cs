using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Updater
{
    public partial class Form1 : Form
    {
        string mc_dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft\\";
        string temp_dir = Path.GetTempPath() + "utilityclient\\";
        string latest_version = "";

        public Form1()
        {
            SetupTempDir();

            InitializeComponent();

            label2.Hide();

            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "C# Web Client");
                var x = client.DownloadString("http://api.github.com/repos/utility-client/utilityclient2/releases/latest");

                dynamic data = JObject.Parse(x);
                latest_version = data.tag_name;
            }
        }

        private void onClick(object sender, EventArgs e)
        {
            label2.Show();
            label1.Hide();
            button1.Hide();

            var t = Task.Run(() => {
                using (var client = new WebClient())
                {
                    client.DownloadFile(getDownloadLink(latest_version), temp_dir + "client.zip");
                }

                ZipFile.ExtractToDirectory(temp_dir + "client.zip", temp_dir);

                FileInfo fi = new FileInfo(temp_dir + "client.zip");
                fi.Delete();

                DirectoryInfo di = new DirectoryInfo(mc_dir + "versions\\1.8.8-UtilityClient\\");

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }

                di.Delete();

                Directory.Move(temp_dir + "1.8.8-UtilityClient", mc_dir + "versions\\1.8.8-UtilityClient\\");
            });

            t.Wait();
            label2.Text = "Update finished.";

        }

        private string getDownloadLink(string version)
        {
            return "https://github.com/Utility-Client/UtilityClient2/releases/download/" + version + "/UtilityClient-" + version + "-1.8.8.zip";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", temp_dir);
        }

        private void SetupTempDir()
        {
            Directory.CreateDirectory(temp_dir);

            DirectoryInfo di = new DirectoryInfo(temp_dir);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", mc_dir + "versions");
        }
    }
}
