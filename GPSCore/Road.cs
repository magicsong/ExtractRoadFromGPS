using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Topology;

namespace GPSCore
{
    public struct RoadType
    {
        byte _RoadFunction;

        public byte RoadFunction
        {
            get { return _RoadFunction; }
            set { _RoadFunction = value; }
        }
        byte _RoadAtrribute;

        public byte RoadAtrribute
        {
            get { return _RoadAtrribute; }
            set { _RoadAtrribute = value; }
        }
        public RoadType(string discription)
        {
            _RoadFunction = byte.Parse(discription.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            _RoadAtrribute = byte.Parse(discription.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        }
        public RoadType(byte fun, byte attr)
        {
            _RoadFunction = fun;
            _RoadAtrribute = attr;
        }
    }
    public class Road : LineString
    {
        private List<RoadType> m_RoadAttribute;
        public List<RoadType> RoadAttribute
        {
            get { return m_RoadAttribute; }
            set { m_RoadAttribute = value; }
        }
        private Road m_UpstreamRoad;
        private double m_RoadLength;

        public double RoadLength
        {
            get { return m_RoadLength; }
            set { m_RoadLength = value; }
        }
        public Road UpstreamRoad
        {
            get { return m_UpstreamRoad; }
            set { m_UpstreamRoad = value; }
        }
        private Road m_DownstreamRoad;

        public Road DownstreamRoad
        {
            get { return m_DownstreamRoad; }
            set { m_DownstreamRoad = value; }
        }

        private byte m_RoadFunction;
        private long m_RoadID;
        private string m_RoadName;
        public long RoadID { get { return m_RoadID; } set { m_RoadID = value; } }
        public string RoadName { get { return m_RoadName; } set { m_RoadName = value; } }
        public byte RoadFunction { get { return m_RoadFunction; } set { m_RoadFunction = value; } }

        public Road(string baiduString, IEnumerable<Coordinate> pts)
            : base(pts)
        {
            string[] splitString = baiduString.Split('\t');
            m_RoadID = long.Parse(splitString[0]);
            m_RoadName = splitString[1].Trim('\"');
            m_RoadFunction = byte.Parse(splitString[2]);
            RoadAttribute = new List<RoadType>();
        }
        public Road(long id, IEnumerable<Coordinate> pts)
            : base(pts)
        {
            m_RoadID = id;
            RoadAttribute = new List<RoadType>();
        }
        public RoadSegment[] GetRoadSegments()
        {
            RoadSegment[] output = new RoadSegment[NumPoints - 1];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = new RoadSegment(this[i], this[i + 1], this.m_RoadID);
            }
            return output;
        }
        public string ToJSON()
        {
            return JSONConverter.LineStringToJSON(this);
        }
    }
}
