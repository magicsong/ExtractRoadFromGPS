using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Topology.Index.Strtree;
using GPSCore;

namespace GPSAlogrithm
{
    class KernelMethodWithDirection
    {
        private IRaster[] densityRasters;
        private bool isExcuted = false;
        private StRtree segmentationRTree;
        private const int DIRECTION_COUNT = 4;
        private float searchDistance = 0.1f;

        public float SearchDistance
        {
            get { return searchDistance; }
            set { searchDistance = value; }
        }
        public IRaster[] DensityRasters
        {
            get
            {
                if (!isExcuted)
                    return null;
                else
                    return densityRasters;
            }
            set { densityRasters = value; }
        }
        private List<GPSTrajectory> trajData;
        public KernelMethodWithDirection(List<GPSTrajectory> data)
        {
            trajData = data;
        }
        public KernelMethodWithDirection()
        {

        }
        public bool Excute(List<GPSTrajectory> data)
        {
            return false;
        }
        /// <summary>
        /// 获取角度所属的图幅
        /// </summary>
        /// <param name="degree">输入的角度，以度为单位</param>
        /// <returns>返回角度所在区间</returns>
        private int GetIndexOfDirection(double degree)
        {
            return (int)(degree / 180 * DIRECTION_COUNT);
        }
        public bool Excute(string outputFilename, Extent extent, double cellSize)
        {
            int numColumns = (int)(extent.Width / cellSize);
            int numRows = (int)(extent.Height / cellSize);
            for (int i = 0; i < DIRECTION_COUNT; i++)
            {
                IRaster output = Raster.CreateRaster(
                   outputFilename, string.Empty, numColumns, numRows, 1, typeof(double), new[] { string.Empty });
                output.Extent = extent;
                output.Projection = DotSpatial.Projections.ProjectionInfo.FromEpsgCode(4326);
                for (int x = 0; x < numColumns; x++)
                    for (int y = 0; y < numRows; y++)
                    {
                        Coordinate cellCenter = output.CellToProj(y, x);
                        var nearestSeg = segmentationRTree.Query(new Envelope(cellCenter));
                        for (int j = 0; j < nearestSeg.Count; j++)
                        {
                            GPSSegmentation gs = nearestSeg[j] as GPSSegmentation;
                            if(GetIndexOfDirection(gs.Angle)==i)
                            {
                                //do something
                            }
                        }
                    }
            }
            return false;
        }
        private void BulidRTree()
        {
            try
            {
                segmentationRTree = new StRtree();
                foreach (var item in trajData)
                {
                    for (int i = 0; i < item.GPSCount - 1; i++)
                    {
                        GPSSegmentation gg = item.GetSeqmentationAtIndex(i);
                        segmentationRTree.Insert(gg.GetOptimizeEnvelop(), gg);
                    }
                }
                segmentationRTree.Build();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
