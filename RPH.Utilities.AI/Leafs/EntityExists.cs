namespace RPH.Utilities.AI.Leafs
{
    // RPH
    using Rage;

    public class EntityExists : Condition
    {
        string entityKey;

        [Serialization.DeserializeBehaviorConstructor]
        /// <param name="entityKey">The key where the entity is saved in the blackboard's tree memory.</param>
        public EntityExists(string entityKey)
        {
            this.entityKey = entityKey;
        }

        protected override bool CheckCondition(ref BehaviorTreeContext context)
        {
            Entity ent = context.Agent.Blackboard.Get<Entity>(entityKey, context.Tree.Id);

            return ent.Exists();
        }
    }
}
