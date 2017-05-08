using DotSpatial.Data;
using DotSpatial.Topology;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using DotSpatial.Topology.KDTree;
using System.Threading.Tasks;

namespace 轨迹数据预处理
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //OnceFunction.CentroidToShapefile();
            //CentroidCorrection.CorretionCentroid();
            Console.WriteLine("Hello World");
        }

        static void GetDataOfEachCentoid()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("开始读取数据");
            IFeatureSet centroid = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\newCentroid.shp");
            IFeatureSet LargePoints = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\myFinalPts.shp");
            sw.Stop();
            Console.WriteLine("数据读取完成，耗费时间为{0}s", sw.Elapsed.TotalSeconds);
            sw.Restart();
            Console.WriteLine("开始处理数据");
            KdTree myTree = new KdTree(2);
            foreach (var item in centroid.Features)
            {
                var c = item.Coordinates[0];
                myTree.Insert(new double[] { c.X, c.Y }, item);
            }
            Console.WriteLine("KD树构建完成");
            LargePoints.DataTable.Columns.Add("Subject", typeof(int));
            int core = 8;
            int PointsCount = LargePoints.NumRows() / 8;
            Parallel.For(0, core, new Action<int>((index) =>
            {
                //并行任务开始


            }));
        }
    }
}
