using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using DotSpatial.Topology.KDTree;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace 轨迹数据预处理
{
    public class CentroidCorrection
    {
        static int GAUSS_EPSG = 2345;
        static int WGS84_EPSG = 4326;
        public static void CorretionCentroid()
        {
            //中心点数据
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Coordinate> centroidPoints = new List<Coordinate>(80);
            StreamReader sr = new StreamReader(@"D:\MagicSong\OneDrive\2017研究生毕业设计\数据\项目用数据\中心点80.txt");
            sr.ReadLine();//读取标题行
            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split(',');
                centroidPoints.Add(new Coordinate(double.Parse(line[1]), double.Parse(line[2])));
            }
            sr.Close();
            //Bus数据,并且构造KD树
            KdTree myKdtree = new KdTree(2);
            IFeatureSet busFS = FeatureSet.Open(@"D:\MagicSong\OneDrive\2017研究生毕业设计\数据\项目用数据\BusStopGauss.shp");
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
            MyProblemChromosome.CandiateNumber = 5;
            List<int[]> candinatesForEachControid = new List<int[]>(centroidPoints.Count);
            foreach (var item in centroidPoints)
            {
                object[] nearest = myKdtree.Nearest(new double[] { item.X, item.Y }, MyProblemChromosome.CandiateNumber);
                candinatesForEachControid.Add(nearest.Select((o) =>
                {
                    var f = o as IFeature;
                    return f.Fid;
                }).ToArray());
            }
            MyProblemFitness fitness = new MyProblemFitness(centroidPoints, busStopPoints, candinatesForEachControid);
            MyProblemChromosome mpc = new MyProblemChromosome(centroidPoints.Count);
            //这边可以并行
            MyProblemChromosome globalBest = null;
            Console.WriteLine("遗传算法构造已经完成！");
            sw.Stop();
            Console.WriteLine("一共用时：{0}s", sw.Elapsed.TotalSeconds);
            int GACount = 8;
            Parallel.For(0, GACount, new Action<int>((index) =>
            {
                var selection = new EliteSelection();
                var crossover = new TwoPointCrossover();
                var mutation = new ReverseSequenceMutation();
                var population = new Population(1000, 1200, mpc);
                var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
                ga.Termination = new GenerationNumberTermination(1000);
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                Console.WriteLine("遗传算法任务{0}正在运行.......", index);
                ga.Start();
                var best = ga.BestChromosome as MyProblemChromosome;
                if (globalBest == null || globalBest.Fitness < best.Fitness)
                    globalBest = best;
                sw1.Stop();
                Console.WriteLine("第{0}次遗传算法已经完成，耗费时间为：{1}s，最终的fitness为：{2},有效个数为：{3}", index, sw1.Elapsed.TotalSeconds, best.Fitness, best.Significance);
            }));
            Console.WriteLine("Final Choose!");
            Console.WriteLine("最终的fitness为：{0},有效个数为：{1}", globalBest.Fitness, globalBest.Significance);
            for (int i = 0; i < globalBest.Length; i++)
            {
                int index = candinatesForEachControid[i][(int)globalBest.GetGene(i).Value];
                Coordinate c = busStopPoints[index];
                var f = newCentroid.AddFeature(new Point(c));
                f.DataRow.BeginEdit();
                f.DataRow["name"] = busFS.GetFeature(index).DataRow["name"];
                f.DataRow.EndEdit();
            }
            newCentroid.SaveAs("newCentroid.shp", true);
            Console.ReadKey();
        }
    }
}
