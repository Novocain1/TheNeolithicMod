using System;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace BehaviorsLib
{
    public class AiTaskInjured : AiTaskBase
    {
        public Vec3d pos;
        public AiTaskInjured(EntityAgent entity) : base(entity)
        {
        }

        public override void LoadConfig(JsonObject taskConfig, JsonObject aiConfig)
        {
            base.LoadConfig(taskConfig, aiConfig);
        }

        public override bool ShouldExecute()
        {
            //if (entity.GetBehavior<EntityBehaviorHealth>().Health < entity.GetBehavior<EntityBehaviorHealth>().MaxHealth / 4) return true;
            if (!entity.Alive) return true;
            return false;
        }

        public override void StartExecute()
        {
            base.StartExecute();
        }

        public override bool ContinueExecute(float dt)
        {
            base.ContinueExecute(dt);
            return true;
        }

    }
}
