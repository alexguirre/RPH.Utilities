namespace RPH.Utilities.AI.Composites
{
    // RPH
    using Rage;

    /// <summary>
    /// Attemps to execute all behaviors in one cycle.
    /// <para>
    /// - <see cref="BehaviorStatus.Failure"/>: if one <see cref="BehaviorTask"/> fails or a exception is thrown.
    /// </para>
    /// <para>
    /// - <see cref="BehaviorStatus.Success"/>: if all <see cref="BehaviorTask"/> are successful.
    /// </para>
    /// <para>
    /// - <see cref="BehaviorStatus.Running"/>: if any <see cref="BehaviorTask"/> is running.
    /// </para>
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.Composites.BehaviorComposite" />
    public class Sequence : BehaviorComposite
    {
        public Sequence(params BehaviorTask[] children) : base(children)
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

                    if (status != BehaviorStatus.Success)
                    {
                        return status;
                    }
                }

                return BehaviorStatus.Success;
            }
            catch (System.Exception ex)
            {
                Game.LogTrivial($"[{this.GetType().Name}] Exception thrown at OnBehave(): {ex}");

                return BehaviorStatus.Failure;
            }
        }
    }
}
