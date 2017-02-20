namespace RPH.Utilities.AI.Serialization
{
    // System
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    // RPH
    using Rage;

    [Serializable]
    public abstract class SerializableBehaviorTask
    {
        [XmlAttribute]
        public string TypeFullName { get; set; }

        [XmlArray]
        [XmlArrayItem(Type = typeof(object), ElementName = "Arg")]
        public object[] Args { get; set; }

        public abstract BehaviorTask CreateBehaviorTask();
    }

    [Serializable]
    [XmlRoot(ElementName = "Composite")]
    public class SerializableBehaviorComposite : SerializableBehaviorTask
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Child")]
        public SerializableBehaviorTask[] Children { get; set; }

        public override BehaviorTask CreateBehaviorTask()
        {
            Game.LogTrivial($"Searching type for '{TypeFullName}'");
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type behaviorTaskType = null;
            foreach (Assembly assembly in loadedAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.FullName == TypeFullName && !type.IsAbstract && type.IsSubclassOf(typeof(BehaviorTask)))
                    {
                        behaviorTaskType = type;
                        break;
                    }
                }
            }

            if (behaviorTaskType == null)
            {
                throw new InvalidOperationException($"Couldn't find the behavior task type '{TypeFullName}'");
            }
            Game.LogTrivial($"Found type for '{TypeFullName}':  {behaviorTaskType}");

            ConstructorInfo[] ctors = behaviorTaskType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            ConstructorInfo finalCtor = null;

            Game.LogTrivial($"Searching constructor - CtorsLength: {ctors.Length}");
            foreach (ConstructorInfo ctor in ctors)
            {
                if (Attribute.IsDefined(ctor, typeof(DeserializeBehaviorConstructorAttribute)))
                {
                    Game.LogTrivial($"Found constructor with DeserializeBehaviorConstructor attribute, checking parameters...");
                    ParameterInfo[] parameters = ctor.GetParameters();

                    Game.LogTrivial($"CtorParamsLength: {parameters.Length}     Args+ChildrenArrayLength: {(Args == null ? (1) : (Args.Length + 1))}");
                    if (Args == null ? (parameters.Length == 1) : (parameters.Length == Args.Length + 1))
                    {
                        bool typesCheckFailed = false;
                        Game.LogTrivial($"Equal length, checking parameters types...");
                        for (int i = 0; i < Args.Length; i++)
                        {
                            Game.LogTrivial($"CtorParameterType: {parameters[i].ParameterType.Name}      ArgsType: {Args[i].GetType().Name}");
                            if (parameters[i].ParameterType != Args[i].GetType())
                            {
                                typesCheckFailed = true;
                                break;
                            }
                        }

                        if (!typesCheckFailed)
                        {
                            Game.LogTrivial($"Parameters types check successful, breaking loop...");
                            finalCtor = ctor;
                            break;
                        }
                        else
                        {
                            Game.LogTrivial($"Parameters types check failed, iterating to next ctor or exiting loop...");
                        }
                    }
                }
            }

            if (finalCtor == null)
            {
                Game.LogTrivial($"No suitable constructor found...");
                throw new InvalidOperationException($"No suitable constructor found for '{TypeFullName}'");
            }

            object[] finalParams = new object[Args == null ? 1 : Args.Length + 1];
            if (Args != null)
            {
                Array.Copy(Args, 0, finalParams, 0, Args.Length);
            }
            finalParams[finalParams.Length - 1] = Children.Select(c => c.CreateBehaviorTask()).ToArray();
            object o = finalCtor.Invoke(finalParams);

            return (BehaviorTask)o;
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "Decorator")]
    public class SerializableBehaviorDecorator : SerializableBehaviorTask
    {
        [XmlElement(ElementName = "Child")]
        public SerializableBehaviorTask Child { get; set; }

        public override BehaviorTask CreateBehaviorTask()
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type behaviorTaskType = null;
            foreach (Assembly assembly in loadedAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.FullName == TypeFullName && !type.IsAbstract && type.IsSubclassOf(typeof(BehaviorTask)))
                    {
                        behaviorTaskType = type;
                        break;
                    }
                }
            }

            if (behaviorTaskType == null)
            {
                throw new InvalidOperationException($"Couldn't find the behavior task type '{TypeFullName}'");
            }

            ConstructorInfo[] ctors = behaviorTaskType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            ConstructorInfo finalCtor = null;

            foreach (ConstructorInfo ctor in ctors)
            {
                if (ctor.GetCustomAttribute<DeserializeBehaviorConstructorAttribute>() != null)
                {
                    ParameterInfo[] parameters = ctor.GetParameters();

                    if (Args == null ? (parameters.Length == 1) : (parameters.Length == Args.Length + 1))
                    {
                        bool typesCheckFailed = false;
                        for (int i = 0; i < Args.Length; i++)
                        {
                            if (parameters[i].ParameterType != Args[i].GetType())
                            {
                                typesCheckFailed = true;
                                break;
                            }
                        }

                        if (!typesCheckFailed)
                        {
                            finalCtor = ctor;
                            break;
                        }
                    }
                }
            }

            if (finalCtor == null)
            {
                throw new InvalidOperationException($"No suitable constructor found for '{TypeFullName}'");
            }

            object[] finalParams = new object[Args == null ? 1 : Args.Length + 1];
            if (Args != null)
            {
                Array.Copy(Args, 0, finalParams, 0, Args.Length);
            }
            finalParams[finalParams.Length - 1] = Child.CreateBehaviorTask();
            object o = finalCtor.Invoke(finalParams);

            return (BehaviorTask)o;
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "Leaf")]
    public class SerializableBehaviorLeaf : SerializableBehaviorTask
    {
        public override BehaviorTask CreateBehaviorTask()
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type behaviorTaskType = null;
            foreach (Assembly assembly in loadedAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.FullName == TypeFullName && !type.IsAbstract && type.IsSubclassOf(typeof(BehaviorTask)))
                    {
                        behaviorTaskType = type;
                        break;
                    }
                }
            }

            if (behaviorTaskType == null)
            {
                throw new InvalidOperationException($"Couldn't find the behavior task type '{TypeFullName}'");
            }

            ConstructorInfo[] ctors = behaviorTaskType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            ConstructorInfo finalCtor = null;

            foreach (ConstructorInfo ctor in ctors)
            {
                if (ctor.GetCustomAttribute<DeserializeBehaviorConstructorAttribute>() != null)
                {
                    ParameterInfo[] parameters = ctor.GetParameters();

                    if (Args == null ? (parameters.Length == 0) : (parameters.Length == Args.Length))
                    {
                        bool typesCheckFailed = false;
                        for (int i = 0; i < Args.Length; i++)
                        {
                            if (parameters[i].ParameterType != Args[i].GetType())
                            {
                                typesCheckFailed = true;
                                break;
                            }
                        }

                        if (!typesCheckFailed)
                        {
                            finalCtor = ctor;
                            break;
                        }
                    }
                }
            }

            if (finalCtor == null)
            {
                throw new InvalidOperationException($"No suitable constructor found for '{TypeFullName}'");
            }

            object[] finalParams = new object[Args == null ? 0 : Args.Length];
            if (Args != null)
            {
                Array.Copy(Args, 0, finalParams, 0, Args.Length);
            }

            object o = finalCtor.Invoke(finalParams);

            return (BehaviorTask)o;
        }
    }
}



