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
    public class Selector : BehaviorComposite
    {
        [Serialization.DeserializeBehaviorConstructor]
        public Selector(params BehaviorTask[] children) : base(children)
        {
        }


        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            try
            {
                if (Children == null || Children.Length == 0)
                    return BehaviorStatus.Success;

                for (int i = 0; i < Children.Length; i++)
                {
                    BehaviorStatus status = Children[i].Behave(ref context);

                    if (status != BehaviorStatus.Failure)
                    {
                        return status;
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
