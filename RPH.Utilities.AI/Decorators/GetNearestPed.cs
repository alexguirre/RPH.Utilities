namespace RPH.Utilities.AI.Decorators
{
    // System
    using System;
    using System.Linq;

    // RPH
    using Rage;
    using Rage.Native;

    public class GetNearestPedToPosition : Service // stores nearest ped to position in blackboard(tree memory) in "key" 
    {
        [Serialization.DeserializeBehaviorConstructor]
        /// <param name="key">The key where the ped will be saved in the blackboard's tree memory.</param>
        public GetNearestPedToPosition(string key, int interval, Vector3 position, float range, bool considerDeadPeds, BehaviorTask child) : base(interval, (ref BehaviorTreeContext c) => { DoService(key, position, range, considerDeadPeds, ref c); }, child)
        {
        }

        [Serialization.DeserializeBehaviorConstructor]
        /// <param name="key">The key where the ped will be saved in the blackboard's tree memory.</param>
        public GetNearestPedToPosition(string key, Vector3 position, float range, bool considerDeadPeds, BehaviorTask child) : base((ref BehaviorTreeContext c) => { DoService(key, position, range, considerDeadPeds, ref c); }, child)
        {
        }

        private static void DoService(string key, Vector3 position, float range, bool considerDeadPeds, ref BehaviorTreeContext context)
        {
            Entity[] nearbyPeds = World.GetEntities(position, range, GetEntitiesFlags.ConsiderAllPeds);
            
            foreach (Entity e in nearbyPeds.OrderBy(e => Vector3.DistanceSquared(e.Position, position)))
            {
                Ped p = (Ped)e;
                if ((considerDeadPeds ? true : p.IsAlive))
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
        [Serialization.DeserializeBehaviorConstructor]
        /// <param name="key">The key where the ped will be saved in the blackboard's tree memory.</param>
        public GetNearestPedToAgent(string key, int interval, float range, bool considerDeadPeds, BehaviorTask child) : base(interval, (ref BehaviorTreeContext c) => { DoService(key, range, considerDeadPeds, ref c); }, child)
        {
        }

        [Serialization.DeserializeBehaviorConstructor]
        /// <param name="key">The key where the ped will be saved in the blackboard's tree memory.</param>
        public GetNearestPedToAgent(string key, float range, bool considerDeadPeds, BehaviorTask child) : base((ref BehaviorTreeContext c) => { DoService(key, range, considerDeadPeds, ref c); }, child)
        {
        }

        private static void DoService(string key, float range, bool considerDeadPeds, ref BehaviorTreeContext context)
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
                if ((considerDeadPeds ? true : p.IsAlive))
                {
                    if (context.Agent.Target is Ped && ((Ped)context.Agent.Target) != p)
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
