using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace 百度数据抓取
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("file://" + Environment.CurrentDirectory + @"\map.html");
            webBrowser1.ObjectForScripting = this;
        }
        public void ImportData(string data)
        {
            StreamWriter sw = new StreamWriter("resultmetro.txt");
            sw.Write(data);
            sw.Close();
            MessageBox.Show("已经完成了全部数据的抓取");
        }

        private void 抓取POI数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.InvokeScript("GetPOI");
        }
    }
}
