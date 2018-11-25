using System;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace TheNeolithicMod
{
    class BlockCreateBehavior : BlockBehavior
    {
        private int count = 1;
        private bool ok;
        private object[][] createBlocks;
        private AssetLocation makes;
        public BlockCreateBehavior(Block block) : base(block){}

        public override void Initialize(JsonObject properties)
        {
            createBlocks = properties["createBlocks"].AsObject<object[][]>();
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            var active = byPlayer.InventoryManager.ActiveHotbarSlot;
            makes = new AssetLocation("");
            BlockPos pos = blockSel.Position;
            ok = false;

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
                active.Itemstack.StackSize -= count;
                if (world.Side == EnumAppSide.Client)
                {
                    world.PlaySoundAt(block.Sounds.Place, pos.X, pos.Y, pos.Z);
                }
                if (world.Side == EnumAppSide.Server) { 
                    world.SpawnItemEntity(new ItemStack(world.GetBlock(makes), 1), pos.ToVec3d().Add(0.5, 0.5, 0.5), null);
                }

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
