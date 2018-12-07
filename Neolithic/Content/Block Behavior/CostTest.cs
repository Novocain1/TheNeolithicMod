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
        HashSet<BlockPos> costPos = new HashSet<BlockPos>();
        Dictionary<BlockPos, int> pastCost = new Dictionary<BlockPos, int>();

        BlockPos targetPos;
        public CostTest(Block block) : base(block) { }

        public bool isInBounds(IWorldAccessor world, BlockPos pos)
        {
            return pos.Y >= 1 && pos.Y <= world.BlockAccessor.MapSizeY;
        }

        public bool isPassable(IWorldAccessor world, BlockPos pos)
        {
            return world.BlockAccessor.GetBlock(pos).GetCollisionBoxes(world.BlockAccessor, pos) == null;
        }

        public bool isWalkable(IWorldAccessor world, BlockPos pos)
        {
            return (world.BlockAccessor.GetBlock(pos + new BlockPos(0, -1, 0)).GetCollisionBoxes(world.BlockAccessor, pos) != null || world.BlockAccessor.GetBlock(pos + new BlockPos(0, -1, 0)).WildCardMatch(new AssetLocation("game:water*")));
        }

        public bool isDangerous(IWorldAccessor world, BlockPos pos)
        {
            for (int i = 0; i < world.BlockAccessor.MapSizeY; i++)
            {
                if (world.BlockAccessor.GetBlock(pos + new BlockPos(0, -i, 0)).WildCardMatch(new AssetLocation("game:lava*"))) return true;
            }
            return false;
        }

        public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ref EnumHandling handled)
        {
            if (world.Side != EnumAppSide.Server) return;
            pastCost.Clear();
            costPos.Clear();
            BlockPos pos = blockPos;
            int sD = 16;
            targetPos = pos + new BlockPos(-2, 0, 8);
            int sR = sD / 2;
            var grid = new CubeGrid(sD, sD, sD);
            if (!isInBounds(world, targetPos) || !isPassable(world, targetPos) || !isWalkable(world, targetPos) || isDangerous(world, targetPos))
            {
                return;
            }
            for (int x = pos.X - sR; x <= pos.X + sR; x++)
            {
                for (int y = pos.Y - sR; y <= pos.Y + sR; y++)
                {
                    for (int z = pos.Z - sR; z <= pos.Z + sR; z++)
                    {
                        BlockPos currentPos = new BlockPos(x, y, z);
                        if (isInBounds(world, currentPos) && isPassable(world, currentPos) && isWalkable(world, currentPos) && !isDangerous(world, currentPos))
                        {
                            costPos.Add(currentPos);
                            pastCost.Add(currentPos, currentPos.ManhattenDistance(targetPos));
                            grid.air.Add(currentPos);
                        }
                        else
                        {
                            grid.walls.Add(currentPos);
                        }
                    }
                }
            }
            var astar = new AStarSearch(world, grid, pastCost, pos, targetPos);
            DrawGrid(world, astar, pos, sR);
            world.BlockAccessor.SetBlock(1, pos);
            world.BlockAccessor.SetBlock(903, targetPos);
            return;
        }

        private void DrawGrid(IWorldAccessor world, AStarSearch astar, BlockPos pos, int sR)
        {
            for (int x = pos.X - sR; x < pos.X + sR; x++)
            {
                for (int y = pos.Y - sR; y < pos.Y + sR; y++)
                {
                    for (int z = pos.Z - sR; z < pos.Z + sR; z++)
                    {
                        ushort ff = 32;
                        BlockPos id = new BlockPos(x, y, z);
                        BlockPos ptr = id;
                        if (!astar.cameFrom.TryGetValue(id, out ptr))
                        {
                            ptr = id;
                            ff = 0;
                        }
                        if (ff != 0) world.BlockAccessor.SetBlock(ff, ptr);
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

        public AStarSearch(IWorldAccessor world, IWeightedGraph<BlockPos> graph, Dictionary<BlockPos, int> pastCost, BlockPos from, BlockPos to)
        {
            cameFrom.Clear();
            currentCost.Clear();
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
                foreach (var next in graph.Neighbors(world, current))
                {
                   // pastCost.TryGetValue(next, out int pC);
                    int newCost = currentCost[current];
                    if (!currentCost.ContainsKey(next) || newCost < currentCost[next])
                    {
                        currentCost[next] = newCost;
                        int priority = newCost + Heuristic(next, to);
                        f.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }
        }
    }
    public interface IWeightedGraph<L>
    {
        int Cost(BlockPos a, BlockPos b);
        IEnumerable<BlockPos> Neighbors(IWorldAccessor world, BlockPos id);
    }

    public class CubeGrid : IWeightedGraph<BlockPos>
    {
        public HashSet<BlockPos> DIRS = new HashSet<BlockPos>();
        public int width, height, length;

        public CubeGrid(int width, int height, int length)
        {
            this.width = width;
            this.height = height;
            this.length = length;
        }
        public HashSet<BlockPos> walls = new HashSet<BlockPos>();
        public HashSet<BlockPos> air = new HashSet<BlockPos>();

        public bool isInBounds(IWorldAccessor world, BlockPos pos)
        {
            return pos.Y >= 1 && pos.Y <= world.BlockAccessor.MapSizeY;
        }

        public bool isPassable(IWorldAccessor world, BlockPos pos)
        {
            return world.BlockAccessor.GetBlock(pos).GetCollisionBoxes(world.BlockAccessor, pos) == null;
        }

        public bool isWalkable(IWorldAccessor world, BlockPos pos)
        {
            return (world.BlockAccessor.GetBlock(pos + new BlockPos(0, -1, 0)).GetCollisionBoxes(world.BlockAccessor, pos) != null || world.BlockAccessor.GetBlock(pos + new BlockPos(0, -1, 0)).WildCardMatch(new AssetLocation("game:water*")));
        }

        public bool isDangerous(IWorldAccessor world, BlockPos pos)
        {
            for (int i = 0; i <= 4; i++)
            {
                if (world.BlockAccessor.GetBlock(pos + new BlockPos(0, -i, 0)).WildCardMatch(new AssetLocation("game:lava*")))
                {
                    return true;
                }
            }
            return false;
        }

        public int Cost(BlockPos a, BlockPos b)
        {
            return air.Contains(b) ? 5 : 1;
        }

        public IEnumerable<BlockPos> Neighbors(IWorldAccessor world, BlockPos id)
        {
            DIRS.Clear();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -4; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        DIRS.Add(new BlockPos(x, y, z));
                    }
                }
            }
            foreach (var dir in DIRS)
            {
                BlockPos next = new BlockPos(id.X + dir.X, id.Y + dir.Y, id.Z + dir.Z);
                if (isInBounds(world, next) && isPassable(world, next) && isWalkable(world, next) && !isDangerous(world, next))
                {
                    yield return next;
                }
            }
        }
    }
}
