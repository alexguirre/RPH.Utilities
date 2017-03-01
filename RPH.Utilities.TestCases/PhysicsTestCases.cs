namespace RPH.Utilities.TestCases
{
    // System
    using System.IO;
    using System.Linq;
    using System.Diagnostics;
    using System.Windows.Forms;
    using System.Collections.Generic;

    // Util
    using RPH.Utilities.Tests;
    using RPH.Utilities.Physics;
    using RPH.Utilities.Physics.Ropes;

    // RPH
    using Rage;

    public static class PhysicsTestCases
    {
        [TestCase]
        public static void PhysicsSpring()
        {
            PhysicsManager mgr = new PhysicsManager();

            Particle a = new Particle(1.0f) { IsAffectedByAirFriction = true };
            Particle b = new Particle(2.0f) { IsAffectedByAirFriction = true };

            SpringJoint spring = new SpringJoint(a, b, 1.0f, 100.0f, 0.15f);


            mgr.PhysicsObjects.Add(a);
            mgr.PhysicsObjects.Add(b);
            mgr.PhysicsObjects.Add(spring);

            a.Position = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 2.0f, 1.0f));
            b.Position = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 3.25f, 1.0f));

            Vector3 positionChange = new Vector3();
            mgr.AfterSolve += () =>
            {
                if (positionChange != Vector3.Zero)
                {
                    a.ApplyForce(positionChange);
                }
            };

            while (true)
            {
                GameFiber.Yield();

                positionChange = new Vector3();

                const float moveValue = 25f;
                if (Game.IsKeyDownRightNow(Keys.NumPad6))
                    positionChange.X += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad4))
                    positionChange.X -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad8))
                    positionChange.Y -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad2))
                    positionChange.Y += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad9))
                    positionChange.Z += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad3))
                    positionChange.Z -= moveValue;


                mgr.Update();
            }
        }

        [TestCase]
        public static void PhysicsString()
        {
            PhysicsManager mgr = new PhysicsManager();

            Particle a = new Particle(1.0f) { IsAffectedByAirFriction = true };
            Particle b = new Particle(2.0f) { IsAffectedByAirFriction = true };

            StringJoint stringJ = new StringJoint(a, b, 1.0f, 100.0f, 0.15f);


            mgr.PhysicsObjects.Add(a);
            mgr.PhysicsObjects.Add(b);
            mgr.PhysicsObjects.Add(stringJ);

            a.Position = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 2.0f, 1.0f));
            b.Position = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 3.25f, 1.0f));

            Vector3 positionChange = new Vector3();
            mgr.AfterSolve += () =>
            {
                if (positionChange != Vector3.Zero)
                {
                    a.ApplyForce(positionChange);
                }
            };

            while (true)
            {
                GameFiber.Yield();

                positionChange = new Vector3();

                const float moveValue = 25f;
                if (Game.IsKeyDownRightNow(Keys.NumPad6))
                    positionChange.X += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad4))
                    positionChange.X -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad8))
                    positionChange.Y -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad2))
                    positionChange.Y += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad9))
                    positionChange.Z += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad3))
                    positionChange.Z -= moveValue;


                mgr.Update();
            }
        }

        [TestCase]
        public static void PhysicsMultipleString()
        {
            const int particlesCount = 10;

            PhysicsManager mgr = new PhysicsManager();

            Vector3 firstParticlePos = Game.LocalPlayer.Character.GetOffsetPositionFront(0.225f);

            List<Particle> particles = new List<Particle>(particlesCount);
            for (int i = 0; i < particlesCount; i++)
            {
                Particle p = new Particle(1.0f) { IsAffectedByAirFriction = true, IsAffectedByGravity = i != 0 };

                if (i == 0)
                    p.Pin(firstParticlePos);
                else
                    p.Position = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 0.225f, i * -0.6f));

                particles.Add(p);
            }

            List<StringJoint> springs = new List<StringJoint>(particlesCount - 1);
            for (int i = 0; i < particlesCount - 1; i++)
            {
                StringJoint spring = new StringJoint(particles[i], particles[i + 1], 0.625f, 10.0f, 0.85f);
                springs.Add(spring);
            }


            mgr.PhysicsObjects.AddRange(particles);
            mgr.PhysicsObjects.AddRange(springs);
            
            Vector3 positionChange = new Vector3();
            mgr.AfterSimulate += (deltaTime) =>
            {
                if (positionChange != Vector3.Zero)
                {
                    Vector3 v = (positionChange / particles[0].Mass) * deltaTime;

                    particles[0].Pin(particles[0].Position + v * deltaTime);
                }
            };

            bool firstUpdate = true;
            while (true)
            {
                GameFiber.Yield();

                positionChange = new Vector3();

                const float moveValue = 25f;
                if (Game.IsKeyDownRightNow(Keys.NumPad6))
                    positionChange.X += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad4))
                    positionChange.X -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad8))
                    positionChange.Y -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad2))
                    positionChange.Y += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad9))
                    positionChange.Z += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad3))
                    positionChange.Z -= moveValue;


                mgr.Update();

                if (firstUpdate)
                {
                    for (int i = 0; i < particlesCount; i++)
                    {
                        particles[i].Velocity = Vector3.Zero;
                    }

                    firstUpdate = false;
                }
            }
        }

        [TestCase]
        public static void PhysicsRope()
        {
            PhysicsManager mgr = new PhysicsManager();

            Rope rope = new Rope(Game.LocalPlayer.Character.GetOffsetPositionFront(2.5f), 40, 0.05f, 20.0f, 100.0f, 0.35f, new Vector3(0f, 0f, -9.81f), 0.15f, 0.2f, 2.0f, 100.0f);

            mgr.PhysicsObjects.Add(rope);

            Vector3 positionChange = new Vector3();
            mgr.AfterSimulate += (deltaTime) =>
            {
                if (positionChange != Vector3.Zero)
                {
                    rope.PinVertex(0, rope.GetVertexPosition(0) + positionChange * deltaTime);
                }
            };

            while (true)
            {
                GameFiber.Yield();

                positionChange = new Vector3();

                const float moveValue = 15f;
                if (Game.IsKeyDownRightNow(Keys.NumPad6))
                    positionChange.X += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad4))
                    positionChange.X -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad8))
                    positionChange.Y -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad2))
                    positionChange.Y += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad9))
                    positionChange.Z += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad3))
                    positionChange.Z -= moveValue;


                mgr.Update();
            }
        }

        [TestCase]
        public static void PhysicsRopeAttached()
        {
            PhysicsManager mgr = new PhysicsManager();

            Rope rope = new Rope(Game.LocalPlayer.Character.GetOffsetPositionFront(2.5f), 35, 0.05f, 15.0f, 100.0f, 0.35f, new Vector3(0f, 0f, -9.81f), 0.15f, 0.2f, 2.0f, 100.0f);

            mgr.PhysicsObjects.Add(rope);

            Vector3 positionChange = new Vector3();
            mgr.AfterSimulate += (deltaTime) =>
            {
                if (positionChange != Vector3.Zero)
                {
                    rope.PinVertex(0, rope.GetVertexPosition(0) + positionChange * deltaTime);
                }
                rope.PinVertex(rope.VertexCount - 1, Game.LocalPlayer.Character.Position);
            };

            while (true)
            {
                GameFiber.Yield();

                positionChange = new Vector3();

                const float moveValue = 15f;
                if (Game.IsKeyDownRightNow(Keys.NumPad6))
                    positionChange.X += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad4))
                    positionChange.X -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad8))
                    positionChange.Y -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad2))
                    positionChange.Y += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad9))
                    positionChange.Z += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad3))
                    positionChange.Z -= moveValue;


                mgr.Update();
            }
        }

        [TestCase]
        public static void PhysicsRopeAttachedVehicle()
        {
            PhysicsManager mgr = new PhysicsManager();
            
            Rope rope = new Rope(Game.LocalPlayer.Character.GetOffsetPositionFront(2.5f), 40, 0.05f, 20.0f, 100.0f, 0.35f, new Vector3(0f, 0f, -9.81f), 0.15f, 0.2f, 2.0f, 100.0f);
            rope.DetectGroundCollisions = true;

            mgr.PhysicsObjects.Add(rope);

            Vector3 positionChange = new Vector3();
            mgr.AfterSimulate += (deltaTime) =>
            {
                if (positionChange != Vector3.Zero)
                {
                    rope.PinVertex(0, rope.GetVertexPosition(0) + positionChange * deltaTime);
                }
                else
                {
                    if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                    {
                        rope.PinVertex(0, Game.LocalPlayer.Character.CurrentVehicle.RearPosition);
                    }
                    else
                    {
                        rope.PinVertex(0, Game.LocalPlayer.Character.Position);
                        if (Game.LocalPlayer.Character.LastVehicle)
                        {
                            rope.PinVertex(rope.VertexCount - 1, Game.LocalPlayer.Character.LastVehicle.RearPosition);
                        }
                    }
                }
            };

            while (true)
            {
                GameFiber.Yield();

                positionChange = new Vector3();

                const float moveValue = 15f;
                if (Game.IsKeyDownRightNow(Keys.NumPad6))
                    positionChange.X += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad4))
                    positionChange.X -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad8))
                    positionChange.Y -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad2))
                    positionChange.Y += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad9))
                    positionChange.Z += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad3))
                    positionChange.Z -= moveValue;


                mgr.Update();
            }
        }

        [TestCase]
        public static void PhysicsMultipleSpring()
        {
            const int particlesCount = 10;

            PhysicsManager mgr = new PhysicsManager();

            Vector3 firstParticlePos = Game.LocalPlayer.Character.GetOffsetPositionFront(0.225f);

            List<Particle> particles = new List<Particle>(particlesCount);
            for (int i = 0; i < particlesCount; i++)
            {
                Particle p = new Particle(1.0f) { IsAffectedByAirFriction = true, IsAffectedByGravity = i != 0 };

                if (i == 0)
                    p.Pin(firstParticlePos);
                else
                    p.Position = Game.LocalPlayer.Character.GetOffsetPosition(new Vector3(0f, 0.225f, i * -0.6f));

                particles.Add(p);
            }

            List<SpringJoint> springs = new List<SpringJoint>(particlesCount - 1);
            for (int i = 0; i < particlesCount - 1; i++)
            {
                SpringJoint spring = new SpringJoint(particles[i], particles[i + 1], 0.625f, 10.0f, 0.85f);
                springs.Add(spring);
            }


            mgr.PhysicsObjects.AddRange(particles);
            mgr.PhysicsObjects.AddRange(springs);

            Vector3 positionChange = new Vector3();
            mgr.AfterSimulate += (deltaTime) =>
            {
                if (positionChange != Vector3.Zero)
                {
                    Vector3 v = (positionChange / particles[0].Mass) * deltaTime;

                    particles[0].Pin(particles[0].Position + v * deltaTime);
                }
            };

            bool firstUpdate = true;
            while (true)
            {
                GameFiber.Yield();

                positionChange = new Vector3();

                const float moveValue = 25f;
                if (Game.IsKeyDownRightNow(Keys.NumPad6))
                    positionChange.X += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad4))
                    positionChange.X -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad8))
                    positionChange.Y -= moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad2))
                    positionChange.Y += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad9))
                    positionChange.Z += moveValue;

                if (Game.IsKeyDownRightNow(Keys.NumPad3))
                    positionChange.Z -= moveValue;


                mgr.Update();

                if (firstUpdate)
                {
                    for (int i = 0; i < particlesCount; i++)
                    {
                        particles[i].Velocity = Vector3.Zero;
                    }

                    firstUpdate = false;
                }
            }
        }

        [TestCase]
        public static void PhysicsGravity()
        {
            PhysicsManager mgr = new PhysicsManager();

            Particle a = new Particle(5.0f) { IsAffectedByAirFriction = true, IsAffectedByGravity = true };
            
            mgr.PhysicsObjects.Add(a);
            
            a.Position = Game.LocalPlayer.Character.GetOffsetPositionFront(2.5f);
            a.Velocity = Game.LocalPlayer.Character.Direction * 15.0f + Vector3.WorldUp * 15.0f;

            while (true)
            {
                GameFiber.Yield();

                mgr.Update();
            }
        }
    }
}
