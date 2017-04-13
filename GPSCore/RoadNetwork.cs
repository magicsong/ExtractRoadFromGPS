using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Topology;
using DotSpatial.Topology.Index.Strtree;

namespace GPSCore
{
    public class RoadNetwork
    {
        private List<Road> _Roads;
        private SortedDictionary<long, int> _RoadsIndex;
        private StRtree _RoadRtree;
        public int RoadCount
        {
            get
            {
                return _Roads.Count;
            }
        }
        public Road GetRoadByIndex(int index)
        {
            if (index >= RoadCount)
                throw new ArgumentOutOfRangeException("输入索引大于路总数");
            return _Roads[index];
        }
        public Road[] GetAllRoads()
        {
            return _Roads.ToArray();
        }
        public StRtree RoadRtree
        {
            get { return _RoadRtree; }
        }
        private StRtree _RoadSegmentRtree;

        public StRtree RoadSegmentRtree
        {
            get { return _RoadSegmentRtree; }
        }
        public void AddRoad(Road road)
        {
            if (_RoadsIndex.ContainsKey(road.RoadID))
            {
                throw new Exception("道路ID不唯一");
            }
            else
            {
                _Roads.Add(road);
                _RoadsIndex.Add(road.RoadID, _Roads.Count - 1);
            }
        }
        public Road GetRoadById(long id)
        {
            if (_RoadsIndex.ContainsKey(id))
                return _Roads[_RoadsIndex[id]];
            else return null;
        }
        public RoadNetwork()
        {
            _Roads = new List<Road>();
            _RoadsIndex = new SortedDictionary<long, int>();
        }
        public RoadNetwork(IEnumerable<Road> roads)
        {
            _Roads = new List<Road>(roads.Count());
            _RoadsIndex = new SortedDictionary<long, int>();
            _Roads.AddRange(roads);
            for (int i = 0; i < roads.Count(); i++)
            {
                _RoadsIndex.Add(roads.ElementAt(i).RoadID, i);
            }
        }
        /// <summary>
        /// 构建道路的R树，需要一段时间，最好用多线程
        /// </summary>
        public void BuildRoadRtree()
        {
            _RoadRtree = new StRtree();
            foreach (var item in _Roads)
            {
                _RoadRtree.Insert(item.Envelope, item);
            }
            _RoadRtree.Build();
        }
        public string ToJSON()
        {
            return JSONConverter.PolylineToJSON(_Roads);
        }
        /// <summary>
        /// 构建道路段的R树，所需时间更长，最好用多线程
        /// </summary>
        public void BuildRoadSegmentRtree()
        {
            _RoadSegmentRtree = new StRtree();
            foreach (var item in _Roads)
            {
                var segs = item.GetRoadSegments();
                foreach (var seg in segs)
                {
                    RoadSegmentRtree.Insert(seg.GetOptimizeEnvelop(), seg);
                }
            }
            _RoadSegmentRtree.Build();
        }
    }
}
