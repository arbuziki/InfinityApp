using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infinity.Data;
using Infinity.HDD.Utils;

namespace Infinity.HDD.Implementation
{
    public class HDDHdaPoint : IHdaPoint
    {
        private readonly PointInfo _info;


        public string Item { get; set; }
        public string Description { get; set; }
        public string EngUnit { get; set; }

        public HDDHdaPoint(PointInfo info)
        {
            _info = info;
        }

        public IEnumerable<HdaPointValue> GetValues()
        {
            var values =
                    CompressedBlocksUtils.LoadFromParts("points",
                                                        _info.Folder,
                                                        _info.ValuesCount,
                                                        HDDDataManager.BLOCK_SIZE);

            return values.OfType<HdaPointValue>();
        }
    }
}
