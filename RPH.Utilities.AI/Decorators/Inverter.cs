namespace RPH.Utilities.AI.Decorators
{
    // RPH
    using Rage;

    /// <summary>
    /// Returns the inverted state of the specified <see cref="BehaviorTask"/>.
    /// <para>
    /// <see cref="BehaviorStatus.Failure"/> is converted to <see cref="BehaviorStatus.Success"/>.
    /// </para>
    /// <para>
    /// <see cref="BehaviorStatus.Success"/> is converted to <see cref="BehaviorStatus.Failure"/>.
    /// </para>
    /// <para>
    /// <see cref="BehaviorStatus.Running"/> is kept the same.
    /// </para>
    /// <para>
    /// In case of an exception thrown it returns <see cref="BehaviorStatus.Success"/>.
    /// </para>
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.Decorators.BehaviorDecorator" />
    public class Inverter : BehaviorDecorator
    {
        public Inverter(BehaviorTask child) : base(child)
        {
        }

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            try
            {
                switch (Child.Behave(ref context))
                {
                    case BehaviorStatus.Failure: return BehaviorStatus.Success;
                    case BehaviorStatus.Success: return BehaviorStatus.Failure;
                    case BehaviorStatus.Running: return BehaviorStatus.Running;
                    default: return BehaviorStatus.Success;
                }
            }
            catch (System.Exception ex)
            {
                Game.LogTrivial($"[{this.GetType().Name}] Exception thrown at OnBehave(): {ex}");

                return BehaviorStatus.Success;
            }
        }
    }
}
