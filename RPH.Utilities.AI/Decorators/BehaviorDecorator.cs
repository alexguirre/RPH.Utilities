using System;

namespace RPH.Utilities.AI.Decorators
{
    /// <summary>
    /// Behavior that can have a single child.
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.Decorators.BehaviorDecorator" />
    public abstract class BehaviorDecorator : BehaviorTask
    {
        protected BehaviorTask Child;

        [Serialization.DeserializeBehaviorConstructor]
        public BehaviorDecorator(BehaviorTask child)
        {
            Child = child;
        }

        public override BehaviorTask GetTaskById(Guid id)
        {
            if (Id == id)
                return this;

            if (Child.Id == id)
            {
                return Child;
            }
            else
            {
                BehaviorTask t = Child.GetTaskById(id);
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
    }
}
