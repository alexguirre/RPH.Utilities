namespace RPH.Utilities.AI.Decorators
{
    // System
    using System;

    // RPH
    using Rage;

    public abstract class Condition : BehaviorDecorator
    {
        public Condition(BehaviorTask child) : base(child)
        {
        }

        protected abstract bool CheckCondition(ref BehaviorTreeContext context);

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            try
            {
                if (CheckCondition(ref context))
                {
                    return Child.Behave(ref context);
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

        ConditionDelegate condition;
        ConditionDelegateWithContext conditionWithContext;

        public DelegatedCondition(ConditionDelegate condition, BehaviorTask child) : base(child)
        {
            this.condition = condition;
        }

        public DelegatedCondition(ConditionDelegateWithContext conditionWithContext, BehaviorTask child) : base(child)
        {
            this.conditionWithContext = conditionWithContext;
        }

        protected override bool CheckCondition(ref BehaviorTreeContext context)
        {
            if (condition != null)
            {
                return condition.Invoke();
            }
            else if(conditionWithContext != null)
            {
                return conditionWithContext.Invoke(ref context);
            }

            return false;
        }
    }
}
