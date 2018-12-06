using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Priority_Queue;
using Cairo;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;

namespace TheNeolithicMod
{
    class CostTest : BlockBehavior
    {
        List<BlockPos> costPos = new List<BlockPos>();
        List<int> HCost = new List<int>();

        BlockPos targetPos;
        public CostTest(Block block) : base(block) { }

        public bool InBounds(IWorldAccessor world, BlockPos pos)
        {
            return pos.Y >= 1 && pos.Y <= world.BlockAccessor.MapSizeY;
        }

        public bool Passable(IWorldAccessor world, BlockPos pos)
        {
            return world.BlockAccessor.GetBlock(pos).BlockId == 0;
        }

        public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ref EnumHandling handled)
        {
            HCost.Clear();
            costPos.Clear();
            BlockPos pos = blockPos;
            int sD = 16;
            targetPos = pos + new BlockPos(0, 8, 0);
            int searchRadius = sD / 2;
            if (world.Side == EnumAppSide.Client)
            {
                for (int x = pos.X - searchRadius; x <= pos.X + searchRadius; x++)
                {
                    for (int y = pos.Y - searchRadius; y <= pos.Y + searchRadius; y++)
                    {
                        for (int z = pos.Z - searchRadius; z <= pos.Z + searchRadius; z++)
                        {
                            BlockPos currentPos = new BlockPos(x, y, z);
                            if (InBounds(world, currentPos) && Passable(world, currentPos)) {
                                costPos.Add(currentPos);
                                HCost.Add(currentPos.ManhattenDistance(targetPos));
                            }
                        }
                    }
                }
            }
            var astar = new AStarSearch(world, costPos, HCost, pos, targetPos);
            DrawGrid(world, costPos, astar, pos);
            HCost.Clear();
            costPos.Clear();
            return;
        }

        private void DrawGrid(IWorldAccessor world, List<BlockPos> grid, AStarSearch astar, BlockPos pos)
        {
            for (int x = pos.X - 16; x < pos.X + 16; x++)
            {
                for (int y = pos.Y - 16; y < pos.Y + 16; y++)
                {
                    for (int z = pos.Z - 16; z < pos.Z + 16; z++)
                    {
                        ushort ff = 32;
                        BlockPos id = new BlockPos(x, y, z);
                        BlockPos ptr = id;
                        if (!astar.cameFrom.TryGetValue(id, out ptr))
                        {
                            ptr = id;
                            ff = 0;
                        }
                        world.BlockAccessor.SetBlock(ff, ptr);
                    }
                }
            }
        }

    }
    public class AStarSearch
    {
        public Dictionary<BlockPos, BlockPos> cameFrom = new Dictionary<BlockPos, BlockPos>();
        public Dictionary<BlockPos, int> currentCost = new Dictionary<BlockPos, int>();

        static public int Heuristic(BlockPos a, BlockPos b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
        }

        public AStarSearch(IWorldAccessor world, List<BlockPos> grid, List<int> hcost, BlockPos from, BlockPos to)
        {

            var f = new SimplePriorityQueue<BlockPos>();
            f.Enqueue(from, 0);

            cameFrom[from] = from;
            currentCost[from] = 0;
            while (f.Count > 0)
            {
                var current = f.Dequeue();
                if (current.Equals(to))
                {
                    break;
                }
                int l = 0;
                foreach (var next in grid)
                {
                    int newCost = currentCost[current] + hcost[l];
                    if (!currentCost.ContainsKey(next) || newCost < currentCost[next])
                    {
                        currentCost[next] = newCost;
                        int priority = newCost + Heuristic(next, to);
                        f.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                    l += 1;
                }
            }
        }
    }
}
