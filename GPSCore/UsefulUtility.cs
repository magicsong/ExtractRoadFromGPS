using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Topology;

namespace GPSCore
{
    public static class UsefulUtility
    {
        static double _R = 6370996.81;

        /// <summary>
        /// 地球半径
        /// </summary>
        public static double R
        {
            get { return UsefulUtility._R; }
            set { UsefulUtility._R = value; }
        }
        /// <summary>
        /// 角度转换为弧度
        /// </summary>
        /// <param name="de">输入的角度</param>
        /// <returns>返回弧度</returns>
        public static double DegreeToRadian(double de)
        {
            return de * Math.PI / 180;
        }

        /// <summary>
        /// 弧度转换为角度
        /// </summary>
        /// <param name="ra">输入的弧度</param>
        /// <returns>返回角度</returns>
        public static double RadianToDegree(double ra)
        {
            return ra / Math.PI * 180;
        }
        /// <summary>
        /// 求地球上两点的距离，输入都需要经纬度（非弧度）
        /// </summary>
        /// <param name="p1">第一个点</param>
        /// <param name="p2">第二个点</param>
        /// <returns>距离</returns>
        public static double DistanceOnEarth(Coordinate p1, Coordinate p2)
        {
            //需要求实际距离
            return R * Math.Acos(Math.Cos(DegreeToRadian(p1.Y)) * Math.Cos(DegreeToRadian(p2.Y)) * Math.Cos(DegreeToRadian(p1.X - p2.X)) + Math.Sin(DegreeToRadian(p1.Y)) * Math.Sin(DegreeToRadian(p2.Y)));
        }
    }
}
