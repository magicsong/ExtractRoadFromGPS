using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using CsvHelper;
using GPSAlogrithm;
using GPSCore;
using DotSpatial.Topology;

namespace 轨迹数据预处理
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //ProecessingOne();
            //TransformGPS();
            ProcessingGPSTrajectories();
        }
        internal class RoadLink
        {
            public long RoadID { get; set; }
            public string Name { get; set; }
            public int Level { get; set; }
            public string Attribute { get; set; }
            public double StartX { get; set; }
            public double StartY { get; set; }
            public double EndX { get; set; }
            public double EndY { get; set; }
        }
        public class RefPoints
        {
            public int ID { get; set; }
            public long RoadID { get; set; }
            public int SerialNum { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
        }
        private static void TransformGPS()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt;*.csv";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (TextReader sr = new StreamReader(ofd.FileName))
                {
                    var csv = new CsvReader(sr);
                    csv.Configuration.Delimiter = "\t";
                    var records = csv.GetRecords<RefPoints>().ToList();
                    foreach (var item in records)
                    {
                        double[] newPoints = CoordinateTransformUtil.bd09towgs84(item.X, item.Y);
                        item.X = newPoints[0];
                        item.Y = newPoints[1];
                    }
                    StreamWriter sw = new StreamWriter("newRef_points_hefei.csv",false,Encoding.UTF8);
                    var csvwriter = new CsvWriter(sw);
                    csvwriter.WriteRecords(records);
                    sw.Close();
                }
                Console.WriteLine("Data Transform Completed!");
                Console.ReadKey();
            }
        }
        private static void ProcessingGPSTrajectories()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt;*.csv";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter("newTraces.txt");
                var gpsData=GPSIO.GPSTrajectoryReader.ReadAll(ofd.FileName);
                foreach (var item in gpsData.GPSTrajectoriesData)
                {
                    sw.Write(item.UserID + "\t");
                    for(int i=0;i<item.GPSCount;i++)
                    {
                        double[] newPoint = CoordinateTransformUtil.bd09towgs84(item[i].X, item[i].Y);
                        sw.Write(string.Format("{0},{1},{2}", item[i].TimeStamp, newPoint[0],newPoint[1]));
                        if (i != item.GPSCount - 1)
                            sw.Write("|");                       
                    }
                    sw.Write(Environment.NewLine);
                }
                sw.Close();
                Console.WriteLine("Data Transform Completed!");
                Console.ReadKey();
            }
        }
        private static void ProecessingOne()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("var gpsdata=[");
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        int split = line.IndexOf('\t');
                        int num = int.Parse(line.Substring(0, split));
                        sb.Append("{'geo':[");
                        string[] data = line.Substring(split + 1).Split(new char[] { ',', '|' });
                        for (int i = 0; i < data.Length; i += 3)
                        {
                            //添加时间信息
                            sb.Append("[" + data[i + 1] + "," + data[i + 2] + "," + data[i].Substring(0, 7) + "]");
                            if (i != data.Length - 3)
                            {
                                sb.Append(',');
                            }
                        }
                        sb.Append("],'count':1},");
                    }
                    sb.Remove(sb.Length - 1, 1);//移除最后一个逗号
                    sb.Append("];");
                    StreamWriter sw = new StreamWriter("gpstrajectories.js");
                    sw.Write(sb.ToString());
                    sw.Close();
                }
            }
        }
    }
}
