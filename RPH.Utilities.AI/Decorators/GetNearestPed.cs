namespace RPH.Utilities.AI.Decorators
{
    // System
    using System;
    using System.Linq;

    // RPH
    using Rage;

    public class GetNearestPedToPosition : Service
    {
        private readonly BlackboardSetter<Ped> pedSetter;
        private readonly Vector3 position;
        private readonly float range;
        private readonly Predicate<Ped> pedPredicate;

        /// <param name="pedSetter">Where the <see cref="Ped"/> will be saved in the blackboard memory.</param>
        /// <param name="pedPredicate">If returns true or is null the ped can be considered for nearest ped.</param>
        public GetNearestPedToPosition(BlackboardSetter<Ped> pedSetter, int interval, Vector3 position, float range, Predicate<Ped> pedPredicate, BehaviorTask child) : base(interval, null, child)
        {
            this.pedSetter = pedSetter;
            this.position = position;
            this.range = range;
            this.pedPredicate = pedPredicate;

            ServiceMethod = DoService;
        }

        /// <param name="pedSetter">Where the <see cref="Ped"/> will be saved in the blackboard memory.</param>
        /// <param name="pedPredicate">If returns true or is null the ped can be considered for nearest ped.</param>
        public GetNearestPedToPosition(BlackboardSetter<Ped> pedSetter, Vector3 position, float range, Predicate<Ped> pedPredicate, BehaviorTask child) : base(null, child)
        {
            this.pedSetter = pedSetter;
            this.position = position;
            this.range = range;
            this.pedPredicate = pedPredicate;

            ServiceMethod = DoService;
        }

        private void DoService(ref BehaviorTreeContext context)
        {
            Entity[] nearbyPeds = World.GetEntities(position, range, GetEntitiesFlags.ConsiderAllPeds);

            foreach (Entity e in nearbyPeds.OrderBy(e => Vector3.DistanceSquared(e.Position, position)))
            {
                Ped p = (Ped)e;
                if (pedPredicate == null || pedPredicate.Invoke(p))
                {
                    pedSetter.Set(context, this, p);
                    return;
                }
            }

            pedSetter.Set(context, this, null);
        }
    }

    public class GetNearestPedToAgent : Service
    {
        private readonly BlackboardSetter<Ped> pedSetter;
        private readonly float range;
        private readonly Predicate<Ped> pedPredicate;

        /// <param name="pedSetter">Where the <see cref="Ped"/> will be saved in the blackboard memory.</param>
        /// <param name="pedPredicate">If returns true or is null the ped can be considered for nearest ped.</param>
        public GetNearestPedToAgent(BlackboardSetter<Ped> pedSetter, int interval, float range, Predicate<Ped> pedPredicate, BehaviorTask child) : base(interval, null, child)
        {
            this.pedSetter = pedSetter;
            this.range = range;
            this.pedPredicate = pedPredicate;

            ServiceMethod = DoService;
        }

        /// <param name="pedSetter">Where the <see cref="Ped"/> will be saved in the blackboard memory.</param>
        /// <param name="pedPredicate">If returns true or is null the ped can be considered for nearest ped.</param>
        public GetNearestPedToAgent(BlackboardSetter<Ped> pedSetter, float range, Predicate<Ped> pedPredicate, BehaviorTask child) : base(null, child)
        {
            this.pedSetter = pedSetter;
            this.range = range;
            this.pedPredicate = pedPredicate;

            ServiceMethod = DoService;
        }

        private void DoService(ref BehaviorTreeContext context)
        {
            if (!(context.Agent.Target is ISpatial))
            {
                throw new InvalidOperationException($"The behavior condition {nameof(GetNearestPedToAgent)} can't be used with {context.Agent.Target.GetType().Name}, it can only be used with {nameof(ISpatial)}s");
            }

            Vector3 position = ((ISpatial)context.Agent.Target).Position;

            Entity[] nearbyPeds = World.GetEntities(position, range, GetEntitiesFlags.ConsiderAllPeds);

            foreach (Entity e in nearbyPeds.OrderBy(e => Vector3.DistanceSquared(e.Position, position)))
            {
                Ped p = (Ped)e;
                if (pedPredicate == null || pedPredicate.Invoke(p))
                {
                    Ped contextPed = context.Agent.Target as Ped;
                    if (contextPed == null || contextPed != p)
                    {
                        pedSetter.Set(context, this, p);
                        return;
                    }
                }
            }
            pedSetter.Set(context, this, null);
        }
    }
}
