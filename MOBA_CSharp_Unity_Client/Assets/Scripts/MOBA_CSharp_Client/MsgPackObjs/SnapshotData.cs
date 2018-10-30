using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBA_CSharp_Server.MsgPackObjs
{
    [MessagePackObject]
    public class SnapshotData
    {
        [Key(0)]
        public PlayerData[] playerDatas { get; set; }
    }
}
