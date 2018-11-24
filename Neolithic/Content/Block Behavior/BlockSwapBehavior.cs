using System;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace TheNeolithicMod
{
    class BlockSwapBehavior : BlockBehavior
    {
        private int count = 1;
        private object[][] swapBlocks;
        public BlockSwapBehavior(Block block) : base(block){}

        public override void Initialize(JsonObject properties)
        {
            count = properties["matcount"].AsInt(count);
            swapBlocks = properties["swapBlocks"].AsObject<object[][]>();
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            AssetLocation makes = new AssetLocation("");
            BlockPos pos = blockSel.Position;
            var active = byPlayer.InventoryManager.ActiveHotbarSlot;
            bool ok = false;

            foreach (var val in swapBlocks)
            {
                if (active.Itemstack.Collectible.WildCardMatch(new AssetLocation(val[0].ToString())))
                {
                    makes = new AssetLocation(val[1].ToString());
                    count = Convert.ToInt32(val[2]);
                    ok = true;
                    break;
                }
            }

            if (ok && active.StackSize >= count)
            {
                active.Itemstack.StackSize -= count;
                if (active.Itemstack.StackSize <= 0)
                {
                    active.Itemstack = null;
                }
                world.PlaySoundAt(block.Sounds.Place, pos.X, pos.Y, pos.Z);
                world.BlockAccessor.SetBlock(world.GetBlock(makes).BlockId, pos);

                active.MarkDirty();
                return true;
            }
            return false;
        }
    }
}
