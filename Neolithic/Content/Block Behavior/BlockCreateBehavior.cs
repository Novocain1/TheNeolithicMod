﻿using System.Collections.Generic;

using Vintagestory.API;
using Vintagestory.API.Common;

namespace TheNeolithicMod
{
    class BlockCreateBehavior : BlockBehavior
    {

        private int matcount = 1;
        private AssetLocation[][] createBlocks;

        public BlockCreateBehavior(Block block) : base(block)
        {

        }

        public override void Initialize(JsonObject properties)
        {
            matcount = properties["matcount"].AsInt(matcount);
            createBlocks = properties["createBlocks"].AsObject<AssetLocation[][]>();
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            AssetLocation makes = new AssetLocation("");
            var active = byPlayer.InventoryManager.ActiveHotbarSlot;
            bool ok = false;

            foreach (var val in createBlocks)
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
                world.SpawnItemEntity(new ItemStack(world.GetBlock(makes)), blockSel.Position.ToVec3d().Add(0.5, 0.5, 0.5), null);

                active.Itemstack.StackSize -= matcount;
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
