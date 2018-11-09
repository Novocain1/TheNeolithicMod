using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API;
using Vintagestory.GameContent;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace TheNeolithicMod
{
    class ItemSwappable : Item
    {

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
        }
        public override void OnHeldInteractStart(IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, ref handling);
            int swapRate = slot.Itemstack.Collectible.Attributes["swapRate"].AsInt(0);
            Block block = byEntity.World.BlockAccessor.GetBlock(blockSel.Position);
            BlockPos pos = blockSel.Position;
            Block fromBlock = api.World.GetBlock(new AssetLocation(slot.Itemstack.Collectible.Attributes["fromCode"].AsString()));
            Block toBlock = api.World.GetBlock(new AssetLocation(slot.Itemstack.Collectible.Attributes["toCode"].AsString()));

            if (block != null && block == fromBlock && slot.Itemstack.StackSize >= swapRate) 
            {
                if (swapRate != 0)
                {
                    slot.TakeOut(swapRate);
                }
                api.World.PlaySoundAt(block.Sounds.Place, pos.X, pos.Y, pos.Z);
                api.World.BlockAccessor.SetBlock(toBlock.BlockId, pos);
            }
        }
    }
}
