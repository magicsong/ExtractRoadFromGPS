using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPSCore;
using System.IO;

namespace GPSIO
{
    public static class GPSTrajectoryReader
    {
        public static UsersTrajectories ReadAll(string filename)
        {
            UsersTrajectories myUserTrajectories = new UsersTrajectories();
            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    GPSTrajectory gt = ReadLine(sr.ReadLine());
                    myUserTrajectories.InsertTrajectories(gt);
                }
                return myUserTrajectories;
            }
        }
        public static GPSTrajectory ReadLine(string line)
        {
            int split = line.IndexOf('\t');
            int num = int.Parse(line.Substring(0, split));
            GPSTrajectory gt = new GPSTrajectory(num);
            string[] data = line.Substring(split + 1).Split(new char[] { ',', '|' });
            for (int i = 0; i < data.Length; i++)
            {
                int time = int.Parse(data[i++]);
                float lo = float.Parse(data[i++]);
                float la = float.Parse(data[i]);
                gt.AddPoint(lo, la, time);
            }
            return gt;
        }
    }
}
