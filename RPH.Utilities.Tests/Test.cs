namespace RPH.Utilities.Tests
{
    // System
    using System;
    using System.Linq;
    using System.Reflection;

    // RPH
    using Rage;

    public static class Test
    {
        public static void ExecuteTests(Type type)
        {
            Game.LogTrivial($"START EXECUTING TESTS IN TYPE: {type.Name}");

            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods)
            {
                if (method.GetCustomAttribute<TestCaseAttribute>() != null)
                {
                    ExecuteTest(method);
                }
            }

            Game.LogTrivial($"FINISHED EXECUTING TESTS IN TYPE: {type.Name}");
        }

        public static void ExecuteTest(MethodInfo method)
        {
            Game.LogTrivial($"START EXECUTING TEST: {method.Name}");
            TestCaseAttribute att = method.GetCustomAttribute<TestCaseAttribute>();
            if (att == null)
            {
                Game.LogTrivial($"FAILED TO EXECUTE TEST: {method.Name}");
                Game.LogTrivial($"  The method {method.Name} doesn't have the {nameof(TestCaseAttribute)}");
                Game.Console.Print();
                return;
            }

            try
            {
                method.Invoke(null, null);
                Game.LogTrivial($"FINISHED EXECUTING TEST: {method.Name}");
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"FAILED TO EXECUTE TEST: {method.Name}");
                Game.LogTrivial($"  Exception:{Environment.NewLine}{ex}");
                Game.Console.Print();
                Game.DisplayHelp($"~r~Test {method.Name} failed!");
            }
        }
    }
}
