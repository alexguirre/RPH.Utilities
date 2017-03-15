namespace RPH.Utilities.AI.Decorators
{
    // RPH
    using Rage;

    /// <summary>
    /// Returns <see cref="BehaviorStatus.Success"/> always.
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.Decorators.BehaviorDecorator" />
    public class Succeeder : BehaviorDecorator
    {
        public Succeeder(BehaviorTask child) : base(child)
        {
        }

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            try
            {
                Child.Behave(ref context);

                return BehaviorStatus.Success;

            }
            catch (System.Exception ex)
            {
                Game.LogTrivial($"[{this.GetType().Name}] Exception thrown at OnBehave(): {ex}");

                return BehaviorStatus.Success;
            }
        }
    }
}
