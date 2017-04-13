using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Topology;
namespace GPSCore
{
    public class GPSTrajectory
    {
        List<GPSPoint> m_PointCollection;
        int m_Number = 0;
        public GPSPoint this[int index]
        {
            get
            {
                if (index >= m_PointCollection.Count)
                    throw new Exception("输入的索引范围不对");
                else
                    return m_PointCollection[index];
            }
            set
            {
                if (value != null)
                {
                    if (index >= m_PointCollection.Count)
                        throw new Exception("输入的索引范围不对");
                    else
                        m_PointCollection[index] = value;
                }
            }
        }//indexer

        public LineString LineString
        {
            get
            {
                return new LineString(m_PointCollection);
            }
        }
        public int UserID { get { return m_Number; } }
        public int GPSCount { get { return m_PointCollection.Count; } }
        public GPSTrajectory(int id)
        {
            m_PointCollection = new List<GPSPoint>();
            m_Number = id;
        }
        public GPSTrajectory(int number, IEnumerable<GPSPoint> points)
        {
            m_Number = number;
            m_PointCollection = new List<GPSPoint>(points);
            for (int i = 1; i < m_PointCollection.Count; i++)
            {
                double distance = m_PointCollection[i].DistanceToOtherPointOnEarth(m_PointCollection[i - 1]);
                double time = m_PointCollection[i].TimeStamp - m_PointCollection[i - 1].TimeStamp;
                m_PointCollection[i].Direction = new SpatialVector(m_PointCollection[i - 1], m_PointCollection[i]);
                m_PointCollection[i].Velocity = distance / time;
            }
        }
        public static GPSTrajectory LineStringToGPSTrajectory(int id, LineString ls, int[] times)
        {
            if (ls.Count != times.Length)
                throw new InvalidOperationException("输入的点数和时间数不一致");
            GPSTrajectory gt = new GPSTrajectory(id);
            for (int i = 0; i < times.Length; i++)
            {
                gt.AddPoint(ls[i].X, ls[i].Y, times[i]);
            }
            return gt;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(UserID.ToString() + '\t');
            foreach (var item in m_PointCollection)
            {
                sb.AppendFormat("{0},{1},{2}|", item.TimeStamp, item.X, item.Y);
            }
            return sb.ToString(0, sb.Length - 1);
        }
        public GPSPoint AddPoint(double lo, double la, int time)
        {
            m_PointCollection.Add(new GPSPoint(lo, la, time));
            int i = m_PointCollection.Count;
            if (i != 1)
            {
                m_PointCollection[i - 1].Direction = new SpatialVector(m_PointCollection[i - 2], m_PointCollection[i - 1]);
                double distance = m_PointCollection[i - 1].DistanceToOtherPointOnEarth(m_PointCollection[i - 2]);
                double elapsedtime = m_PointCollection[i - 1].TimeStamp - m_PointCollection[i - 2].TimeStamp;
                m_PointCollection[i - 1].Velocity = distance / elapsedtime;
            }
            return m_PointCollection[i - 1];
        }
        public void AddPoint(GPSPoint pt)
        {
            AddPoint(pt.X, pt.Y, pt.TimeStamp); 
        }
        public List<GPSPoint> PointCollection
        {
            get { return m_PointCollection; }
        }
        public GPSSegmentation GetSeqmentationAtIndex(int index)
        {
            if (index >= m_Number - 1)
                throw new Exception("GPS分段的索引应该小于'轨迹的总GPS点数-1'");
            else
                return new GPSSegmentation(this[index], this[index + 1]);
        }
        public GPSPoint Start { get { return this[0]; } }
        public GPSPoint End { get { return m_PointCollection.Last(); } }
        //获取时重新计算，如果要多次用，注意保存
        public Envelope GetMBB()
        {
            return LineString.Envelope as Envelope;
        }
        /// <summary>
        /// 速度一致性检验
        /// </summary>
        /// <param name="velocity">异常速度临界值</param>
        /// <returns>True表示所有点的速度都小于临界值</returns>
        public bool CheckVelocity(double velocity)
        {
            for (int i = 1; i < GPSCount - 1; i++)
            {
                if (this[i].Velocity > velocity)
                    return false;
            }
            return true;
        }
        public bool CheckTurnAround(double dif, int threshold, out int turnaround)
        {
            turnaround = 0;
            byte straight = 0;
            for (int i = 1; i < GPSCount - 1; i++)
            {
                double difference = Math.Abs(this[i].Direction.Direction - this[i - 1].Direction.Direction);
                if (difference > dif)
                {
                    if (straight < 5)
                        turnaround += 1;
                    straight = 0;
                }
                else
                {
                    straight++;
                }
            }
            return turnaround < threshold;
        }
        public string ToJSON()
        {
            return JSONConverter.LineStringToJSON(LineString);
        }
    }
}
