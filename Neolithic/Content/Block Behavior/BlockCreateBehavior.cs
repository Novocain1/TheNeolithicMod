using System;
using Vintagestory.API;
using Vintagestory.API.Common;

namespace TheNeolithicMod
{
    class BlockCreateBehavior : BlockBehavior
    {
        private int count = 1;
        private object[][] createBlocks;
        public BlockCreateBehavior(Block block) : base(block){}

        public override void Initialize(JsonObject properties)
        {
            createBlocks = properties["createBlocks"].AsObject<object[][]>();
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            AssetLocation makes = new AssetLocation("");
            var active = byPlayer.InventoryManager.ActiveHotbarSlot;
            bool ok = false;

            foreach (var val in createBlocks)
            {
                if (active.Itemstack.Collectible.WildCardMatch(new AssetLocation(val[0].ToString()))) {
                    makes = new AssetLocation(val[1].ToString());
                    count = Convert.ToInt32(val[2]);
                    ok = true;
                    break;
                }
            }

            if (ok && active.StackSize >= count)
            {
                world.SpawnItemEntity(new ItemStack(world.GetBlock(makes)), blockSel.Position.ToVec3d().Add(0.5, 0.5, 0.5), null);

                active.Itemstack.StackSize -= count;
                if (active.Itemstack.StackSize <= 0)
                {
                    active.Itemstack = null;
                }
                active.MarkDirty();
                return true;
            }
            return false;
        }
    }
}
