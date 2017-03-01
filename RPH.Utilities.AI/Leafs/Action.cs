namespace RPH.Utilities.AI.Leafs
{
    // RPH
    using Rage;

    /// <summary>
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.Leafs.BehaviorLeaf" />
    public abstract class Action : BehaviorLeaf
    {
    }

    /// <summary>
    /// Performs the given action.
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.Leafs.Action" />
    public class DelegatedAction : Action
    {
        public delegate void ActionDelegate();
        public delegate void ActionDelegateWithContext(ref BehaviorTreeContext context);

        private readonly ActionDelegate action;
        private readonly ActionDelegateWithContext actionWithContext;

        public DelegatedAction(ActionDelegate action)
        {
            this.action = action;
        }

        public DelegatedAction(ActionDelegateWithContext actionWithContext)
        {
            this.actionWithContext = actionWithContext;
        }

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            try
            {
                if (action != null)
                {
                    action.Invoke();
                }
                else
                {
                    actionWithContext?.Invoke(ref context);
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
