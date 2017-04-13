using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Topology;

namespace GPSCore
{
    public class GPSSegmentation : LineSegment
    {
        private const double threhold = 10;
        private const double extension = 0.0005f;
        private int _StartGPSTime;

        public int StartGPSTime
        {
            get { return _StartGPSTime; }
            set { _StartGPSTime = value; }
        }
        private int _EndGPSTime;
        public int ElaspedTime
        {
            get
            {
                return EndGPSTime - StartGPSTime;
            }
        }
        public double AverageSpeed
        {
            get
            {
                double dis = UsefulUtility.DistanceOnEarth(P0, P1);
                return dis / ElaspedTime;
            }
        }
        public int EndGPSTime
        {
            get { return _EndGPSTime; }
            set { _EndGPSTime = value; }
        }
        public GPSSegmentation(GPSPoint p0, GPSPoint p1) : this(p0.X, p0.Y, p0.TimeStamp, p1.X, p1.Y, p1.TimeStamp) { }
        public GPSSegmentation(double startlo, double startla, int startttime, double endlo, double endla, int endtime)
        {
            P0 = new Coordinate(startlo, startla);
            P1 = new Coordinate(endlo, endla);
            _StartGPSTime = startttime;
            _EndGPSTime = endtime;
        }
        public Envelope GetOptimizeEnvelop()
        {
            //这要有一个优化逻辑，如果接近90度或者0度的话需要增加rectangle

            double direction = UsefulUtility.RadianToDegree(Angle);
            if (Math.Abs(direction - 90) < threhold)
            {
                double stlo = 0f, edlo = 0f;
                //表示垂直的情况,小的经度更小，大的经度更大
                if (P0.X < P1.X)
                {
                    stlo = P0.X - extension;
                    edlo = P1.X + extension;
                }
                else
                {
                    stlo = P0.X + extension;
                    edlo = P1.X - extension;
                }
                return new Envelope(stlo, edlo, P0.Y, P1.Y);
            }
            else if ((90 - Math.Abs(direction - 90)) < threhold)
            {
                double stla = 0, edla = 0;
                if (P0.Y < P1.Y)
                {
                    stla = P0.Y - extension;
                    edla = P1.Y + extension;
                }
                else
                {
                    stla = P0.Y + extension;
                    edla = P1.Y - extension;
                }
                return new Envelope(P0.X, P1.X, stla, edla);
            }
            return new Envelope(P0.X, P1.X, P0.Y, P1.Y);
        }
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", P0.X, P0.Y, P1.X, P1.Y);
        }
        public string ToJSON()
        {
            return JSONConverter.LineSegmentToJSON(this);
        }
    }
}
