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
            GetDataOfEachCentoid();
            //DuplicateShapeElimination();
            //Console.WriteLine("Hello World");
        }
        static void DuplicateShapeElimination()
        {
            IFeatureSet centroid = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\newCentroid.shp");
            HashSet<string> set = new HashSet<string>();
            List<int> index = new List<int>(5);
            foreach (var item in centroid.Features)
            {
                string name = item.DataRow["name"].ToString();
                if (!set.Add(name))
                    index.Add(item.Fid);
            }
            centroid.RemoveShapesAt(index);
            Console.WriteLine("数据删除完毕");
            centroid.SaveAs("newCentroidNoDup.shp", true);
            Console.ReadKey();
        }
        static void GetDataOfEachCentoid()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("开始读取数据");
            IFeatureSet centroid = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\newCentroidNoDup.shp");
            IFeatureSet LargePoints = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\OriginalGauss.shp");
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
            List<int>[] result = new List<int>[centroid.NumRows()];
            for (int i = 0; i < result.Length; i++)
                result[i] = new List<int>();
            int core = 8;
            double eachTaskPointsCount = LargePoints.NumRows() / 8.0;
            Parallel.For(0, core, new Action<int>((index) =>
            {
                //并行任务开始
                int start = (int)Math.Ceiling(index * eachTaskPointsCount);
                int end = index != 7 ? (int)Math.Floor((index + 1) * eachTaskPointsCount) : LargePoints.NumRows() - 1;
                Console.WriteLine("开始为：{0}，终点为：{1}", start, end);
                for (int i = start; i <= end; i++)
                {
                    var currentFeature = LargePoints.GetFeature(i);
                    Coordinate c = currentFeature.Coordinates[0];
                    IFeature f = myTree.Nearest(new double[] { c.X, c.Y }) as IFeature;
                    if (f.Distance(currentFeature) < 300)
                    {
                        result[f.Fid].Add(currentFeature.Fid);
                    }
                }
            }));
            sw.Stop();
            Console.WriteLine("计算结束！还需保存！耗时：{0}", sw.Elapsed.TotalSeconds);
            Console.ReadKey();
        }
    }
}
