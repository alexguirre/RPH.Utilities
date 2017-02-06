namespace RPH.Utilities.AI
{
    // RPH
    using Rage;

    public class AdvancedEntity : ComplexObject
    {
        public Entity Entity { get; }

        public AdvancedEntity(Entity entity) : base()
        {
            Entity = entity;
        }

        protected override void OnUpdate()
        {
            if (Entity)
            {
                OnSubUpdate();
            }
            else
            {
                CanUpdate = false;
                Destroy();
            }
        }

        protected virtual void OnSubUpdate()
        {
        }

        protected override void OnDestroy()
        {
            if (Entity)
            {
                Entity.Delete();
            }
        }
    }
}
