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
        public List<Coordinate> CentroidPoints { get; set; }
        public List<Coordinate> BusStopPoints { get; set; }
        public List<int[]> CandiatesForEachCentroid { get; set; }
        public double Evaluate(IChromosome chromosome)
        {
            HashSet<int> checkRepeat = new HashSet<int>();
            MyProblemChromosome mpc = chromosome as MyProblemChromosome;
            for (int i = 0; i < mpc.Length; i++)
            {
                if (!checkRepeat.Add((int)mpc.GetGene(i).Value))
                    return double.MinValue;
            }
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
