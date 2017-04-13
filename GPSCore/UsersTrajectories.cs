using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GPSCore
{
    public class UsersTrajectories
    {
        SortedList<int, List<int>> usersTrajectoriesDictionary;
        List<GPSTrajectory> gpsTrajectoriesData;

       public List<GPSTrajectory> GPSTrajectoriesData
        {
            get { return gpsTrajectoriesData; }
        }
        public UsersTrajectories()
        {
            gpsTrajectoriesData = new List<GPSTrajectory>();
            usersTrajectoriesDictionary = new SortedList<int, List<int>>();
        }
        public void WriteToFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            foreach (var item in GPSTrajectoriesData)
            {
                sw.WriteLine(item.ToString());
            }
            sw.Close();
        }
        public void InsertTrajectories(GPSTrajectory traj)
        {
            int userid = traj.UserID;
            if(usersTrajectoriesDictionary.IndexOfKey(userid)==-1)
            {
                usersTrajectoriesDictionary[userid]=new List<int> ();
            }
            usersTrajectoriesDictionary[userid].Add(gpsTrajectoriesData.Count);
            gpsTrajectoriesData.Add(traj);
        }
        public List<GPSTrajectory> GetTrajectoriesByUserID(int userId)
        {
            List<int> indexes = usersTrajectoriesDictionary[userId];
            List<GPSTrajectory> output = new List<GPSTrajectory>();
            foreach (var item in indexes)
            {
                output.Add(gpsTrajectoriesData[item]);
            }
            return gpsTrajectoriesData.GetRange(indexes[0],indexes.Count);
        }
    }
}
