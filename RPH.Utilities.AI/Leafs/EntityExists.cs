namespace RPH.Utilities.AI.Leafs
{
    // RPH
    using Rage;

    public class EntityExists : Condition
    {
        private readonly BlackboardGetter<Entity> entity;

        /// <param name="entity">Where to get the <see cref="Rage.Entity"/> from the blackboard memory.</param>
        [Serialization.DeserializeBehaviorConstructor]
        public EntityExists(BlackboardGetter<Entity> entity)
        {
            this.entity = entity;
        }

        protected override bool CheckCondition(ref BehaviorTreeContext context)
        {
            Entity ent = entity.Get(context, this);

            return ent.Exists();
        }
    }
}
