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
        Dictionary<AssetLocation, AssetLocation> swapMapping = new Dictionary<AssetLocation, AssetLocation>();
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            AssetLocation[][] arraySwapMapping = Attributes["swapBlocks"].AsObject<AssetLocation[][]>();
            foreach (var val in arraySwapMapping)
            {
                swapMapping[val[0]] = val[1];
            }
        }
        public override void OnHeldInteractStart(IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, ref handling);
            int swapRate = slot.Itemstack.Collectible.Attributes["swapRate"].AsInt(0);
            AssetLocation toCode;
            Block block = byEntity.World.BlockAccessor.GetBlock(blockSel.Position);
            BlockPos pos = blockSel.Position;

            if (!swapMapping.TryGetValue(block.Code, out toCode))
            {
                return;
            }

            if (slot.Itemstack.StackSize >= swapRate)
            {
                if (swapRate != 0)
                {
                    slot.TakeOut(swapRate);
                }
                api.World.PlaySoundAt(block.Sounds.Place, pos.X, pos.Y, pos.Z);
                api.World.BlockAccessor.SetBlock(api.World.GetBlock(toCode).BlockId, pos);
            }
        }
    }
}
