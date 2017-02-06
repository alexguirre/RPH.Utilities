namespace RPH.Utilities.AI
{
    // System
    using System;
    using System.Collections.Generic;

    // RPH
    using Rage;

    internal static class ComplexObjectUpdater
    {
        public static Dictionary<Type, GameFiber> GameFibersByType = new Dictionary<Type, GameFiber>();
        public static Dictionary<Type, List<ComplexObject>> ComplexObjectsByType = new Dictionary<Type, List<ComplexObject>>();


        public static void RegisterComplexObject(ComplexObject complexObject)
        {
            Type t = complexObject.GetType();

            if (ComplexObjectsByType.ContainsKey(t))
            {
                ComplexObjectsByType[t].Add(complexObject);
            }
            else
            {
                ComplexObjectsByType.Add(t, new List<ComplexObject>() { complexObject });
            }

            if (!GameFibersByType.ContainsKey(t))
            {
                GameFiber fiber = GameFiber.StartNew(() => { UpdateComplexObjectsLoop(ComplexObjectsByType[t]); }, $"{t.Name} Update Fiber");
                GameFibersByType.Add(t, fiber);
            }
        }

        private static void UpdateComplexObjectsLoop(List<ComplexObject> list)
        {
            while (true)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    ComplexObject o = list[i];
                    if (o != null)
                    {
                        if (o.CanUpdate)
                        {
                            o.Update();
                        }
                    }
                    else
                    {
                        list.RemoveAt(i);
                    }
                }

                GameFiber.Yield();
            }
        }
    }
}
