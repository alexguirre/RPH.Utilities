namespace RPH.Utilities.Tests
{
    // System
    using System;

    [Serializable]
    public class AssertFailedException : Exception
    {
        public AssertFailedException()
        {
        }

        public AssertFailedException(string message) : base(message)
        {
        }

        public AssertFailedException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public static class Assert
    {
        public static void AreEqual(bool expected, bool actual, string message = null)
        {
            try
            {
                if (expected != actual)
                {
                    throw new AssertFailedException($"{nameof(AreEqual)} assertion failed, values are not equal.{Environment.NewLine}Expected:{expected} Actual:{actual}.{Environment.NewLine} {message}");
                }
            }
            catch (Exception ex)
            {
                throw new AssertFailedException($"Exception was thrown during assertion in {nameof(AreEqual)}", ex);
            }
        }

        public static void AreNotEqual(bool expected, bool actual, string message = null)
        {
            try
            {
                if (expected == actual)
                {
                    throw new AssertFailedException($"{nameof(AreNotEqual)} assertion failed, values are equal.{Environment.NewLine}Expected:{expected} Actual:{actual}.{Environment.NewLine} {message}");
                }
            }
            catch (Exception ex)
            {
                throw new AssertFailedException($"Exception was thrown during assertion in {nameof(AreNotEqual)}", ex);
            }
        }
    }
}
