namespace RPH.Utilities.AI.Decorators
{
    // System
    using System;
    using System.Linq;

    // RPH
    using Rage;

    public class GetNearestPedToPosition : Service // stores nearest ped to position in blackboard(tree memory) in "key" 
    {
        /// <param name="key">The key where the ped will be saved in the blackboard's tree memory.</param>
        /// <param name="pedPredicate">If returns true or is null the ped can be considered for nearest ped.</param>
        public GetNearestPedToPosition(string key, int interval, Vector3 position, float range, Predicate<Ped> pedPredicate, BehaviorTask child) : base(interval, (ref BehaviorTreeContext c) => DoService(key, position, range, pedPredicate, ref c), child)
        {
        }

        /// <param name="key">The key where the ped will be saved in the blackboard's tree memory.</param>
        /// <param name="pedPredicate">If returns true or is null the ped can be considered for nearest ped.</param>
        public GetNearestPedToPosition(string key, Vector3 position, float range, Predicate<Ped> pedPredicate, BehaviorTask child) : base((ref BehaviorTreeContext c) => DoService(key, position, range, pedPredicate, ref c), child)
        {
        }

        private static void DoService(string key, Vector3 position, float range, Predicate<Ped> pedPredicate, ref BehaviorTreeContext context)
        {
            Entity[] nearbyPeds = World.GetEntities(position, range, GetEntitiesFlags.ConsiderAllPeds);

            foreach (Entity e in nearbyPeds.OrderBy(e => Vector3.DistanceSquared(e.Position, position)))
            {
                Ped p = (Ped)e;
                if (pedPredicate == null || pedPredicate.Invoke(p))
                {
                    context.Agent.Blackboard.Set<Ped>(key, p, context.Tree.Id);
                    return;
                }
            }

            context.Agent.Blackboard.Set<Ped>(key, null, context.Tree.Id);
        }
    }

    public class GetNearestPedToAgent : Service // stores nearest ped to Agent.Target(if is ISpatial) in "key"
    {
        /// <param name="key">The key where the ped will be saved in the blackboard's tree memory.</param>
        /// <param name="pedPredicate">If returns true or is null the ped can be considered for nearest ped.</param>
        public GetNearestPedToAgent(string key, int interval, float range, Predicate<Ped> pedPredicate, BehaviorTask child) : base(interval, (ref BehaviorTreeContext c) => DoService(key, range, pedPredicate, ref c), child)
        {
        }

        /// <param name="key">The key where the ped will be saved in the blackboard's tree memory.</param>
        /// <param name="pedPredicate">If returns true or is null the ped can be considered for nearest ped.</param>
        public GetNearestPedToAgent(string key, float range, Predicate<Ped> pedPredicate, BehaviorTask child) : base((ref BehaviorTreeContext c) => DoService(key, range, pedPredicate, ref c), child)
        {
        }

        private static void DoService(string key, float range, Predicate<Ped> pedPredicate, ref BehaviorTreeContext context)
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
                        context.Agent.Blackboard.Set<Ped>(key, p, context.Tree.Id);
                        return;
                    }
                }
            }
            context.Agent.Blackboard.Set<Ped>(key, null, context.Tree.Id);
        }
    }
}
