using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Cairo;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;

namespace TheNeolithicMod
{
    class CostTest : BlockBehavior
    {
        List<BlockPos> costPos = new List<BlockPos>();
        List<int> HCosts = new List<int>();
        BlockPos targetPos;
        public CostTest(Block block) : base(block) { }

        public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ref EnumHandling handled)
        {
            HCosts.Clear();
            costPos.Clear();
            BlockPos pos = blockPos;
            BlockPos tO = new BlockPos(16, 0, 0);
            int sD = 16;
            targetPos = pos + tO + new BlockPos(sD-world.Rand.Next(0, sD * 2), sD - world.Rand.Next(0, sD * 2), sD - world.Rand.Next(0, sD * 2));
            int searchRadius = sD / 2;
            if (world.Side == EnumAppSide.Client)
            {
                for (int x = pos.X - searchRadius; x <= pos.X + searchRadius; x++)
                {
                    for (int y = pos.Y - searchRadius; y <= pos.Y + searchRadius; y++)
                    {
                        for (int z = pos.Z - searchRadius; z <= pos.Z + searchRadius; z++)
                        {
                            costPos.Add(new BlockPos(x, y, z) + tO);
                        }
                    }
                }
                costPos.Add(targetPos);
                int l = 0;
                foreach (var val in costPos)
                {
                    HCosts.Add(val.ManhattenDistance(targetPos));
                    if (val.Y >= 1 && val.Y <= world.BlockAccessor.MapSizeY)
                    {
                        world.BlockAccessor.SetBlock((ushort)(HCosts[l] + 2), val);
                    }
                    l += 1;
                }
            }
            return;
        }

        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref EnumHandling handling)
        {
            int l = 0;
            foreach (var val in costPos)
            {
                if (val.Y >= 1 && val.Y <= world.BlockAccessor.MapSizeY)
                {
                    world.BlockAccessor.SetBlock(0, val);
                }
                l += 1;
            }
            HCosts.Clear();
            costPos.Clear();
            return;
        }

    }
}
