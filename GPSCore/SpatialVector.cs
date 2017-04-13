using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Topology;

namespace GPSCore
{
    public class SpatialVector
    {
        double m_x;
        double m_y;
        double direction;

        public double Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public double X { get { return m_x; } set { m_x = value; } }
        public double Y { get { return m_y; } set { m_y = value; } }
        public SpatialVector(Coordinate start, Coordinate end)
        {
            m_x = end.X - start.X;
            m_y = end.Y - start.Y;
            Direction = GetDegree();
        }
        public SpatialVector(double x, double y)
        {
            m_x = x;
            m_y = y;
            Direction = GetDegree();
        }
        public SpatialVector()
        {
            m_x = 0;
            m_y = 0;
        }
        /// <summary>
        /// 向量的模
        /// </summary>
        public double Norm
        {
            get { return Math.Sqrt(this * this); }
        }
        /// <summary>
        /// 返回与x正轴的夹角，单位是角度
        /// </summary>
        public double GetDegree()
        {
            return Math.Atan2(Y, X);
        }
        public static double operator *(SpatialVector v1, SpatialVector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
        public static SpatialVector operator +(SpatialVector v1, SpatialVector v2)
        {
            return new SpatialVector(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static SpatialVector operator -(SpatialVector v1, SpatialVector v2)
        {
            return new SpatialVector(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static SpatialVector operator *(SpatialVector v1, double f)
        {
            return new SpatialVector(v1.X * f, v1.Y * f);
        }
        public static SpatialVector operator *(double f, SpatialVector v1)
        {
            return new SpatialVector(v1.X * f, v1.Y * f);
        }
        public Coordinate ToCoordinate()
        {
            return new Coordinate((float)m_y, (float)m_x);
        }
    }
}
