namespace RPH.Utilities.AI.Serialization
{
    // System
    using System;

    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class DeserializeBehaviorConstructorAttribute : Attribute
    {
        public DeserializeBehaviorConstructorAttribute()
        {
        }
    }
}
