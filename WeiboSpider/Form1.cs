using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.IO;

namespace WeiboSpider
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeChromium();
        }
        public ChromiumWebBrowser chromeBrowser;
        private string myScripts;
        private string nextPage;
        private StreamWriter sw;
        private bool isStartSpider = false;
        private int CurrentCount = 1;
        public void InitializeChromium()
        {
            string website = "http://s.weibo.com/weibo/%25E5%2585%25AC%25E5%2585%25B1%25E8%2587%25AA%25E8%25A1%258C%25E8%25BD%25A6";
            CefSettings settings = new CefSettings();
            settings.CachePath = Environment.CurrentDirectory + @"\Caches\";
            settings.PersistSessionCookies = true;
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser(website);
            //chromeBrowser.BrowserSettings.ImageLoading = CefState.Disabled;
            //chromeBrowser.LoadingStateChanged += ChromeBrowser_LoadingStateChanged;
            // Add it to the form and fill it to the form window.
            splitContainer1.Panel2.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;
        }

        private void ChromeBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (isStartSpider && !e.IsLoading)
            {
                OneTask();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader("spider.js");
            myScripts = sr.ReadToEnd();
            sr.Close();
            textBox3.Text = timer1.Interval.ToString();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cef.Shutdown();
            if (sw != null)
                sw.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sw = new StreamWriter(textBox1.Text, true, Encoding.UTF8);
            isStartSpider = true;
            toolStripProgressBar1.MarqueeAnimationSpeed = 30;
            timer1.Start();
            var btn = sender as Button;
            btn.Enabled = false;
            button4.Enabled = true;
        }

        private void OneTask()
        {
            var task = chromeBrowser.GetMainFrame().EvaluateScriptAsync(myScripts, null);
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    object EvaluateJavaScriptResult = response.Success ? (response.Result ?? "null") : response.Message;
                    string[] data = EvaluateJavaScriptResult.ToString().Split('\n');
                    if (data.Length <= 1)
                    {
                        Invoke(new Action(() =>
                        {
                            timer1.Stop();                                           
                            toolStripProgressBar1.MarqueeAnimationSpeed = 0;
                            textBox2.AppendText("抓取完毕或者出现错误了");
                            //chromeBrowser.BrowserSettings.ImageLoading = CefState.Enabled;
                            //chromeBrowser.Reload();
                            this.TopMost = true;
                            if (MessageBox.Show("出现了错误请处理！") == DialogResult.OK)
                            {
                                TopMost = false;
                            }
                        }));
                    }
                    else
                    {
                        nextPage = data[0];
                        chromeBrowser.Load(nextPage);
                        for (int i = 1; i < data.Length; i++)
                        {
                            sw.WriteLine(data[i]);
                        }
                        Invoke(new Action(() =>
                        {
                            toolStripStatusLabel1.Text = "抓取成功！";
                            textBox2.AppendText(string.Format("第{0}次抓取数据成功！下一页是{1}", CurrentCount++, nextPage));                            
                        }));
                    }
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //chromeBrowser.BrowserSettings.ImageLoading = CefState.Disabled;
            timer1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripProgressBar1.MarqueeAnimationSpeed = 30;
            OneTask();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            timer1.Interval = int.Parse(textBox3.Text);
        }
    }
}
