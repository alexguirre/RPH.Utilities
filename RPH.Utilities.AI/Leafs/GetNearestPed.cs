namespace RPH.Utilities.AI.Leafs
{
    // System
    using System;
    using System.Linq;

    // RPH
    using Rage;
    using Rage.Native;

    public class GetNearestPedToPosition : Condition // stores nearest ped to blackboard(tree memory) in blackboardKey 
    {
        Vector3 position;
        float range;
        bool considerDeadPeds;
        string blackboardKey;

        [Serialization.DeserializeBehaviorConstructor]
        public GetNearestPedToPosition(Vector3 position, float range, bool considerDeadPeds, string blackboardKey = "nearestPedToPosition"/*the key where the nearest ped will be stored*/)
        {
            this.position = position;
            this.range = range;
            this.considerDeadPeds = considerDeadPeds;
            this.blackboardKey = blackboardKey;
        }

        public override bool CheckCondition(ref BehaviorTreeContext context)
        {
            Entity[] nearbyPeds = World.GetEntities(position, range, GetEntitiesFlags.ConsiderAllPeds);

            Ped result;
            foreach (Entity e in nearbyPeds.OrderBy(e => Vector3.DistanceSquared(e.Position, position)))
            {
                Ped p = (Ped)e;
                if((considerDeadPeds ? p.IsAlive : true))
                {
                    result = p;
                    context.Agent.Blackboard.Set<Ped>(blackboardKey, result, context.Tree.Id);
                    return true;
                }
            }

            context.Agent.Blackboard.Set<Ped>(blackboardKey, null, context.Tree.Id);
            return false;
        }
    }

    public class GetNearestPedToAgent : Condition // stores nearest ped to blackboard(tree memory) in blackboardKey
    {
        float range;
        bool considerDeadPeds;
        string blackboardKey;

        [Serialization.DeserializeBehaviorConstructor]
        public GetNearestPedToAgent(float range, bool considerDeadPeds, string blackboardKey = "nearestPedToAgent"/*the key where the nearest ped will be stored*/)
        {
            this.range = range;
            this.considerDeadPeds = considerDeadPeds;
            this.blackboardKey = blackboardKey;
        }

        public override bool CheckCondition(ref BehaviorTreeContext context)
        {
            if (!(context.Agent.Target is ISpatial))
            {
                throw new InvalidOperationException($"The behavior condition {nameof(GetNearestPedToAgent)} can't be used with {context.Agent.Target.GetType().Name}, it can only be used with {nameof(ISpatial)}s");
            }

            Vector3 position = ((ISpatial)context.Agent.Target).Position;

            Entity[] nearbyPeds = World.GetEntities(position, range, GetEntitiesFlags.ConsiderAllPeds);

            Ped result;
            foreach (Entity e in nearbyPeds.OrderBy(e => Vector3.DistanceSquared(e.Position, position)))
            {
                Ped p = (Ped)e;
                if ((considerDeadPeds ? p.IsAlive : true))
                {
                    if (context.Agent.Target is Ped && ((Ped)context.Agent.Target) != p)
                    {
                        result = p;
                        context.Agent.Blackboard.Set<Ped>(blackboardKey, result, context.Tree.Id);
                        return true;
                    }
                }
            }

            context.Agent.Blackboard.Set<Ped>(blackboardKey, null, context.Tree.Id);
            return false;
        }
    }
}
