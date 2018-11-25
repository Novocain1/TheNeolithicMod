using System;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace TheNeolithicMod
{
    class BlockSwapBehavior : BlockBehavior
    {
        private int count = 1;
        private bool ok;
        private object[][] swapBlocks;
        private AssetLocation makes;
        private AssetLocation requires;
        public BlockSwapBehavior(Block block) : base(block){}

        public override void Initialize(JsonObject properties)
        {
            swapBlocks = properties["swapBlocks"].AsObject<object[][]>();
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            var active = byPlayer.InventoryManager.ActiveHotbarSlot;
            makes = new AssetLocation("game:rock-granite");
            BlockPos pos = blockSel.Position;
            Block block = world.BlockAccessor.GetBlock(pos);
            requires = block.Code;
            ok = false;

            foreach (var val in swapBlocks)
            {
                if (active.Itemstack != null && active.Itemstack.Collectible.WildCardMatch(new AssetLocation(val[0].ToString())))
                {
                 
                    makes = new AssetLocation(val[1].ToString());
                    count = Convert.ToInt32(val[2]);
                    if (new AssetLocation(val[3].ToString()) != requires) { requires = new AssetLocation(val[3].ToString()); }
                    ok = true;
                    break;
                }
            }

            if (active.Itemstack != null && ok && active.StackSize >= count && new ItemStack(world.BlockAccessor.GetBlock(pos)).Collectible.WildCardMatch(requires))
            {
                active.Itemstack.StackSize -= count;

                if (world.Side == EnumAppSide.Client)
                {
                    world.PlaySoundAt(block.Sounds.Place, pos.X, pos.Y, pos.Z);
                }
                if (active.Itemstack.StackSize <= 0)
                {
                    active.Itemstack = null;
                }
                world.BlockAccessor.SetBlock(world.GetBlock(makes).BlockId, pos);

                active.MarkDirty();
                return true;
            }
            return false;
        }
    }
}
