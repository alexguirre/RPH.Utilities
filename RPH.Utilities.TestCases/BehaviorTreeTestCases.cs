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
    using RPH.Utilities.AI.Serialization;

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
                new GetNearestPedToAgent("nearestPedTarget", 100, 15.0f, false, 
                    new Sequence(
                        new EntityExists("nearestPedTarget",
                            new ShootAt(3000, FiringPattern.BurstFireRifle, "nearestPedTarget")
                        )
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

        [TestCase]
        public static void BehaviorTreeSerialization()
        {
            SerializableBehaviorTask root = new SerializableBehaviorComposite()
            {
                TypeFullName = typeof(Selector).FullName,
                Args = new object[0],
                Children = new SerializableBehaviorTask[]
                {
                    new SerializableBehaviorComposite()
                    {
                        TypeFullName = typeof(Sequence).FullName,
                        Args = new object[0],
                        Children = new SerializableBehaviorTask[]
                        {
                            new SerializableBehaviorDecorator()
                            {
                                TypeFullName = typeof(GetNearestPedToAgent).FullName,
                                Args = new object[] { "nearestPedToAgentForShootingTask", 15.0f, false },
                                Child = new SerializableBehaviorLeaf()
                                        {
                                            TypeFullName = typeof(EntityExists).FullName,
                                            Args = new object[] { "nearestPedToAgentForShootingTask" },
                                        }
                            },
                            new SerializableBehaviorLeaf()
                            {
                                TypeFullName = typeof(ShootAt).FullName,
                                Args = new object[] { 2000, FiringPattern.BurstFireRifle, "nearestPedToAgentForShootingTask" },
                            }
                        },
                    },

                    new SerializableBehaviorComposite()
                    {
                        TypeFullName = typeof(StatefulSequence).FullName,
                        Args = new object[0],
                        Children = new SerializableBehaviorTask[]
                        {
                            new SerializableBehaviorLeaf()
                            {
                                TypeFullName = typeof(GoToPosition).FullName,
                                Args = new object[] { new Vector3(1549.564f, 3204.138f, 40.41154f), 10f, 2.5f },
                            },
                            new SerializableBehaviorLeaf()
                            {
                                TypeFullName = typeof(GoToPosition).FullName,
                                Args = new object[] { new Vector3(1531.945f, 3219.676f, 40.51015f), 10f, 2.5f },
                            },
                            new SerializableBehaviorLeaf()
                            {
                                TypeFullName = typeof(GoToPosition).FullName,
                                Args = new object[] { new Vector3(1571.225f, 3225.011f, 40.45512f), 10f, 2.5f },
                            },
                        },
                    },
                },
            };

            SerializableBehaviorTree tree = new SerializableBehaviorTree()
            {
                Root = root,
            };

            string str = null;
            using (StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SerializableBehaviorTree), new[] { typeof(SerializableBehaviorTask), typeof(SerializableBehaviorComposite), typeof(SerializableBehaviorDecorator), typeof(SerializableBehaviorLeaf), typeof(Vector3), typeof(FiringPattern) });
                serializer.Serialize(writer, tree);
                str = writer.ToString();
            }

            File.WriteAllText("behavior.btree", str);

            SerializableBehaviorTree desTree = null;
            using (StringReader reader = new StringReader(str))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SerializableBehaviorTree), new[] { typeof(SerializableBehaviorTask), typeof(SerializableBehaviorComposite), typeof(SerializableBehaviorDecorator), typeof(SerializableBehaviorLeaf), typeof(Vector3), typeof(FiringPattern) });
                desTree = (SerializableBehaviorTree)serializer.Deserialize(reader);
            }

            BehaviorTree finalTree = desTree.CreateTree();

            Ped ped = new Ped("s_m_y_cop_01", Game.LocalPlayer.Character.GetOffsetPositionFront(20f), Game.LocalPlayer.Character.Heading);
            ped.BlockPermanentEvents = true;
            ped.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);

            BehaviorAgent pedAgent = new BehaviorAgent(ped);

            while (true)
            {
                GameFiber.Sleep(250);

                finalTree.ExecuteOn(pedAgent);
            }
        }
    }
}
