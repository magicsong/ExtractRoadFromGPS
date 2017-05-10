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
            GetDataOfEachCentoid2();
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
            IFeatureSet LargePoints = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\DesGauss.shp");
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
            StreamWriter streamw = new StreamWriter("summaryDes.txt");
            foreach (var item in result)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    streamw.Write(item[i]);
                    if (i != item.Count - 1)
                        streamw.Write(',');
                }
                streamw.Write(Environment.NewLine);
            }
            streamw.Close();
            Console.WriteLine("保存完毕！");
            Console.ReadKey();
        }
        static void GetDataOfEachCentoid2()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("开始读取数据");
            IFeatureSet centroid = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\newCentroidNoDup.shp");
            IFeatureSet LargePoints = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\OriginalGauss.shp");
            string newColumnName = "Subject";
            LargePoints.DataTable.Columns.Add(newColumnName, typeof(int));
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
            int core = 8;
            IFeature[] newFeatures = LargePoints.Features.ToArray();
            double eachTaskPointsCount = LargePoints.NumRows() / 8.0;
            Parallel.For(0, core, new Action<int>((index) =>
            {
                //并行任务开始
                int start = (int)Math.Ceiling(index * eachTaskPointsCount);
                int end = index != 7 ? (int)Math.Floor((index + 1) * eachTaskPointsCount) : LargePoints.NumRows() - 1;
                Console.WriteLine("开始为：{0}，终点为：{1}", start, end);
                for (int i = start; i <= end; i++)
                {
                    var currentFeature = newFeatures[i];
                    Coordinate c = currentFeature.Coordinates[0];
                    IFeature f = myTree.Nearest(new double[] { c.X, c.Y }) as IFeature;
                    lock (currentFeature.DataRow.Table)
                    {
                        currentFeature.DataRow.BeginEdit();
                        if (f.Distance(currentFeature) <= 350)
                            currentFeature.DataRow[newColumnName] = f.Fid;
                        else
                            currentFeature.DataRow[newColumnName] = -1;
                        currentFeature.DataRow.EndEdit();
                    }
                }
            }));
            sw.Stop();
            Console.WriteLine("计算结束！还需保存！耗时：{0}", sw.Elapsed.TotalSeconds);
            FeatureSet newFeatureSet = new FeatureSet(newFeatures);
            newFeatureSet.Name = "OriginalGaussWithSubject";
            newFeatureSet.Projection = LargePoints.Projection;
            newFeatureSet.SaveAs(newFeatureSet.Name + ".shp", true);
            Console.WriteLine("保存完毕！");
            Console.ReadKey();
        }
    }
}
