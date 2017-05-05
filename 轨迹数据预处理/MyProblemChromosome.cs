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
                    value = (value + 1) % CandiateNumber;
                    current = new Gene(value);
                    ReplaceGene(i, current);
                    count++;
                    if (count == CandiateNumber)
                    {
                        ReplaceGene(i, GenerateGene(i));
                        break;
                    }
                }
            }
        }

        public override IChromosome CreateNew()
        {
            return new MyProblemChromosome(Length);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            int value = RandomizationProvider.Current.GetInt(0, 5);
            return new Gene(value);
        }
    }
}
