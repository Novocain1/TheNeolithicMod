using System;
using System.Collections.Generic;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace TheNeolithicMod
{
    public class AiTaskAstar : AiTaskBase
    {
        float seekingR = 25;
        int li = 0;
        long listenerId;
        BlockPos strPos;
        private Dictionary<int, BlockPos> path = new Dictionary<int, BlockPos>();

        public AiTaskAstar(EntityAgent entity) : base(entity)
        {
        }

        public override void LoadConfig(JsonObject taskConfig, JsonObject aiConfig)
        {
            base.LoadConfig(taskConfig, aiConfig);
            if (taskConfig["seekingRange"] != null)
            {
                seekingR = taskConfig["seekingRange"].AsFloat(25);
            }
        }

        public override bool ShouldExecute()
        {
            BlockPos strPos = entity.ServerPos.AsBlockPos;
            BlockPos tgtPos = entity.World.GetNearestEntity(entity.ServerPos.XYZ, seekingR, seekingR).Pos.AsBlockPos;
            var grid = new CubeGrid((int)seekingR, (int)seekingR, (int)seekingR);
            if (!grid.isInBounds(world, tgtPos) || !grid.isPassable(world, tgtPos) || !grid.isWalkable(world, tgtPos))
            {
                return false;
            }
            return true;
        }

        public override void StartExecute()
        {
            if (world.Side != EnumAppSide.Server) return;
            base.StartExecute();
            strPos = entity.ServerPos.AsBlockPos;
            BlockPos tgtPos = entity.World.GetNearestEntity(entity.ServerPos.XYZ, seekingR, seekingR, (e) => {
                if (!e.Alive || !e.IsInteractable || e.EntityId == this.entity.EntityId) return false;
                return true;
            }).Pos.AsBlockPos;
            Vec3d posv = strPos.ToVec3d();
            Vec3d tgtv = tgtPos.ToVec3d();
            BlockPos mP = ((posv.Add(tgtv)) / 2).AsBlockPos;
            int sR = (int)mP.DistanceTo(tgtPos) + 4;
            var grid = new CubeGrid(sR * 2, sR * 2, sR * 2);

            for (int x = mP.X - sR; x <= mP.X + sR; x++)
            {
                for (int y = mP.Y - sR; y <= mP.Y + sR; y++)
                {
                    for (int z = mP.Z - sR; z <= mP.Z + sR; z++)
                    {
                        BlockPos currentPos = new BlockPos(x, y, z);
                        if (grid.isInBounds(world, currentPos) && grid.isPassable(world, currentPos) && grid.isWalkable(world, currentPos))
                        {
                            grid.air.Add(currentPos);
                        }
                        else
                        {
                            grid.walls.Add(currentPos);
                        }
                    }
                }
            }
            var astar = new AStarSearch(world, grid, strPos, tgtPos);
            WalkPath(world, astar, mP, sR);
        }

        public override bool ContinueExecute(float dt)
        {
            return true;
        }

        private void WalkPath(IWorldAccessor world, AStarSearch astar, BlockPos pos, int sR)
        {
            path.Clear();
            int i = 0;
            for (int x = pos.X - sR; x < pos.X + sR; x++)
            {
                for (int y = pos.Y - sR; y < pos.Y + sR; y++)
                {
                    for (int z = pos.Z - sR; z < pos.Z + sR; z++)
                    {
                        BlockPos id = new BlockPos(x, y, z);
                        if (!astar.cameFrom.TryGetValue(id, out BlockPos ptr))
                        {
                            ptr = id;
                        }
                        else
                        {
                            //world.BlockAccessor.SetBlock(2277, ptr);
                            path.Add(i, ptr);
                            i += 1;
                        }
                    }
                }
            }
            listenerId = world.RegisterGameTickListener(walkPath, 500);
        }

        private void walkPath(float dt)
        {
            BlockPos goTo = strPos;
            int mx = path.Count;
            path.TryGetValue(li, out goTo);
            if (li < mx && goTo != null)
            {
                li += 1;
                pathTraverser.GoTo(goTo.ToVec3d(), 0.05f, OnGoalReached, OnStuck);
                world.BlockAccessor.SetBlock(2277, goTo);
            }
            else
            {
                li = 0;
                world.UnregisterGameTickListener(listenerId);
            }
        }

        private void OnStuck()
        {
            bool stuck = true;
        }

        private void OnGoalReached()
        {
            pathTraverser.Active = true;
        }

    }
}
