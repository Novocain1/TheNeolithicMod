using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace BehaviorsLib
{
    public class BehaviorsLib : ModSystem
	{
        public override void Start(ICoreAPI api)
		{
		    AiTaskManager.RegisterTaskType("AiTaskSleep", typeof(AiTaskSleep));
            //AiTaskManager.RegisterTaskType("seekastar", typeof(AiTaskBaseAStar));
            AiTaskManager.RegisterTaskType("fixedseekfoodandeat", typeof(FixedAiTaskSeekFoodAndEat));
            //AiTaskManager.RegisterTaskType("fleepoi", typeof(AiTaskFleePOI));
            AiTaskManager.RegisterTaskType("injured", typeof(AiTaskInjured));
            
        }
    }
}