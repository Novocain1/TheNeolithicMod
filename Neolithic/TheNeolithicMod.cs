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
            api.RegisterItemClass("ItemSwappable", typeof(ItemSwappable));
            //Block Entity
            api.RegisterBlockEntityClass("GenericPOI", typeof(POIEntity));
            api.RegisterBlockEntityClass("NeolithicTransient", typeof(NeolithicTransient));
            api.RegisterBlockEntityClass("BEScary", typeof(BEScary));
            api.RegisterBlockEntityClass("FixedBESapling", typeof(FixedBESapling));
            api.RegisterBlockEntityClass("BEMortarAndPestle", typeof(BEMortarAndPestle));
        }
    }
}