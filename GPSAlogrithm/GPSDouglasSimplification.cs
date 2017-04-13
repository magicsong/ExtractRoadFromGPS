using GPSCore;

namespace GPSAlogrithm
{
    public class GPSDouglasSimplification
    {
        
        private double _distanceTolerance;
        /// <summary>
        /// 自己实现的GPS轨迹类，继承了linestring类，额外添加了用户，时间和速度信息
        /// </summary>
        private GPSTrajectory GPSPts;
        /// <summary>
        /// 最大阈值，调节简化的粗细
        /// </summary>
        public double DistanceTolerance
        {
            get { return _distanceTolerance; }
            set { _distanceTolerance = value; }
        }
        /// <summary>
        /// 用来标记是否保存轨迹中某一个点，长度与GPS轨迹的点数一致
        /// </summary>
        private bool[] _usePt;
        public GPSDouglasSimplification()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="traj">GPS轨迹</param>
        /// <param name="maxdistance">最大阈值</param>
        public GPSDouglasSimplification(GPSTrajectory traj, double maxdistance)
        {
            GPSPts = traj;
            _distanceTolerance = maxdistance;
        }
        /// <summary>
        /// DP算法的核心算法，是一个分治迭代函数，当I==j-1迭代结束
        /// </summary>
        /// <param name="i">开始点</param>
        /// <param name="j">结束点</param>
        private void SimplifySection(int i, int j)
        {
            if ((i + 1) == j)
                return;
            GPSSegmentation _seg = new GPSSegmentation(GPSPts[i], GPSPts[j]);
            double maxDistance = -1.0;
            int maxIndex = i;
            for (int k = i + 1; k < j; k++)
            {
                double distance = _seg.Distance(GPSPts[k]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = k;
                }
            }
            if (maxDistance <= _distanceTolerance)
                for (int k = i + 1; k < j; k++)
                    _usePt[k] = false;
            else
            {
                SimplifySection(i, maxIndex);
                SimplifySection(maxIndex, j);
            }
        }
        private GPSTrajectory Simplify()
        {
            _usePt = new bool[GPSPts.GPSCount];
            for (int i = 0; i < GPSPts.GPSCount; i++)
                _usePt[i] = true;
            SimplifySection(0, GPSPts.GPSCount-1);
            GPSTrajectory simplifiedLine = new GPSTrajectory(GPSPts.UserID);
            for (int i = 0; i < GPSPts.GPSCount; i++)
                if (_usePt[i])
                    simplifiedLine.AddPoint(GPSPts[i]);
            return simplifiedLine;
        }
        public GPSTrajectory Excute(GPSTrajectory source,double maxdistance)
        {
            _distanceTolerance = maxdistance;
            GPSPts = source;
            return Simplify();
        }
        public GPSTrajectory Excute()
        {
            return Simplify();
        }
    }
}
