namespace RPH.Utilities.AI.Leafs
{
    // System
    using System;

    // RPH
    using Rage;
    using Rage.Native;

    public class GoToPosition : Action
    {
        private readonly Func<Vector3> getTarget;
        private readonly BlackboardGetter<Vector3> target;
        private readonly float speed;
        private readonly float distanceThreshold;

        [Serialization.DeserializeBehaviorConstructor]
        public GoToPosition(Vector3 targetPosition, float speed, float distanceThreshold) : this(() => targetPosition, speed, distanceThreshold)
        {
        }

        /// <param name="targetPosition">Where to get the target <see cref="Vector3"/> from the blackboard memory.</param>
        [Serialization.DeserializeBehaviorConstructor]
        public GoToPosition(BlackboardGetter<Vector3> targetPosition, float speed, float distanceThreshold) : base()
        {
            this.target = targetPosition;
            this.speed = speed;
            this.distanceThreshold = distanceThreshold;
        }

        public GoToPosition(Func<Vector3> getTargetPosition, float speed, float distanceThreshold) : base()
        {
            this.getTarget = getTargetPosition;
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
                Vector3 targetPos = getTarget?.Invoke() ?? target.Get(context, this);
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
                Ped ped = (Ped)context.Agent.Target;

                return Vector3.Distance2D(ped.Position, getTarget()) > (distanceThreshold * 1.25f) ? BehaviorStatus.Failure : BehaviorStatus.Success;
            }
        }

        protected override void OnClose(ref BehaviorTreeContext context)
        {
            context.Agent.Blackboard.Set<Task>("goToPosTask", null, context.Tree.Id, this.Id);
        }
    }
}
