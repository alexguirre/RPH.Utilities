namespace RPH.Utilities.AI.Leafs
{
    // System
    using System;

    // RPH
    using Rage;
    using Rage.Native;

    public class GoToPosition : Action
    {
        private Func<Vector3> getTarget;
        private string targetKey;
        private float speed;
        private float distanceThreshold;

        [Serialization.DeserializeBehaviorConstructor]
        public GoToPosition(Vector3 target, float speed, float distanceThreshold) : this(() => target, speed, distanceThreshold)
        {
        }

        [Serialization.DeserializeBehaviorConstructor]
        public GoToPosition(string targetVector3Key, float speed, float distanceThreshold) : base()
        {
            this.targetKey = targetVector3Key;
            this.speed = speed;
            this.distanceThreshold = distanceThreshold;
        }

        public GoToPosition(Func<Vector3> target, float speed, float distanceThreshold) : base()
        {
            this.getTarget = target;
            this.speed = speed;
            this.distanceThreshold = distanceThreshold;
        }

        protected override void OnOpen(ref BehaviorTreeContext context)
        {
            if (!(context.Agent.Target is Ped))
            {
                throw new InvalidOperationException($"The behavior action {nameof(GoToPosition)} can't be used with {context.Agent.Target.GetType().Name}, it can only be used with {nameof(Ped)}s");
            }
            
            Task task = context.Agent.Blackboard.Get<Task>("goToPosTask", context.Tree.Id, this.Id, null);

            if (task == null)
            {
                Ped ped = ((Ped)context.Agent.Target);
                Vector3 targetPos = getTarget != null ? getTarget() : context.Agent.Blackboard.Get<Vector3>(targetKey, context.Tree.Id);
                float heading = MathHelper.ConvertDirectionToHeading((targetPos - ped.Position).ToNormalized());

                task = ped.Tasks.FollowNavigationMeshToPosition(targetPos, heading, speed, distanceThreshold);
                context.Agent.Blackboard.Set<Task>("goToPosTask", task, context.Tree.Id, this.Id);
            }
        }

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            Task task = context.Agent.Blackboard.Get<Task>("goToPosTask", context.Tree.Id, this.Id, null);

            if (task != null && task.IsActive)
            {
                return BehaviorStatus.Running;
            }
            else
            {
                Ped ped = ((Ped)context.Agent.Target);
                if (Vector3.Distance2D(ped.Position, getTarget()) > (distanceThreshold * 1.25f))
                {
                    return BehaviorStatus.Failure;
                }
                else
                {
                    return BehaviorStatus.Success;
                }
            }
        }

        protected override void OnClose(ref BehaviorTreeContext context)
        {
            context.Agent.Blackboard.Set<Task>("goToPosTask", null, context.Tree.Id, this.Id);
        }
    }
}
