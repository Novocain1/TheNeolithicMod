// ClayPressBehavior by Milo Christiansen
//
// To the extent possible under law, the person who associated CC0 with
// this project has waived all copyright and related or neighboring rights
// to this project.
//
// You should have received a copy of the CC0 legalcode along with this
// work.  If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;

using Vintagestory.API;
using Vintagestory.API.Common;

namespace TheNeolithicMod
{
    class ClayPressBehavior : BlockBehavior
    {
        private AssetLocation makes;
        private int matcount = 1;

        private static AssetLocation[] inputs = new AssetLocation[]{
            new AssetLocation("game:clay-*"),
            new AssetLocation("game:metal-*"),
			// You can add mod clay types here.
		};

        public ClayPressBehavior(Block block) : base(block)
        {
            // NOP
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;

            var active = byPlayer.InventoryManager.ActiveHotbarSlot;
            if (active == null || active.Itemstack == null || active.Itemstack.Collectible == null)
            {
                return false;
            }

            bool ok = false;
            foreach (var input in inputs)
            {
                if (active.Itemstack.Collectible.WildCardMatch(input))
                {
                    ok = true;
                    break;
                }
            }
            if (!ok)
            {
                return false;
            }

            if (active.StackSize >= matcount)
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

        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);
            matcount = properties["matcount"].AsInt(matcount);
            makes = new AssetLocation(properties["makes"].AsString("game:bowl-raw"));
        }
    }
}
