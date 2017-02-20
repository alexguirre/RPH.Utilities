namespace RPH.Utilities.AI.Decorators
{
    // RPH
    using Rage;

    /// <summary>
    /// Returns <see cref="BehaviorStatus.Failure"/> always.
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.Decorators.BehaviorDecorator" />
    public class Failer : BehaviorDecorator
    {
        [Serialization.DeserializeBehaviorConstructor]
        public Failer(BehaviorTask child) : base(child)
        {
        }

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            try
            {
                Child.Behave(ref context);

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
