namespace RPH.Utilities.TestCases
{
    // Util
    using RPH.Utilities.Tests;
    using RPH.Utilities.AI;
    using RPH.Utilities.AI.Components;
    using RPH.Utilities.AI.Components.Entities;

    // RPH
    using Rage;

    public static class ComponentsTestCases
    {
        [TestCase]
        public static void EntityEventsComponentTest()
        {
            AdvancedPed ped = new AdvancedPed(Game.LocalPlayer.Character.GetOffsetPositionFront(5f));

            Assert.AreEqual(true, ped.Entity.Exists(), "AdvancedPed.Entity doesn't exist.");
            Assert.AreEqual(true, ped.Ped.Exists(), "AdvancedPed.Ped doesn't exist.");

            Assert.AreEqual(true, ped.Position == ped.Ped.Position, "AdvancedPed.Position isn't equal to Ped.Position");
            Assert.AreEqual(true, ped.Rotation == ped.Ped.Rotation, "AdvancedPed.Rotation isn't equal to Ped.Rotation");

            EntityEventsComponent component = ped.AddComponent<EntityEventsComponent>();
            Assert.AreNotEqual(true, component == null, "EntityEventsComponent instance returned from ComplexObject.AddComponent is null.");

            bool hasComponent = ped.HasComponent<EntityEventsComponent>();
            Assert.AreEqual(true, hasComponent, "ComplexObject.HasComponent returned false for EntityEventsComponent.");

            component = ped.GetComponent<EntityEventsComponent>();
            Assert.AreNotEqual(true, component == null, "EntityEventsComponent instance returned from ComplexObject.GetComponent is null.");

            Assert.AreEqual(true, component.Parent == ped, "EntityEventsComponent.Parent doesn't equal the AdvancedPed instance.");
            Assert.AreEqual(true, component.ParentEntity == ped, "EntityEventsComponent.ParentEntity doesn't equal the AdvancedPed instance.");

            bool wasDiedCalled = false;
            component.Died += (s) => { wasDiedCalled = true; };

            ped.Ped.Kill();
            GameFiber.Wait(200);
            Assert.AreEqual(true, wasDiedCalled, "EntityEventsComponent.Died wasn't raised after Ped.Kill call.");

            bool wasResurrectedCalled = false;
            component.Resurrected += (s) => { wasResurrectedCalled = true; };

            ped.Ped.Resurrect();
            GameFiber.Wait(200);

            Assert.AreEqual(true, wasResurrectedCalled, "EntityEventsComponent.Resurrected wasn't raised after Ped.Resurrect call.");

            bool wasDestroyedCalled = false;
            ped.Destroyed += (s) => { wasDestroyedCalled = true; };

            ped.Destroy();

            Assert.AreEqual(false, ped.Entity.Exists(), "AdvancedPed.Entity still exists after ComplexObject.Destroy call.");
            Assert.AreEqual(false, ped.Ped.Exists(), "AdvancedPed.Ped still exists after ComplexObject.Destroy call.");
            Assert.AreEqual(true, ped.IsDestroyed, "ComplexObject.IsDestroyed isn't true after ComplexObject.Destroy call.");
            Assert.AreEqual(true, wasDestroyedCalled, "ComplexObject.Destroyed wasn't raised at ComplexObject.Destroy call.");
        }
    }
}
