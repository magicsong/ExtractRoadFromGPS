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
        public MyProblemChromosome(int length) : base(length)
        {
            for(int i=0;i<length;i++)
            {
                ReplaceGene(i, GenerateGene(i));
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
