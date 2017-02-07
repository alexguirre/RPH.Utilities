namespace RPH.Utilities.TestCases
{
    // System
    using System;
    using System.Reflection;
    using System.Collections.Generic;

    // Util
    using RPH.Utilities.Tests;

    // RPH
    using Rage;
    using Rage.Attributes;
    using Rage.ConsoleCommands;
    using Rage.ConsoleCommands.AutoCompleters;

    internal static class Plugin
    {
        private static Dictionary<string, MethodInfo> TestMethodsByName = new Dictionary<string, MethodInfo>();

        private static void Main()
        {
            Game.LogTrivial("Getting test methods");
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    TestCaseAttribute att = method.GetCustomAttribute<TestCaseAttribute>();
                    if (att != null)
                    {
                        Game.LogTrivial($" - {method.Name}");
                        TestMethodsByName.Add(method.Name, method);
                    }
                }
            }

            GameFiber.Hibernate();
        }

        private static void OnUnload(bool isTerminating)
        {
        }

        [ConsoleCommand]
        private static void ExecuteTest([ConsoleCommandParameter(AutoCompleterType = typeof(ConsoleCommandTestsNamesAutoCompleter))] string testName)
        {
            while (Game.Console.IsOpen)
                GameFiber.Sleep(100);

            if (TestMethodsByName.ContainsKey(testName))
            {
                Test.ExecuteTest(TestMethodsByName[testName]);
            }
        }

        [Serializable]
        [ConsoleCommandParameterAutoCompleter(typeof(string))]
        public class ConsoleCommandTestsNamesAutoCompleter : ConsoleCommandParameterAutoCompleter
        {
            public ConsoleCommandTestsNamesAutoCompleter(Type type) : base(type)
            {
            }

            public override void UpdateOptions()
            {
                this.Options.Clear();
                foreach (string methodName in TestMethodsByName.Keys)
                {
                    this.Options.Add(new AutoCompleteOption(methodName, methodName, null));
                }
            }
        }
    }
}
