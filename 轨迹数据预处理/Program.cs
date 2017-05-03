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

namespace 轨迹数据预处理
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

        }
        static void CaculateData()
        {
            ///数据读取环节
            Console.WriteLine("开始读取数据");
            IFeatureSet oFS = FeatureSet.Open(@"D:\MagicSong\OneDrive\2017研究生毕业设计\数据\项目用数据\OriginalGauss.shp");
            IFeatureSet dFS = FeatureSet.Open(@"D:\MagicSong\OneDrive\2017研究生毕业设计\数据\项目用数据\DesGauss.shp");
            List<Coordinate> centroidPoints = new List<Coordinate>(80);
            StreamReader sr=new StreamReader (@"D:\MagicSong\OneDrive\2017研究生毕业设计\数据\项目用数据\中心点.txt");
            sr.ReadLine();//读取标题行
            while(!sr.EndOfStream)
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
        private static IFeature[]RegionQuery(IFeature[] allPoints, Coordinate point, double epsilon)
        {
            var neighborPts = allPoints.Where(x => {
                Coordinate c = x.Coordinates[0];
                return c.Distance(point) <= epsilon;
            }).ToArray();
            return neighborPts;
        }
    }
}
