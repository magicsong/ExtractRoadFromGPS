using DotSpatial.Topology;

namespace GPSAlogrithm.DbscanImplementation
{
    public enum ClusterIds
    {
        Unclassified = 0,
        Noise = -1
    }
    public class DbscanPoint<T> where T:Coordinate
    {
        public bool IsVisited;
        public T ClusterPoint;
        public int ClusterId;

        public DbscanPoint(T x)
        {
            ClusterPoint = x;
            IsVisited = false;
            ClusterId = (int)ClusterIds.Unclassified;
        }

    }
}
