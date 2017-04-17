using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Topology;

namespace GPSCore
{
    public static class JSONConverter
    {
        public static string LineStringToJSON(ILineString ls)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"{geometry:{type:'LineString,coordinates:[");
            for (int i = 0; i < ls.NumPoints; i++)
            {
                sb.AppendFormat("[{0},{1}]", ls.Coordinates[i].X, ls.Coordinates[i].Y);
                if (i != ls.NumPoints - 1)
                    sb.Append(',');
            }
            sb.Append("]}}");
            return sb.ToString();
        }
        public static string LineSegmentToJSON(ILineSegment ls)
        {
            return string.Format("{\"geo\":\"lineseg\",\"data\":[[{0},{1}],[{2},{3}]]}", ls.P0.X, ls.P0.Y, ls.P1.X, ls.P1.Y);
        }
        public static string PolylineToJSON(IEnumerable<ILineString> polyline)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"geo\":\"polyline\",\"count\":" + polyline.Count() + ",\"data\":[");
            foreach (var line in polyline)
            {
                sb.Append("[");
                for (int i = 0; i < line.NumPoints; i++)
                {
                    sb.AppendFormat("[{0},{1}]", line.Coordinates[i].X, line.Coordinates[i].Y);
                    if (i != line.NumPoints - 1)
                        sb.Append(",");
                }
                sb.Append("],");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");
            return sb.ToString();
        }
        public static string MultiLineSegToJSON(IEnumerable<ILineSegment> lines)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"geo\":\"multilineseg\",\"count\":" + lines.Count()+ ",\"data\":[");
            foreach (var seg in lines)
            {
                sb.AppendFormat("[[{0},{1}],[{2},{3}]],", seg.P0.X, seg.P0.Y, seg.P1.X, seg.P1.Y);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");
            return sb.ToString();
        }
        /// <summary>
        /// 添加属性到现有JSON对象
        /// </summary>
        /// <param name="json">现有JSON对象字符串</param>
        /// <param name="elements">添加元素（不包含尾部逗号）</param>
        /// <returns>组合后的JSON</returns>
        public static string AddElementsToJSON(string json,string elements)
        {
            return "{" + elements + "," + json.Substring(1);
        }
    }
}
