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
        public string fromCode = "";
        public string toCode = "";
        public void LoadConfig(JsonObject swapconfig)
        {
            if (swapconfig["convertFrom"] != null)
            {
                fromCode = swapconfig["convertFrom"].AsString();
            }
            if (swapconfig["convertTo"] != null)
            {
                toCode = swapconfig["convertTo"].AsString();
            }
        }
        public override void OnHeldInteractStart(IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, ref handling);
            Block block = byEntity.World.BlockAccessor.GetBlock(blockSel.Position);
            BlockPos pos = blockSel.Position;
            Block fromBlock = api.World.GetBlock(new AssetLocation(fromCode));
            Block toBlock = api.World.GetBlock(new AssetLocation(toCode));
            
            if (block != null && block == fromBlock) 
            {
                api.World.BlockAccessor.SetBlock(toBlock.BlockId, pos);
            }
        }
    }
}
