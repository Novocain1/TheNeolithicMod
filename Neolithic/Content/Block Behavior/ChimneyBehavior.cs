using System;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace TheNeolithicMod
{
    class ChimneyBehavior : BlockBehavior
    {
        private AssetLocation[][] lookFor;
        public BlockPos pos;
        public IWorldAccessor world;
        long listenerId;
        public ChimneyBehavior(Block block) : base(block) { }

        public override void Initialize(JsonObject properties)
        {
            lookFor = properties["lookFor"].AsObject<AssetLocation[][]>();
        }

        public override bool TryPlaceBlock(IWorldAccessor aworld, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling)
        {
            pos = blockSel.Position;
            world = aworld; //idunno, becomes null and breaks if I don't do this
            listenerId = world.RegisterGameTickListener(ticker, 1000);
            return true;
        }

        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref EnumHandling handling)
        {
            world.UnregisterGameTickListener(listenerId);
        }

        public void ticker(float dt)
        {
            checkBelow(pos, world);
        }

        public void checkBelow(BlockPos pos, IWorldAccessor world)
        {
            foreach (var val in lookFor)
            {
                BlockPos unlitpos = new BlockPos(0,0,0);
                BlockPos litpos = new BlockPos(0,0,0);
                AssetLocation jc = world.BlockAccessor.GetBlock(pos).Code;
                for (int y = 0; y < 16; y++)
                {
                    AssetLocation fc = world.BlockAccessor.GetBlock(pos + new BlockPos(0, -y, 0)).Code;
                    if (new ItemStack(world.GetBlock(jc)).Collectible.WildCardMatch(val[3]))
                    {
                        if (new ItemStack(world.GetBlock(fc)).Collectible.WildCardMatch(val[0]))
                        {
                            world.BlockAccessor.SetBlock(world.GetBlock(val[1]).BlockId, pos);
                            unlitpos = pos + new BlockPos(0, -y, 0);
                            break;
                        }

                    }
                    if (new ItemStack(world.GetBlock(jc)).Collectible.WildCardMatch(val[1]))
                    {
                        if (new ItemStack(world.GetBlock(fc)).Collectible.WildCardMatch(val[2]))
                        {
                            world.BlockAccessor.SetBlock(world.GetBlock(val[3]).BlockId, pos);
                            litpos = pos + new BlockPos(0, -y, 0);
                            break;
                        }
                    }
                    /*
                    if (!new ItemStack(world.BlockAccessor.GetBlock(unlitpos)).Collectible.WildCardMatch(val[0]) || !new ItemStack(world.BlockAccessor.GetBlock(litpos)).Collectible.WildCardMatch(val[2]))
                    {
                        world.BlockAccessor.SetBlock(world.GetBlock(val[1]).BlockId, pos);
                        break;
                    }
                    */
                }
                //world.UnregisterGameTickListener(listenerId);

            }
            return;
        }
    }
}
