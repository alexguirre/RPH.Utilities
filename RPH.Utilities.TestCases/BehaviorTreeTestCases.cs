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
        //[TestCase]
        //public static void BehaviorTreeTest()
        //{
            //// https://outforafight.files.wordpress.com/2014/07/selector1.png

            //Sequence mainSequence = new Sequence
            //    (
            //        new ActionPerformer(() =>
            //        {
            //            Game.LogTrivial("   MainSequence -> Walk to Door");
            //            return MathHelper.Choose(BehaviorStatus.Success, BehaviorStatus.Running, BehaviorStatus.Running);
            //        }),
            //        new Selector(
            //            new ActionPerformer(() =>
            //            {
            //                Game.LogTrivial("   MainSequence -> Selector -> Open Door");
            //                return MathHelper.Choose(BehaviorStatus.Success, BehaviorStatus.Failure, BehaviorStatus.Running);
            //            }),
            //            new Sequence(
            //                new ActionPerformer(() =>
            //                {
            //                    Game.LogTrivial("   MainSequence -> Selector -> Sequence -> Unlock Door");
            //                    return MathHelper.Choose(BehaviorStatus.Success, BehaviorStatus.Failure, BehaviorStatus.Running);
            //                }),
            //                new ActionPerformer(() =>
            //                {
            //                    Game.LogTrivial("   MainSequence -> Selector -> Sequence -> Open Door");
            //                    return MathHelper.Choose(BehaviorStatus.Success, BehaviorStatus.Failure, BehaviorStatus.Running);
            //                })
            //            ),
            //            new ActionPerformer(() =>
            //            {
            //                Game.LogTrivial("   MainSequence -> Selector -> Smash Door");
            //                return MathHelper.Choose(BehaviorStatus.Success, BehaviorStatus.Failure, BehaviorStatus.Running);
            //            })
            //        ),
            //        new ActionPerformer(() =>
            //        {
            //            Game.LogTrivial("   MainSequence -> Walk through Door");
            //            return MathHelper.Choose(BehaviorStatus.Success, BehaviorStatus.Running);
            //        }),
            //        new ActionPerformer(() =>
            //        {
            //            Game.LogTrivial("   MainSequence -> Close Door");
            //            return MathHelper.Choose(BehaviorStatus.Success, BehaviorStatus.Running);
            //        })
            //    );

            //Selector root = new Selector(mainSequence);

            //BehaviorTree behavior = new BehaviorTree(root);

            //while (true)
            //{
            //    GameFiber.Yield();

            //    BehaviorStatus s = behavior.Perform();
            //    Game.LogTrivial("BehaviorState: " + s);
            //    if (s != BehaviorStatus.Running)
            //        break;
            //}
            //Game.DisplayNotification("Result: " + behavior.Status);
            //Game.LogTrivial("Result: " + behavior.Status);
        //}


        //[TestCase]
        //public static void BehaviorTreePlayerAndPedGoToPositionTasks()
        //{
        //    //GameFiber.Sleep(250);

        //    //Ped ped = new Ped("s_m_y_cop_01", Game.LocalPlayer.Character.GetOffsetPositionFront(20f), Game.LocalPlayer.Character.Heading);
        //    //ped.BlockPermanentEvents = true;
        //    //ped.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, -1, true);


        //    //BehaviorTask root = new Selector(
        //    //    new Sequence(
        //    //            new IsAnyPedNearAgent(5f, false),
        //    //            new ShootAtNearestPed(3000, FiringPattern.BurstFireRifle)
        //    //        ),
        //    //    new StatefulSequence(
        //    //            new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(-10f, 25f, 0f)), 10f, 2.0f),
        //    //            new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(-10f, 45f, 0f)), 10f, 2.0f),
        //    //            new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(10f, 45f, 0f)), 10f, 2.0f),
        //    //            new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(10f, 25f, 0f)), 10f, 2.0f)
        //    //        )
        //    //    );

        //    //BehaviorTree tree = new BehaviorTree(root);
            
        //    //BehaviorAgent pedAgent = new BehaviorAgent(ped);

        //    //while (true)
        //    //{
        //    //    GameFiber.Sleep(250);

        //    //    //tree.ExecuteOn(playerPedAgent);
        //    //    tree.ExecuteOn(pedAgent);
        //    //}
        //}

        [TestCase]
        public static void BehaviorTreeSerializationTest()
        {
            //BehaviorTask root = new Selector(
            //    new Sequence(
            //            new IsAnyPedNearAgent(5f, false),
            //            new ShootAtNearestPed(3000, FiringPattern.BurstFireRifle)
            //        ),
            //    new StatefulSequence(
            //            new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(-10f, 25f, 0f)), 10f, 2.0f),
            //            new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(-10f, 45f, 0f)), 10f, 2.0f),
            //            new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(10f, 45f, 0f)), 10f, 2.0f),
            //            new GoToPosition(Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(10f, 25f, 0f)), 10f, 2.0f)
            //        )
            //    );

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
                            new SerializableBehaviorLeaf()
                            {
                                TypeFullName = typeof(GetNearestPedToAgent).FullName,
                                Args = new object[] { 8.0f, false, "nearestPedToAgentForShootingTask" },
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
