namespace RPH.Utilities.AI.Leafs
{
    // System
    using System;

    // RPH
    using Rage;

    public abstract class Condition : BehaviorLeaf
    {
        protected abstract bool CheckCondition(ref BehaviorTreeContext context);

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
    
    public class DelegatedCondition : Condition
    {
        public delegate bool ConditionDelegate();
        public delegate bool ConditionDelegateWithContext(ref BehaviorTreeContext context);

        private readonly ConditionDelegate condition;
        private readonly ConditionDelegateWithContext conditionWithContext;

        public DelegatedCondition(ConditionDelegate condition)
        {
            this.condition = condition;
        }

        public DelegatedCondition(ConditionDelegateWithContext conditionWithContext)
        {
            this.conditionWithContext = conditionWithContext;
        }

        protected override bool CheckCondition(ref BehaviorTreeContext context)
        {
            if (condition != null)
            {
                return condition.Invoke();
            }

            if (conditionWithContext != null)
            {
                return conditionWithContext.Invoke(ref context);
            }

            return false;
        }
    }
}
