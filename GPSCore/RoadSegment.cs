using System;
using System.Collections.Generic;
using DotSpatial.Topology;
namespace GPSCore
{
    public class RoadSegment : LineSegment
    {
        private const double threhold = 10;
        private const float extension = 0.0005f;
        private long m_ParentRoadID;

        public long ParentRoadID
        {
            get { return m_ParentRoadID; }
            set { m_ParentRoadID = value; }
        }
        public RoadSegment(ILineSegmentBase ls, long id)
            : base(ls)
        {
            m_ParentRoadID = id;
        }
        public RoadSegment(Coordinate p0, Coordinate p1, long id)
            : base(p0, p1)
        {
            m_ParentRoadID = id;
        }
        /// <summary>
        /// 路网的方向，路网只有0-180度
        /// </summary>
        public override double Angle
        {
            get
            {
                return base.Angle % 180;
            }
        }


        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", P0.X, P0.Y, P1.X, P1.Y);
        }
        public string ToJSON()
        {
            return JSONConverter.LineSegmentToJSON(this);
        }
        /// <summary>
        /// 为了增强索引，将线的Envelop扩大
        /// </summary>
        /// <returns>增强后的Envelop</returns>
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
    }
}
