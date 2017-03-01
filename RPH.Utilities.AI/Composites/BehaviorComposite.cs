namespace RPH.Utilities.AI.Composites
{
    // System
    using System;

    /// <summary>
    /// Behavior that can have multiple childs.
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.BehaviorTask" />
    public abstract class BehaviorComposite : BehaviorTask
    {
        protected BehaviorTask[] Children { get; }

        protected BehaviorComposite(params BehaviorTask[] children)
        {
            Children = children;
        }

        public override BehaviorTask GetTaskById(Guid id)
        {
            if (Id == id)
                return this;

            for (int i = 0; i < Children.Length; i++)
            {
                BehaviorTask child = Children[i];

                if (child.Id == id)
                {
                    return child;
                }
                else
                {
                    BehaviorTask t = child.GetTaskById(id);
                    if (t != null)
                    {
                        return t;
                    }
                }
            }

            return null;
        }
    }
}
