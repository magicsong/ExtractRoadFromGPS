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
using DotSpatial.Data;
using DotSpatial.Topology;

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
            HashSet<IFeature[]> clusters = new HashSet<IFeature[]>();
            int numpts = minPtsTrackBar.Value;
            double eps = double.Parse(textBox2.Text);
            Task.Factory.StartNew(new Action(() => {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                IFeatureSet fs = FeatureSet.Open(fileNameTextBox.Text);
                minPtsTrackBar.Invoke(new Action(() => {
                    logTextBox.Text += "数据读取成功，用时：" + sw.Elapsed.TotalSeconds + "秒" + Environment.NewLine;
                }));
                //数据已经准备好
                DbscanAlgorithm<IFeature> da = new DbscanAlgorithm<IFeature>((fp1, fp2) => {
                    var p1 = fp1.Coordinates[0];
                    var p2 = fp2.Coordinates[0];
                    return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
                });
                sw.Restart();
                minPtsTrackBar.Invoke(new Action(() => {
                    logTextBox.Text += "算法开始！" + Environment.NewLine;
                }));
                da.ComputeClusterDbscan(fs.Features.ToArray().ToArray(), eps, numpts, out clusters);
                sw.Stop();
                minPtsTrackBar.Invoke(new Action(() => {
                    logTextBox.Text += "结算结束，用时："+sw.Elapsed.TotalSeconds+"秒" + Environment.NewLine;
                }));
            }));            
        }

        private void minPtsTrackBar_Scroll(object sender, EventArgs e)
        {
            label4.Text = minPtsTrackBar.Value.ToString();
        }
    }
}
