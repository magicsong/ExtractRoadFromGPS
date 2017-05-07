using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 轨迹数据预处理
{
    public class MyProblemChromosome : ChromosomeBase
    {
        public static int CandiateNumber { get; set; }
        public int Significance { get; set; }
        public MyProblemChromosome(int length) : base(length)
        {
            HashSet<int> checkDuplicate = new HashSet<int>();
            for (int i = 0; i < length; i++)
            {
                var current = GenerateGene(i);
                int value = (int)current.Value;
                ReplaceGene(i, current);
                int count = 1;
                while (!checkDuplicate.Add(MyProblemFitness.CandiatesForEachCentroid[i][value]))
                {
                    if (count == CandiateNumber)
                    {
                        //这边可以继续优化，如果无法满足全局要求，那么我就从最近的三个里面。
                        int value2 = RandomizationProvider.Current.GetInt(0, CandiateNumber / 2);
                        ReplaceGene(i, new Gene(value2));
                        break;
                    }
                    value = (value + 1) % CandiateNumber;
                    current = new Gene(value);
                    ReplaceGene(i, current);
                    count++;                    
                }
            }
        }

        public override IChromosome CreateNew()
        {
            return new MyProblemChromosome(Length);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            int value = RandomizationProvider.Current.GetInt(0, CandiateNumber);
            return new Gene(value);
        }
    }
}