//void a()
//{
//    SerialiazableBehaviorTask t = new SerialiazableBehaviorComposite()
//    {
//        TypeFullName = "RPH.Utilities.AI.Composites.Selector",
//        Args = new object[] { 20, "something", true },
//        Children = new SerialiazableBehaviorTask[]
//        {
//                    new SerialiazableBehaviorDecorator()
//                    {
//                        TypeFullName = "RPH.Utilities.AI.Decorators.Inverter",
//                        Args = new object[0],
//                        Child = new SerialiazableBehaviorLeaf()
//                        {
//                            TypeFullName = "RPH.Utilities.AI.Leafs.GoToPosition",
//                            Args = new object[] { 20f, 20f, 20f }
//                        },
//                    },

//                    new SerialiazableBehaviorLeaf()
//                    {
//                        TypeFullName = "RPH.Utilities.AI.Leafs.ShootAt",
//                        Args = new object[] { 2000, "BurstFire", "nearestPed" },
//                    }
//        },
//    };

//    using (StringWriter writer = new StringWriter())
//    {
//        XmlSerializer serializer = new XmlSerializer(typeof(SerialiazableBehaviorTask), new[] { typeof(SerialiazableBehaviorComposite), typeof(SerialiazableBehaviorDecorator), typeof(SerialiazableBehaviorLeaf) });
//        serializer.Serialize(writer, t);
//        Console.WriteLine(writer.ToString());
//    }
//}
