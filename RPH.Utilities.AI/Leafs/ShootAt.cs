namespace RPH.Utilities.AI.Leafs
{
    // System
    using System;
    using System.Linq;

    // RPH
    using Rage;
    using Rage.Native;

    public class ShootAt : Action
    {
        private readonly int duration;
        private readonly FiringPattern firingPattern;
        private readonly BlackboardGetter<Entity> target;

        /// <param name="target">Where to get the target <see cref="Entity"/> from the blackboard memory.</param>
        [Serialization.DeserializeBehaviorConstructor]
        public ShootAt(int duration, FiringPattern firingPattern, BlackboardGetter<Entity> target)
        {
            this.duration = duration;
            this.firingPattern = firingPattern;
            this.target = target;
        }

        protected override void OnOpen(ref BehaviorTreeContext context)
        {
            if (!(context.Agent.Target is Ped))
            {
                throw new InvalidOperationException($"The behavior action {nameof(GoToPosition)} can't be used with {context.Agent.Target.GetType().Name}, it can only be used with {nameof(Ped)}s");
            }

            context.Agent.Blackboard.Set<DateTime>("startTime", DateTime.UtcNow, context.Tree.Id, this.Id);

            Task task = context.Agent.Blackboard.Get<Task>("shootAtPedTargetTask", context.Tree.Id, this.Id, null);

            if (task == null)
            {
                Ped ped = ((Ped)context.Agent.Target);

                Entity ent = target.Get(context, this);

                if (ent)
                {
                    task = ped.Tasks.FireWeaponAt(ent, duration, firingPattern);
                    context.Agent.Blackboard.Set<Task>("shootAtPedTargetTask", task, context.Tree.Id, this.Id);
                }
            }
        }

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            Ped ped = ((Ped)context.Agent.Target);
            
            Task task = context.Agent.Blackboard.Get<Task>("shootAtPedTargetTask", context.Tree.Id, this.Id, null);

            if (task != null && task.IsActive)
            {
                DateTime startTime = context.Agent.Blackboard.Get<DateTime>("startTime", context.Tree.Id, this.Id);

                if ((DateTime.UtcNow - startTime).TotalMilliseconds > duration)
                {
                    task.Ped.Tasks.Clear();
                    return BehaviorStatus.Success;
                }

                return BehaviorStatus.Running;
            }
            else
            {
                if (task != null && !task.IsActive)
                {
                    return BehaviorStatus.Success;
                }
                else
                {
                    return BehaviorStatus.Failure;
                }
            }


        }

        protected override void OnClose(ref BehaviorTreeContext context)
        {
            context.Agent.Blackboard.Set<Task>("shootAtPedTargetTask", null, context.Tree.Id, this.Id);
        }
    }
}
