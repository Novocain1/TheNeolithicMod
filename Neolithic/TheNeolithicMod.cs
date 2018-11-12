using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace TheNeolithicMod
{
    public class TheNeolithicMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            //Block Behavior
            api.RegisterBlockBehaviorClass("ClayPressBehavior", typeof(ClayPressBehavior));
            api.RegisterBlockBehaviorClass("BlockCreateBehavior", typeof(BlockCreateBehavior));

            //Block
            api.RegisterBlockClass("BlockPlaceOnDrop", typeof(BlockPlaceOnDrop));
            api.RegisterBlockClass("BlockGiantReeds", typeof(BlockGiantReeds));
            api.RegisterBlockClass("BlockMortarAndPestle", typeof(BlockMortarAndPestle));
            //api.RegisterBlockClass("BlockPlant", typeof(BlockPlant));
            //api.RegisterBlockClass("FixedBlockCrop", typeof(FixedBlockCrop));

            //Item
            api.RegisterItemClass("ItemSickle", typeof(ItemSickle));
            api.RegisterItemClass("ItemShears", typeof(ItemShears));
            api.RegisterItemClass("ItemGiantReedsRoot", typeof(ItemGiantReedsRoot));
            api.RegisterItemClass("ItemAdze", typeof(ItemAdze));
            api.RegisterItemClass("ItemSwapBlocks", typeof(ItemSwapBlocks));
            //Block Entity
            api.RegisterBlockEntityClass("GenericPOI", typeof(POIEntity));
            api.RegisterBlockEntityClass("NeolithicTransient", typeof(NeolithicTransient));
            api.RegisterBlockEntityClass("BEScary", typeof(BEScary));
            api.RegisterBlockEntityClass("FixedBESapling", typeof(FixedBESapling));
            api.RegisterBlockEntityClass("BEMortarAndPestle", typeof(BEMortarAndPestle));
        }
    }
}