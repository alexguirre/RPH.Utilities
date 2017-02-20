namespace RPH.Utilities.AI.Leafs
{
    // System
    using System;

    // RPH
    using Rage;

    public abstract class Condition : BehaviorLeaf
    {
        public Condition()
        {
        }

        public abstract bool CheckCondition(ref BehaviorTreeContext context);

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            try
            {
                if (CheckCondition(ref context))
                {
                    return BehaviorStatus.Success;
                }
                else
                {
                    return BehaviorStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"[{this.GetType().Name}] Exception thrown at OnBehave(): {ex}");

                return BehaviorStatus.Failure;
            }
        }
    }
}
