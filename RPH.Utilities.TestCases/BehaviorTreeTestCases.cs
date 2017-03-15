namespace RPH.Utilities.TestCases
{
    // System
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    // Util
    using RPH.Utilities.Tests;
    using RPH.Utilities.AI;
    using RPH.Utilities.AI.Composites;
    using RPH.Utilities.AI.Decorators;
    using RPH.Utilities.AI.Leafs;

    // RPH
    using Rage;

    public static class BehaviorTreeTestCases
    {
        [TestCase]
        public static void BehaviorTreePedTasks()
        {
            GameFiber.Sleep(250);

            Ped ped = new Ped("s_m_y_cop_01", Game.LocalPlayer.Character.GetOffsetPositionFront(20f), Game.LocalPlayer.Character.Heading);
            ped.BlockPermanentEvents = true;
            ped.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);


            BehaviorTask root = new Selector(
                new GetNearestPedToAgent(new BlackboardSetter<Ped>("nearestPedTarget", BlackboardMemoryScope.Tree), 100, 15.0f, (p) => p.IsAlive, 
                    new Sequence(
                        new EntityExists(new BlackboardGetter<Entity>("nearestPedTarget", BlackboardMemoryScope.Tree)),
                        new ShootAt(3000, FiringPattern.BurstFireRifle, new BlackboardGetter<Entity>("nearestPedTarget", BlackboardMemoryScope.Tree))
                    )
                ),
                new StatefulSequence(
                        new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(-10f, 25f, 0f)), 10f, 2.0f),
                        new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(-10f, 45f, 0f)), 10f, 2.0f),
                        new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(10f, 45f, 0f)), 10f, 2.0f),
                        new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(10f, 25f, 0f)), 10f, 2.0f)
                    )
                );

            BehaviorTree tree = new BehaviorTree(root);

            BehaviorAgent pedAgent = new BehaviorAgent(ped);

            while (true)
            {
                GameFiber.Sleep(250);
                
                tree.ExecuteOn(pedAgent);
            }
        }
    }
}
