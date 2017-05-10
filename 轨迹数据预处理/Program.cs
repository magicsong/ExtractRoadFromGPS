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
            Console.WriteLine("Hello World");
            GetDataOfEachPoints();
            Console.ReadKey();
        }
        static void GetDataOfEachPoints()
        {
            //是按照O-D的顺序来的
            IFeatureSet desPoints = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\DesGauss.shp");
            IFeatureSet orgPoints = FeatureSet.Open(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\OriginalGauss.shp");
            string newColumnName = "Subject";
            desPoints.DataTable.Columns.Add(newColumnName, typeof(int));
            orgPoints.DataTable.Columns.Add(newColumnName, typeof(int));
            StreamReader sr = new StreamReader(@"D:\OneDrive\2017研究生毕业设计\数据\项目用数据\统计用数据\cluster数据.txt");
            sr.ReadLine();
            Console.WriteLine("标题栏读取结束");
            int count = 0;
            int NUMBER = 105243;
            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split();
                if (count < NUMBER)
                {
                    IFeature current = orgPoints.Features[count];
                    current.DataRow.BeginEdit();
                    current.DataRow[newColumnName] = int.Parse(line[1]) - 1;
                    current.DataRow.EndEdit();
                }
                else
                {
                    IFeature current = desPoints.Features[count-NUMBER];
                    current.DataRow.BeginEdit();
                    current.DataRow[newColumnName] = int.Parse(line[1]) - 1;
                    current.DataRow.EndEdit();
                }
                count++;
            }
            sr.Close();
            Console.WriteLine("字段添加完成，开始保存数据");
            orgPoints.SaveAs("OriginalGaussWithSubject.shp", true);
            desPoints.SaveAs("DesGaussWithSubject.shp", true);
        }
    }
}
