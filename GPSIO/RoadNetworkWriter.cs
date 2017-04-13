using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPSCore;
using DotSpatial.Data;
using DotSpatial.Topology;
using System.IO;

namespace GPSIO
{
    public static class RoadNetworkWriter
    {
        public static IFeatureSet RoadNetworkToShapefile(RoadNetwork rn, string writefilename)
        {
            FeatureSet fs = new FeatureSet(FeatureType.Line);
            string filename = Path.GetFileNameWithoutExtension(writefilename);
            fs.Name = filename;
            fs.DataTable.Columns.Add("RoadID", typeof(long));
            fs.DataTable.Columns.Add("RoadName", typeof(string));
            fs.DataTable.Columns.Add("RoadFunction", typeof(byte));
            fs.DataTable.Columns.Add("RoadLength", typeof(double));
            fs.DataTable.Columns.Add("RoadAttribute", typeof(string));
            for (int i = 0; i < rn.RoadCount; i++)
            {
                Road r = rn.GetRoadByIndex(i);
                var fe = fs.AddFeature(r);
                fe.DataRow.BeginEdit();
                fe.DataRow["RoadID"] = r.RoadID;
                fe.DataRow["RoadName"] = r.RoadName;
                fe.DataRow["RoadFunction"] = r.RoadFunction;
                fe.DataRow["RoadLength"] = r.RoadLength;
                string attr = string.Empty;
                for (int j = 0; j < r.RoadAttribute.Count; j++)
                {
                    attr += r.RoadAttribute[j].RoadFunction.ToString() + r.RoadAttribute[j].RoadAtrribute.ToString();
                    if (j != r.RoadAttribute.Count - 1)
                        attr += "|";
                }
                fe.DataRow["RoadAttribute"] = attr;
                fe.DataRow.EndEdit();
            }
            fs.SaveAs(writefilename, true);
            return fs;
        }
    }
}
