using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using DotSpatial.Topology;

namespace 轨迹数据预处理
{
    public class MyProblemFitness : IFitness
    {
        public MyProblemFitness(List<Coordinate> centroidPoints, List<Coordinate> busStopPoints, List<int[]> candiatesForEachCentroid)
        {
            CentroidPoints = centroidPoints;
            BusStopPoints = busStopPoints;
            CandiatesForEachCentroid = candiatesForEachCentroid;
        }

        static public List<Coordinate> CentroidPoints { get; set; }
        static public List<Coordinate> BusStopPoints { get; set; }
        static public List<int[]> CandiatesForEachCentroid { get; set; }
        public double Evaluate(IChromosome chromosome)
        {
            MyProblemChromosome mpc = chromosome as MyProblemChromosome;
            HashSet<int> checkDupicate = new HashSet<int>();
            int count = 0;
            for (int i = 0; i < mpc.Length; i++)
            {
                int index = (int)mpc.GetGene(i).Value;
                int id=CandiatesForEachCentroid[i][index];
                if (checkDupicate.Add(id))
                    count++;
            }
            mpc.Significance = count;
            if (count <= CentroidPoints.Count() * 0.94)
                return double.MinValue;
            double fitness = 0;
            for (int i = 0; i < mpc.Length; i++)
            {
                var o = CentroidPoints[i];
                int index = (int)mpc.GetGene(i).Value;
                var d = BusStopPoints[CandiatesForEachCentroid[i][index]];
                fitness += (o.X - d.X) * (o.X - d.X) + (o.Y - d.Y) * (o.Y - d.Y);
            }
            return -fitness;
        }
    }
}
