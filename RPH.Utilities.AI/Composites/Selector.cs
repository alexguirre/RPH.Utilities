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

    ///// <summary>
    ///// Executes all behaviors until one returns <see cref="BehaviorStatus.Success"/> or all have failed.
    ///// <para>
    ///// - <see cref="BehaviorStatus.Failure"/>: if all <see cref="BehaviorTask"/>s fail.
    ///// </para>
    ///// <para>
    ///// - <see cref="BehaviorStatus.Success"/>: if a <see cref="BehaviorTask"/> is successful.
    ///// </para>
    ///// <para>
    ///// - <see cref="BehaviorStatus.Running"/>: if any <see cref="BehaviorTask"/> is running.
    ///// </para>
    ///// </summary>
    ///// <seealso cref="RPH.Utilities.AI.Composites.BehaviorComposite" />
    //public class Selector : BehaviorComposite
    //{
    //    protected BehaviorTask RunningChild;
    //    protected int RunningChildIndex;

    //    public Selector(params BehaviorTask[] behaviors) : base(behaviors)
    //    {
    //    }


    //    protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
    //    {
    //        try
    //        {
    //            if (Children == null || Children.Length == 0)
    //                return BehaviorStatus.Success;

    //            if (RunningChild == null)
    //            {
    //                RunningChildIndex = MathHelper.Min(RunningChildIndex, Children.Length - 1);
    //                RunningChild = Children[RunningChildIndex];
    //            }

    //            BehaviorStatus childState = RunningChild.Behave(ref context);

    //            switch (childState)
    //            {
    //                case BehaviorStatus.Running: return BehaviorStatus.Running;
    //                case BehaviorStatus.Success: return BehaviorStatus.Success;
    //                default:
    //                case BehaviorStatus.Failure:
    //                    {
    //                        RunningChildIndex++;
    //                        if (RunningChildIndex >= Children.Length) // all childs failed, return Failure
    //                        {
    //                            return BehaviorStatus.Failure;
    //                        }
    //                        else
    //                        {
    //                            // current child returned Failure, get next child
    //                            RunningChild = Children[RunningChildIndex];
    //                            return BehaviorStatus.Running;
    //                        }
    //                    }
    //            }

    //        }
    //        catch (System.Exception ex)
    //        {
    //            Game.LogTrivial($"[{this.GetType().Name}] Exception thrown at OnBehave(): {ex}");

    //            return BehaviorStatus.Failure;
    //        }
    //    }
    //}
}
