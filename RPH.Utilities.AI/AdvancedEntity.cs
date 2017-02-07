namespace RPH.Utilities.AI
{
    // RPH
    using Rage;

    public class AdvancedEntity : ComplexWorldObject
    {
        public Entity Entity { get; }

        public override Vector3 Position
        {
            get
            {
                return Entity ? Entity.Position : Vector3.Zero; ;
            }
            set
            {
                if (Entity)
                {
                    Entity.Position = value;
                }
            }
        }

        public override Rotator Rotation
        {
            get
            {
                return Entity ? Entity.Rotation : Rotator.Zero;
            }
            set
            {
                if (Entity)
                {
                    Entity.Rotation = value;
                }
            }
        }

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
