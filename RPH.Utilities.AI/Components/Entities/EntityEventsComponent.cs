namespace RPH.Utilities.AI.Components.Entities
{
    // System
    using System;

    // RPH
    using Rage;

    public class EntityEventsComponent : Component
    {
        public delegate void EntityEventHandler(AdvancedEntity sender);

        public AdvancedEntity ParentEntity { get; private set; }

        public event EntityEventHandler Died;
        public event EntityEventHandler Resurrected;

        public override void OnStart()
        {
            if (!(Parent is AdvancedEntity))
            {
                throw new InvalidOperationException($"Can't add a {nameof(EntityEventsComponent)} to a {nameof(ComplexObject)} that doesn't inherit from {nameof(AdvancedEntity)}.");
            }

            ParentEntity = Parent as AdvancedEntity;
        }


        public override void OnContinuousUpdate()
        {
            if ((Game.GameTime - lastUpdateGameTime) > 100)
            {
                CheckDiedAndResurrectedEvents();

                lastUpdateGameTime = Game.GameTime;
            }
        }

        bool previouslyDead;
        private void CheckDiedAndResurrectedEvents()
        {
            bool currentlyDead = ParentEntity.Entity.IsDead;
            if (previouslyDead != currentlyDead)
            {
                previouslyDead = currentlyDead;
                
                if (currentlyDead)
                {
                    Died?.Invoke(ParentEntity);
                }
                else
                {
                    Resurrected?.Invoke(ParentEntity);
                }
            }
        }

        uint lastUpdateGameTime;
    }
}
