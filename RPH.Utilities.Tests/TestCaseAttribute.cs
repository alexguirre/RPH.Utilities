namespace RPH.Utilities.Tests
{
    // System
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TestCaseAttribute : Attribute
    {
        public TestCaseAttribute()
        {
        }
    }
}
