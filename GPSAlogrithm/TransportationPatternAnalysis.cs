using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPSCore;

namespace GPSAlogrithm
{
    public enum TransportationMode
    {
        Stop,
        Walk,
        Vehicle,
        Other
    }
    public class BreakPointInfo
    {
        int breakIndex;
        TransportationMode transportMode;
        int segmentLength;
        public int BreakIndex
        {
            get
            {
                return breakIndex;
            }

            set
            {
                breakIndex = value;
            }
        }
        public BreakPointInfo(int breakindex, TransportationMode mode, int seg)
        {
            BreakIndex = breakindex;
            TransportMode = mode;
            segmentLength = seg;
        }
        public TransportationMode TransportMode
        {
            get
            {
                return transportMode;
            }

            set
            {
                transportMode = value;
            }
        }

        public int SegmentLength
        {
            get
            {
                return segmentLength;
            }

            set
            {
                segmentLength = value;
            }
        }
    }
    public class TransportationPatternAnalysis
    {
        public static double[] SpeedThreshold = { 1, 10, 50 };
        private static TransportationMode InWhichMode(double speed)
        {
            if (speed <= SpeedThreshold[0])
                return TransportationMode.Stop;
            else if (speed <= SpeedThreshold[1])
                return TransportationMode.Walk;
            else if (speed <= SpeedThreshold[2])
                return TransportationMode.Vehicle;
            else
                return TransportationMode.Other;
        }
        public static List<BreakPointInfo> AnalyzeOneTrajectory(GPSTrajectory traj)
        {
            if (traj.GPSCount <= 3)
                throw new Exception("GPS轨迹数据点太少");
            List<BreakPointInfo> breakinfo = new List<BreakPointInfo>();
            TransportationMode last = InWhichMode(traj[1].Velocity);
            int duration = 1;
            for (int i = 2; i < traj.GPSCount; i++)
            {
                TransportationMode mode = InWhichMode(traj[i].Velocity);
                if (mode != last)
                {
                    //检测到移动模式变化
                    BreakPointInfo bpinfo = new BreakPointInfo(i - 1, last, duration);
                    breakinfo.Add(bpinfo);
                    duration = 1;
                }
                else
                {
                    duration++;
                }
                last = mode;
            }
            //处理最后一个点
            if (duration != 1)
            {
                breakinfo.Add(new BreakPointInfo(traj.GPSCount - 1, last, duration));
            }
            return breakinfo;
        }
    }
}
