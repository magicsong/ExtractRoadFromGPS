using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GPSAlogrithm.DbscanImplementation;

namespace DataMiningProgram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Shapefile文件|*.shp";
            ofd.Title = "打开一个Shapefile";
            ofd.RestoreDirectory = true;
            ofd.InitialDirectory = Environment.CurrentDirectory;
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                fileNameTextBox.Text = ofd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //首先要测试一下性能
        }
    }
}
