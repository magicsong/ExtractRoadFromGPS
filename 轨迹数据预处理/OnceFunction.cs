using CsvHelper;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using GPSAlogrithm;
using GPSIO;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace 轨迹数据预处理
{
    public class OnceFunction
    {
        internal class RoadLink
        {
            public long RoadID { get; set; }
            public string Name { get; set; }
            public int Level { get; set; }
            public string Attribute { get; set; }
            public double Length { get; set; }
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
                using (TextReader sr = new StreamReader(ofd.FileName/*,Encoding.GetEncoding("GB2312")*/))
                {
                    var csv = new CsvReader(sr);
                    csv.Configuration.Delimiter = "\t";
                    var records = csv.GetRecords<RefPoints>().ToList();
                    foreach (var item in records)
                    {
                        double[] newPoints = CoordinateTransformUtil.bd09towgs84(item.X, item.Y);
                        item.X = newPoints[0];
                        item.Y = newPoints[1];
                        //double[] newStart = CoordinateTransformUtil.bd09towgs84(item.StartX, item.StartY);
                        //double[] newEnd = CoordinateTransformUtil.bd09towgs84(item.EndX, item.EndY);
                        //item.StartX = newStart[0];
                        //item.StartY = newStart[1];
                        //item.EndX = newEnd[0];
                        //item.EndY = newEnd[1];
                    }
                    StreamWriter sw = new StreamWriter("newRef_points_hefei.csv", false, Encoding.UTF8);
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
                var gpsData = GPSIO.GPSTrajectoryReader.ReadAll(ofd.FileName);
                foreach (var item in gpsData.GPSTrajectoriesData)
                {
                    sw.Write(item.UserID + "\t");
                    for (int i = 0; i < item.GPSCount; i++)
                    {
                        double[] newPoint = CoordinateTransformUtil.bd09towgs84(item[i].X, item[i].Y);
                        sw.Write(string.Format("{0},{1},{2}", item[i].TimeStamp, newPoint[0], newPoint[1]));
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
        private static void RoadToShapefile()
        {
            var roadNetwork = RoadNetworkReader.ReadRoadDataFromFile(@"D:\My University\数据挖掘\程序\项目用数据\newLink_hefei.csv", @"D:\My University\数据挖掘\程序\项目用数据\connectivity_hefei.txt", @"D:\My University\数据挖掘\程序\项目用数据\newRef_points_hefei.csv");
            RoadNetworkWriter.RoadNetworkToShapefile(roadNetwork, "road.shp");
        }
        private static void GPSTraceToShp()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt;*.csv";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("Application thread ID: {0}",
                        Thread.CurrentThread.ManagedThreadId);
                var gpsData = GPSIO.GPSTrajectoryReader.ReadAll(ofd.FileName);
                var t = Task.Run(() =>
                {
                    GPSTrajectoryWriter.ExportGPSTrajectoriesToShapefile("GPSTraces.shp", gpsData.GPSTrajectoriesData);
                    Console.WriteLine("Data Transform Completed!");
                    Console.ReadKey();
                });
                Console.Write("Please wait for completition");
                t.Wait();
            }
        }
        private static void GetODPoints()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt;*.csv";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("Application thread ID: {0}",
                        Thread.CurrentThread.ManagedThreadId);
                var gpsData = GPSIO.GPSTrajectoryReader.ReadAll(ofd.FileName);
                //保存成shapefile
                IFeatureSet oFS = new FeatureSet(FeatureType.Point);
                oFS.Name = "OriginalPoints";
                oFS.DataTable.Columns.Add("UserID", typeof(int));
                oFS.DataTable.Columns.Add("Time", typeof(int));
                //D
                IFeatureSet dFS = new FeatureSet(FeatureType.Point);
                dFS.Name = "DesPoints";
                dFS.DataTable.Columns.Add("UserID", typeof(int));
                dFS.DataTable.Columns.Add("Time", typeof(int));
                for (int i = 0; i < gpsData.GPSTrajectoriesData.Count; i++)
                {
                    //起点shapefile
                    var fe = oFS.AddFeature(new Point(gpsData.GPSTrajectoriesData[i].Start));
                    fe.DataRow.BeginEdit();
                    fe.DataRow["UserID"] = gpsData.GPSTrajectoriesData[i].UserID;
                    fe.DataRow["Time"] = gpsData.GPSTrajectoriesData[i].Start.TimeStamp;
                    fe.DataRow.EndEdit();
                    //终点shapefile
                    var dfe = dFS.AddFeature(new Point(gpsData.GPSTrajectoriesData[i].End));
                    dfe.DataRow.BeginEdit();
                    dfe.DataRow["UserID"] = gpsData.GPSTrajectoriesData[i].UserID;
                    dfe.DataRow["Time"] = gpsData.GPSTrajectoriesData[i].End.TimeStamp;
                    dfe.DataRow.EndEdit();
                }
                oFS.SaveAs("OriginalPoints.shp", true);
                dFS.SaveAs("DesPoints.shp", true);
            }
        }
        static void CaculateData()
        {
            ///数据读取环节
            Console.WriteLine("开始读取数据");
            IFeatureSet oFS = FeatureSet.Open(@"D:\MagicSong\OneDrive\2017研究生毕业设计\数据\项目用数据\OriginalGauss.shp");
            IFeatureSet dFS = FeatureSet.Open(@"D:\MagicSong\OneDrive\2017研究生毕业设计\数据\项目用数据\DesGauss.shp");
            List<Coordinate> centroidPoints = new List<Coordinate>(80);
            StreamReader sr = new StreamReader(@"D:\MagicSong\OneDrive\2017研究生毕业设计\数据\项目用数据\中心点.txt");
            sr.ReadLine();//读取标题行
            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(',');
                centroidPoints.Add(new Coordinate(double.Parse(line[1]), double.Parse(line[2])));
            }
            sr.Close();
            Console.WriteLine("数据读取完毕，开始处理数据");

        }
        /// <summary>
        /// Checks and searchs neighbor points for given point, there is a way to optimize performance
        /// </summary>
        /// <param name="allPoints">Dataset</param>
        /// <param name="point">centered point to be searched neighbors</param>
        /// <param name="epsilon">radius of center point</param>
        private static IFeature[] RegionQuery(IFeature[] allPoints, Coordinate point, double epsilon)
        {
            var neighborPts = allPoints.Where(x =>
            {
                Coordinate c = x.Coordinates[0];
                return c.Distance(point) <= epsilon;
            }).ToArray();
            return neighborPts;
        }
        public static void CentroidToShapefile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt;*.csv";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(ofd.FileName);
                //string[] header = sr.ReadLine().Split(',');
                IFeatureSet oFS = new FeatureSet(FeatureType.Point);
                oFS.Projection = ProjectionInfo.FromEpsgCode(4326);
                oFS.Name = "BusStop";
                oFS.DataTable.Columns.Add("Name", typeof(string));
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine().Split(',');
                    double x = double.Parse(line[2]);
                    double y = double.Parse(line[3]);
                    var xyCorrection = CoordinateTransformUtil.bd09towgs84(x, y);
                    var fe = oFS.AddFeature(new Point(xyCorrection[0], xyCorrection[1]));
                    fe.DataRow.BeginEdit();
                    fe.DataRow["Name"] = line[0];
                    fe.DataRow.EndEdit();
                }
                oFS.SaveAs("BusStop.shp", true);
                sr.Close();
                Console.WriteLine("Shapefile转换成功！");
                Console.ReadKey();
            }
        }
    }
}
