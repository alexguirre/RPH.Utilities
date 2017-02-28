namespace RPH.Utilities.Physics
{
    // System
    using System.Collections.Generic;

    // RPH
    using Rage;

    public class PhysicsManager
    {
        public delegate void PhysicsManagerEventHandler();
        public delegate void PhysicsManagerDeltaTimeEventHandler(float deltaTime);

        public event PhysicsManagerEventHandler BeforeInit, AfterInit;
        public event PhysicsManagerEventHandler BeforeSolve, AfterSolve;
        public event PhysicsManagerDeltaTimeEventHandler BeforeSimulate, AfterSimulate;

#if DEBUG
        public bool DoDebugDrawing { get; set; } = true;
#else
        public bool DoDebugDrawing { get; set; } = false;
#endif

        public List<IPhysicsObject> PhysicsObjects { get; } = new List<IPhysicsObject>();

        private uint lastUpdateGameTime;

        public float DeltaTime { get; private set; }
        public float MaxPossibleDeltaTime { get; set; } = 0.02f;

        public void Update()
        {
            if (lastUpdateGameTime == 0)
                lastUpdateGameTime = Game.GameTime;

            uint currentGameTime = Game.GameTime;
            DeltaTime = (currentGameTime - lastUpdateGameTime) / 1000.0f;
            lastUpdateGameTime = currentGameTime;

            int numOfIterations = (int)(DeltaTime / MaxPossibleDeltaTime) + 1;

            if (numOfIterations != 0)
                DeltaTime /= numOfIterations;

            for (int i = 0; i < numOfIterations; i++)
            {
                for (int j = 0; j < PhysicsObjects.Count; j++)
                {
                    InitObjects();
                    SolveObjects();
                    SimulateObjects(DeltaTime);
                }
            }
        }

        protected void InitObjects()
        {
            BeforeInit?.Invoke();
            for (int i = 0; i < PhysicsObjects.Count; i++)
            {
                PhysicsObjects[i].Init();
            }
            AfterInit?.Invoke();
        }

        protected void SolveObjects()
        {
            BeforeSolve?.Invoke();
            for (int i = 0; i < PhysicsObjects.Count; i++)
            {
                PhysicsObjects[i].Solve();
            }
            AfterSolve?.Invoke();
        }

        protected void SimulateObjects(float deltaTime)
        {
            BeforeSimulate?.Invoke(deltaTime);
            for (int i = 0; i < PhysicsObjects.Count; i++)
            {
                PhysicsObjects[i].Simulate(deltaTime);

                if (DoDebugDrawing)
                {
                    PhysicsObjects[i].DebugDraw();
                }
            }
            AfterSimulate?.Invoke(deltaTime);
        }
    }
}
