using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace BbB
{
	public class BrickByBrick : ModSystem
	{
		
		public override void Start(ICoreAPI api)
		{
			
			base.Start(api);
			api.RegisterBlockBehaviorClass("LampConnectorBehavior", typeof(LampConnectorBehavior));
			api.RegisterBlockBehaviorClass("LampPostBehavior", typeof(LampPostBehavior));
			api.RegisterBlockBehaviorClass("rotateninety", typeof(rotateninety));
		}
	}
}