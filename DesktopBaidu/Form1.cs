using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Linq;
using GPSAlogrithm;
using GPSCore;
using GPSIO;
using DotSpatial.Topology;
using Microsoft.Win32;

namespace DesktopBaidu
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
#if DEBUG
            // 调试用代码
            //TestAlogrithms();
#endif
        }

        private void TestAlogrithms()
        {
            GPSPoint p1 = new GPSPoint(117.282203, 31.86491, 0);
            GPSPoint p2 = new GPSPoint(117.281501, 31.86434, 0);
            //答案是91.65m
            double distance1 = p1.DistanceToOtherPointOnEarth(p2);
            double[] xy1 = p1.ProjectedPoint;
            double[] xy2 = p2.ProjectedPoint;
            double distance2 = Math.Sqrt((xy1[0] - xy2[0]) * (xy1[0] - xy2[0]) + (xy1[1] - xy2[1]) * (xy1[1] - xy2[1]));
            MessageBox.Show(distance1 + "----" + distance2);
        }

        private Thread workerThread = null;
        UsersTrajectories myUserTrajectories;
        private RoadNetwork myRoadNetwork;
        private StreamWriter logWriter;
        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("file://" + Application.StartupPath + @"\map.html");
            webBrowser1.ObjectForScripting = this;
            logWriter = new StreamWriter("log.txt");
            workerThread = new Thread(new ThreadStart(ConstructRoadAndData));
            workerThread.Start();
        }
        public void WriteToLog(string log)
        {
            logWriter.Write(log);
            logWriter.Flush();
        }
        void ConstructRoadAndData()
        {
            myRoadNetwork = RoadNetworkReader.ReadRoadDataFromFile(@"Data\link_hefei.txt", @"Data\connectivity_hefei.txt", @"Data\ref_points_hefei.txt");
            myRoadNetwork.BuildRoadRtree();
            toolStripProgressBar1.ProgressBar.Invoke(new Action(() => { toolStripStatusLabel1.Text = "路网以及R树构建完成"; }));
            AddGPSFile(@"Data/newTrajectoriespart.txt");
            toolStripProgressBar1.ProgressBar.Invoke(new Action(() => { toolStripStatusLabel1.Text = "GPS轨迹装载完成"; webBrowser1.Document.InvokeScript("LoadingDone"); }));
            //通知浏览器数据加载完成

        }
        public void AddRoadData()
        {
            webBrowser1.Document.InvokeScript("DrawRoad", new object[] { myRoadNetwork.ToJSON() });
        }
        public void AddGPSDataWithDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddGPSFile(ofd.FileName);
            }
        }

        private void AddGPSFile(string filename)
        {
            myUserTrajectories = GPSTrajectoryReader.ReadAll(filename);
            if (this.InvokeRequired)
                toolStripProgressBar1.ProgressBar.Invoke(new Action(() =>
                {
                    toolStripStatusLabel1.Text = "成功读入" + myUserTrajectories.GPSTrajectoriesData.Count + "条轨迹";
                }));
            else
                toolStripStatusLabel1.Text = "成功读入" + myUserTrajectories.GPSTrajectoriesData.Count + "条轨迹";
            //为了测试需要看一下删除了哪些数据
        }

        #region JS调用客户端函数
        public string GetRoadDataJS()
        {
            string result = myRoadNetwork.ToJSON();
            return result;
        }
        #endregion
        /// <summary>
        /// 导出轨迹数据到shapefile
        /// </summary>
        public void ExportGPSData()
        {
            GPSTrajectoryWriter.ExportGPSTrajectoriesToShapefile("GPSShapefile.shp", myUserTrajectories.GPSTrajectoriesData);
            MessageBox.Show("导出数据完成！");
        }
        public string ShowUserTrajectories(int id)
        {
            if (id == -1)
                return GPSTrajectoryWriter.TrajectoriesToJson(myUserTrajectories.GPSTrajectoriesData);
            else
                return GPSTrajectoryWriter.TrajectoriesToJson(myUserTrajectories.GetTrajectoriesByUserID(id));
        }
        /// <summary>
        /// 获取用户轨迹数据索引
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public string GetUserTrajectories(int id)
        {
            List<GPSTrajectory> traces = myUserTrajectories.GetTrajectoriesByUserID(id);
            StringBuilder sb = new StringBuilder();
            foreach (var item in traces)
            {
                sb.AppendFormat("{0},", item.GPSCount);
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }
        public string GetOneTrajectory(int userID, int index)
        {
            GPSTrajectory gt = myUserTrajectories.GetTrajectoriesByUserID(userID)[index];
            string result = gt.ToJSON();
            //添加速度
            StringBuilder sb = new StringBuilder();
            sb.Append("\"Velocity\":[0,");
            for (int i = 1; i < gt.GPSCount; i++)
            {
                sb.Append(gt[i].Velocity*3.6);
                if (i != gt.GPSCount - 1)
                    sb.Append(',');
            }
            sb.Append(']');
            return JSONConverter.AddElementsToJSON(result, sb.ToString());
        }
        public string TestRTree()
        {
            Random r = new Random();
            int index = r.Next(myUserTrajectories.GPSTrajectoriesData.Count);
            StringBuilder roads = new StringBuilder();
            var path = myUserTrajectories.GPSTrajectoriesData[index];
            for (int i = 0; i < path.GPSCount; i++)
            {
                //这里的查询可以优化
                var result = myRoadNetwork.RoadSegmentRtree.Query(new Envelope(path[i]));
                foreach (RoadSegment item in result)
                {
                    roads.Append(item.ToString() + "\n");
                }
            }
            return GPSTrajectoryWriter.SingleTrajectoryToBaiduMapJson(path) + "\n" + roads.ToString();
        }
        public void ShowNearestRoad(float lo, float la)
        {
            var result = myRoadNetwork.RoadSegmentRtree.Query(new Envelope(new Coordinate(lo, la)));
            webBrowser1.Document.InvokeScript("DrawLineWithString", new object[] { JSONConverter.MultiLineSegToJSON(result.Cast<RoadSegment>()) });
        }
        public string SimplifyGPSTest()
        {
            Random r = new Random();
            int index = r.Next(myUserTrajectories.GPSTrajectoriesData.Count);
            var path = myUserTrajectories.GPSTrajectoriesData[index];
            string oldjson = GPSTrajectoryWriter.SingleTrajectoryToBaiduMapJson(path);
            //获取简化后的轨迹
            GPSDouglasSimplification gds = new GPSDouglasSimplification(path, 20);
            var newpath = gds.Excute();
            return oldjson + "\n" + GPSTrajectoryWriter.SingleTrajectoryToBaiduMapJson(newpath);
        }
    }
}
