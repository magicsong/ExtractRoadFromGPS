using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Topology;
using DotSpatial.Projections;

namespace GPSCore
{
    public class GPSPoint : Coordinate
    {
        int m_TimeStamp = 0;
        double velocity = 0;
        SpatialVector directionVector;
        public double[] ProjectedPoint
        {
            get { return ProjectPoint(117, this); }
        }
        public SpatialVector Direction
        {
            get { return directionVector; }
            set { directionVector = value; }
        }

        public double Velocity
        {
            get { return velocity; }
            set
            {
                if (value < 0) throw new Exception("速度值不能小于0");
                velocity = value;
            }
        }
        //表示以秒为单位的时间
        public int TimeStamp { get { return m_TimeStamp; } set { if (value > 0)m_TimeStamp = value; } }
        public GPSPoint(double lo, double la, int time)
        {
            TimeStamp = time;
            X = lo;
            Y = la;
        }
        public GPSPoint(Coordinate c, int time) : this(c.X, c.Y, time) { }
        public SpatialVector ToSpatialVector()
        {
            return new SpatialVector(X, Y);
        }
        public double DistanceToOtherPointOnEarth(Coordinate c)
        {
            return UsefulUtility.DistanceOnEarth(this, c);
        }
        public static double[] ProjectPoint(double meridian, GPSPoint p)
        {
            //这边采用高斯投影吧
            var source = KnownCoordinateSystems.Geographic.Asia.Beijing1954;
            var density = KnownCoordinateSystems.Projected.GausKrugerBeijing1954.Beijing19543DegreeGKCM117E;
            double[] xy = new double[] { p.X, p.Y };
            Reproject.ReprojectPoints(xy, new double[] { 0 }, source, density, 0, 1);
            return xy;
        }
    }
}
