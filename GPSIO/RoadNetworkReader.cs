using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Topology;
using GPSCore;
using System.IO;

namespace GPSIO
{
    public static class RoadNetworkReader
    {
        public static RoadNetwork ReadRoadDataFromFile(string attrfile, string connectionfile, string refpointsfile)
        {
            StreamReader sr = new StreamReader(refpointsfile);
            //读取形状数据
            SortedDictionary<long, Road> roads = new SortedDictionary<long, Road>();
            long currentId = 0;
            List<Coordinate> oneRoad = new List<Coordinate>();
            while (!sr.EndOfStream)
            {
                string[] oneline = sr.ReadLine().Split('\t');
                int currentOrder = int.Parse(oneline[2]);
                if (currentOrder == 0)
                {
                    
                    //表示上面一段已经结束
                    if (oneRoad.Count > 0)
                    {
                        Road r = new Road(currentId, oneRoad.ToArray());
                        roads.Add(currentId, r);
                        currentId = long.Parse(oneline[1]);
                        oneRoad.Clear();
                    }
                    else
                        currentId = long.Parse(oneline[1]);
                }
                oneRoad.Add(new Coordinate(double.Parse(oneline[3]), double.Parse(oneline[4])));
            }
            roads.Add(currentId, new Road(currentId, oneRoad));
            sr.Close();
            //属性文件
            sr = new StreamReader(attrfile);
            while (!sr.EndOfStream)
            {
                string[] oneline = sr.ReadLine().Split('\t');
                long id = long.Parse(oneline[0]);
                Road r = roads[id];
                r.RoadName = oneline[1].Trim('\"');
                r.RoadFunction = byte.Parse(oneline[2], System.Globalization.NumberStyles.HexNumber);
                r.RoadLength = double.Parse(oneline[3]);
                string[] types = oneline[4].Split('|');
                foreach (var item in types)
                {
                    r.RoadAttribute.Add(new RoadType(item));
                }
            }
            sr.Close();
            //邻接文件
            sr = new StreamReader(connectionfile);
            while (!sr.EndOfStream)
            {
                string[] oneline = sr.ReadLine().Split('\t');
                long up = long.Parse(oneline[0]);
                long down = long.Parse(oneline[1]);
                roads[up].DownstreamRoad = roads[down];
                roads[down].UpstreamRoad = roads[up];
            }
            sr.Close();
            RoadNetwork rn = new RoadNetwork(roads.Values);
            return rn;
        }
    }
}
