using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace BehaviorsLib
{
    public class BehaviorsLib : ModSystem
	{
        public override void Start(ICoreAPI api)
		{
            AiTaskRegistry.Register("AiTaskSleep", typeof(AiTaskSleep));
            //AiTaskManager.RegisterTaskType("seekastar", typeof(AiTaskBaseAStar));
            AiTaskRegistry.Register("fixedseekfoodandeat", typeof(FixedAiTaskSeekFoodAndEat));
            //AiTaskManager.RegisterTaskType("fleepoi", typeof(AiTaskFleePOI));
            AiTaskRegistry.Register("injured", typeof(AiTaskInjured));
            api.RegisterEntityBehaviorClass("fixedmultiply", typeof(FixedEntityBehaviorMultiply));
        }
    }
}