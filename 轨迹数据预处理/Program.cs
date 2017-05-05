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
using GPSIO;
using DotSpatial.Topology;
using DotSpatial.Data;
using System.Threading;
using DotSpatial.Projections;
using DotSpatial.Topology.KDTree;
using GeneticSharp;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Populations;
using System.Diagnostics;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Terminations;

namespace 轨迹数据预处理
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //OnceFunction.CentroidToShapefile();
            CorretionCentroid();
        }
        static int GAUSS_EPSG = 2345;
        static int WGS84_EPSG = 4326;
        static void CorretionCentroid()
        {
            //中心点数据
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Coordinate> centroidPoints = new List<Coordinate>(80);
            StreamReader sr = new StreamReader(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\中心点80.txt");
            sr.ReadLine();//读取标题行
            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(',');
                centroidPoints.Add(new Coordinate(double.Parse(line[1]), double.Parse(line[2])));
            }
            sr.Close();
            //Bus数据,并且构造KD树
            KdTree myKdtree = new KdTree(2);
            IFeatureSet busFS = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\BusStopGauss.shp");
            List<Coordinate> busStopPoints = new List<Coordinate>(busFS.NumRows());
            foreach (var item in busFS.Features)
            {
                var c = item.Coordinates[0];
                busStopPoints.Add(c);
                myKdtree.Insert(new double[] { c.X, c.Y }, item);
            }
            Console.WriteLine("数据读取完毕，开始构造遗传算法");
            IFeatureSet newCentroid = new FeatureSet(FeatureType.Point);
            newCentroid.Name = "优化过的中心点";
            newCentroid.Projection = ProjectionInfo.FromEpsgCode(GAUSS_EPSG);
            newCentroid.DataTable.Columns.Add("name", typeof(string));
            //遗传算法，构造适应性函数
            int candidateNumber = 5;
            MyProblemChromosome.CandiateNumber = candidateNumber;
            List<int[]> candinatesForEachControid = new List<int[]>(centroidPoints.Count);
            foreach (var item in centroidPoints)
            {
                object[] nearest = myKdtree.Nearest(new double[] { item.X, item.Y }, candidateNumber);
                candinatesForEachControid.Add(nearest.Select((o) =>
                {
                    var f = o as IFeature;
                    return f.Fid;
                }).ToArray());
            }
            MyProblemFitness fitness = new MyProblemFitness(centroidPoints, busStopPoints, candinatesForEachControid);
            MyProblemChromosome mpc = new MyProblemChromosome(centroidPoints.Count);
            var selection = new EliteSelection();
            var crossover = new TwoPointCrossover();
            var mutation = new ReverseSequenceMutation();
            var population = new Population(50, 70, mpc);
            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = new GenerationNumberTermination(500);
            sw.Stop();
            Console.WriteLine("遗传算法构造已经完成！");
            Console.WriteLine("一共用时：{0}s", sw.Elapsed.TotalSeconds);
            sw.Restart();
            Console.WriteLine("遗传算法正在运行.......");
            ga.Start();
            var best = ga.BestChromosome as MyProblemChromosome;
            for (int i = 0; i < best.Length; i++)
            {
                int index = candinatesForEachControid[i][(int)best.GetGene(i).Value];
                Coordinate c = busStopPoints[index];
                var f = newCentroid.AddFeature(new Point(c));
                f.DataRow.BeginEdit();
                f.DataRow["name"] = busFS.GetFeature(index).DataRow["name"];
                f.DataRow.EndEdit();
            }
            newCentroid.SaveAs("newCentroid.shp", true);
            sw.Stop();
            Console.WriteLine("全部工作已经完成，耗费时间为：{0}s，最终的fitness为：{1}", sw.Elapsed.TotalSeconds,best.Fitness);
            Console.ReadKey();
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
    }
}
