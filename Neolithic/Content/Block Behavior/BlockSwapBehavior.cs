using System.Collections.Generic;

using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace TheNeolithicMod
{
    class BlockSwapBehavior : BlockBehavior
    {

        private int matcount = 1;
        private AssetLocation[][] swapBlocks;

        public BlockSwapBehavior(Block block) : base(block)
        {

        }

        public override void Initialize(JsonObject properties)
        {
            matcount = properties["matcount"].AsInt(matcount);
            swapBlocks = properties["swapBlocks"].AsObject<AssetLocation[][]>();
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
                if (active.Itemstack.Collectible.WildCardMatch(val[0]))
                {
                    makes = val[1];
                    ok = true;
                    break;
                }
            }

            if (ok && active.StackSize >= matcount)
            {
                active.Itemstack.StackSize -= matcount;
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
