namespace RPH.Utilities.AI
{
    // System
    using System;
    using System.Collections.Generic;

    // RPH
    using Rage;

    // Util
    using RPH.Utilities.AI.Composites;
    using RPH.Utilities.AI.Decorators;
    using RPH.Utilities.AI.Leafs;

    public enum BehaviorStatus
    {
        Failure, 
        Success,
        Running,
    }

    public class BehaviorTree
    {
        public Guid Id { get; }

        public BehaviorTask Root { get; }
        public BehaviorStatus Status { get; private set; }

        public BehaviorTree(BehaviorTask root)
        {
            Id = Guid.NewGuid();
            Root = root;
        }

        public BehaviorStatus ExecuteOn(BehaviorAgent agent)
        {
            try
            {
                BehaviorTreeContext context = new BehaviorTreeContext
                {
                    Agent = agent,
                    Tree = this,
                    OpenNodes = new List<Guid>()
                };

                Status = Root.Behave(ref context);

                List<Guid> previousOpenNodes = agent.Blackboard.Get<List<Guid>>("openNodes", Id);
                List<Guid> currentOpenNodes = context.OpenNodes;

                int start = 0;
                for (int i = 0; i < Math.Min(previousOpenNodes.Count, currentOpenNodes.Count); i++)
                {
                    start = i + 1;

                    if (previousOpenNodes[i] != currentOpenNodes[i])
                    {
                        break;
                    }
                }

                for (int i = previousOpenNodes.Count - 1; i >= start; i--)
                {
                    Root.GetTaskById(previousOpenNodes[i])?.CloseInternal(ref context);
                }

                agent.Blackboard.Set<List<Guid>>("openNodes", currentOpenNodes, Id);
                agent.Blackboard.Set<int>("nodeCount", context.NodeCount, Id);
            }
            catch (System.Exception ex)
            {
                Game.LogTrivial($"[{this.GetType().Name}] Exception thrown when performing behavior tree: {ex}");
                Status = BehaviorStatus.Failure;
            }

            return Status;
        }
    }

    public struct BehaviorTreeContext
    {
        public BehaviorAgent Agent { get; set; }
        public BehaviorTree Tree { get; set; }
        public List<Guid> OpenNodes { get; set; }
        public int NodeCount { get; set; }

        public void OnEnterNode(Guid node) 
        {
            NodeCount++;
            OpenNodes.Add(node);
        }

        public void OnOpenNode(Guid node)
        {
        }

        public void OnBehaveNode(Guid node)
        {
        }

        public void OnCloseNode(Guid node)
        {
            OpenNodes.Remove(node);
        }

        public void OnExitNode(Guid node)
        {
        }
    }
}
