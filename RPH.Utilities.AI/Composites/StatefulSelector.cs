namespace RPH.Utilities.AI.Composites
{
    // RPH
    using Rage;

    /// <summary>
    /// Executes all behaviors until one returns <see cref="BehaviorStatus.Success"/> or all have failed.
    /// <para>
    /// - <see cref="BehaviorStatus.Failure"/>: if all <see cref="BehaviorTask"/>s fail.
    /// </para>
    /// <para>
    /// - <see cref="BehaviorStatus.Success"/>: if a <see cref="BehaviorTask"/> is successful.
    /// </para>
    /// <para>
    /// - <see cref="BehaviorStatus.Running"/>: if any <see cref="BehaviorTask"/> is running.
    /// </para>
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.Composites.BehaviorComposite" />
    public class StatefulSelector : BehaviorComposite
    {
        public StatefulSelector(params BehaviorTask[] children) : base(children)
        {
        }

        protected override void OnOpen(ref BehaviorTreeContext context)
        {
            context.Agent.Blackboard.Set("runningChildIndex", 0, context.Tree.Id, this.Id);
        }

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            try
            {
                if (Children == null || Children.Length == 0)
                    return BehaviorStatus.Success;

                int runningChildIndex = context.Agent.Blackboard.Get("runningChildIndex", context.Tree.Id, this.Id, 0);

                for (int i = runningChildIndex; i < Children.Length; i++)
                {
                    BehaviorStatus status = Children[i].Behave(ref context);

                    switch (status)
                    {
                        case BehaviorStatus.Success: return BehaviorStatus.Success;
                        case BehaviorStatus.Running:
                            {
                                context.Agent.Blackboard.Set("runningChildIndex", i, context.Tree.Id, this.Id);
                                return BehaviorStatus.Running;
                            }
                    }
                }

                return BehaviorStatus.Failure;
            }
            catch (System.Exception ex)
            {
                Game.LogTrivial($"[{this.GetType().Name}] Exception thrown at OnBehave(): {ex}");

                return BehaviorStatus.Failure;
            }
        }
    }
}
