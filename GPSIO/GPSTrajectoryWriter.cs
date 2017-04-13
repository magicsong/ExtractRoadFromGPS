using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GPSCore;

namespace GPSIO
{
    public static class GPSTrajectoryWriter
    {
        public static string TrajectoriesToJson(List<GPSTrajectory> trajs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in trajs)
            {
                sb.Append("{'geo':[");
                for (int i = 0; i < item.GPSCount; i++)
                {
                    sb.AppendFormat("[{0},{1},{2}]", item[i].X, item[i].Y, item[i].TimeStamp);
                    if (i != item.GPSCount - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append("],'count':1},");
            }
            sb.Remove(sb.Length - 1, 1);//移除最后一个逗号
            sb.Append("]");
            return sb.ToString();
        }
        public static string SingleTrajectoryToJson(GPSTrajectory item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append("{'geo':[");
            for (int i = 0; i < item.GPSCount; i++)
            {
                sb.AppendFormat("[{0},{1},{2}]", item[i].X, item[i].Y, item[i].TimeStamp);
                if (i != item.GPSCount - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("],'count':1},");
            sb.Remove(sb.Length - 1, 1);//移除最后一个逗号
            sb.Append("]");
            return sb.ToString();
        }
        public static void WriteFile(string filename, IEnumerable<GPSTrajectory> trajs)
        {
            StreamWriter sw = new StreamWriter(filename);
            foreach (var item in trajs)
            {
                sw.WriteLine(item.ToString());
            }
            sw.Close();
        }
        public static string SingleTrajectoryToBaiduMapJson(GPSTrajectory path)
        {
            StringBuilder data = new StringBuilder();
            for (int i = 0; i < path.GPSCount; i++)
            {
                data.AppendFormat("{0},{1}", path[i].X, path[i].Y);
                if (i != path.GPSCount - 1)
                {
                    data.Append(",");
                }
            }
            return data.ToString();
        }
        public static string TrajectoriesToBaiduMapJson(IEnumerable<GPSTrajectory> paths)
        {
            StringBuilder data = new StringBuilder();
            foreach (var path in paths)
            {
                for (int i = 0; i < path.GPSCount; i++)
                {
                    data.AppendFormat("{0},{1}", path[i].X, path[i].Y);
                    if (i != path.GPSCount - 1)
                    {
                        data.Append(",");
                    }
                    else
                        data.Append('\n');
                }
            }
            //去掉最后一个
            data.Remove(data.Length - 1, 1);
            return data.ToString();
        }
        public static void ExportGPSTrajectoriesToShapefile(string shpfilename, IEnumerable<GPSTrajectory> trajs)
        {
            DotSpatial.Data.FeatureSet fs = ToDotSpatialData(trajs);
            fs.SaveAs(shpfilename, true);
        }
        public static string ToWKT(IEnumerable<GPSTrajectory> trajs)
        {
            StringBuilder sb = new StringBuilder();
            DotSpatial.Topology.Utilities.WktWriter ww = new DotSpatial.Topology.Utilities.WktWriter();
            foreach (var item in trajs)
            {
                sb.AppendLine(ww.Write(item.LineString));
            }
            return sb.ToString();
        }
        public static DotSpatial.Data.FeatureSet ToDotSpatialData(IEnumerable<GPSTrajectory> trajs)
        {
            DotSpatial.Data.FeatureSet fs = new DotSpatial.Data.FeatureSet(DotSpatial.Topology.FeatureType.Line);
            fs.Name = "GPSShapefile";
            fs.DataTable.Columns.Add("UserId", typeof(int));
            foreach (var item in trajs)
            {
                var fe = fs.AddFeature(item.LineString);
                fe.DataRow.BeginEdit();
                fe.DataRow["UserId"] = item.UserID;
                fe.DataRow.EndEdit();
            }

            return fs;
        }
    }
}
