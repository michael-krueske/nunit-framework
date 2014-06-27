using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Internal
{
#if !NETCF
    [TestFixture]
    public class GenericTypeInferenceTests
    {
        private static readonly object[][] GetTypeArgumentsForMethodCases =
        {
            new object[]
            {
                GetMethodInfo(() => SimpleGenericMethod(0)), 
                new object[] { default(int) }, 
                new [] {typeof(int)}
            }, 
            new object[]
            {
                GetMethodInfo(() => SimpleGenericMethod(0)), 
                new object[] { default(long) }, 
                new [] {typeof(long)}
            }, 
            new object[]
            {
                GetMethodInfo(() => SimpleGenericMethod(0)), 
                new object[] { default(double) }, 
                new [] {typeof(double)}
            }, 
            new object[]
            {
                GetMethodInfo(() => SimpleGenericMethod(0)), 
                new object[] { new object() }, 
                new [] {typeof(object)}
            }, 
            new object[]
            {
                GetMethodInfo(() => SimpleGenericMethod(0)), 
                new object[] { "Hello, World!" }, 
                new [] {typeof(string)}
            }, 
            new object[]
            {
                GetMethodInfo(() => SimpleGenericMethod2((int?)0)), 
                new object[] { (int?)0 }, 
                new [] {typeof(int)}
            }, 
        };

        [Test]
        [TestCaseSource("GetTypeArgumentsForMethodCases")]
        public void GetTypeArgumentsForMethod(MethodInfo methodInfo, object[] arglist, Type[] expected)
        {
            Type[] typeArguments = GenericTypeInference.GetTypeArgumentsForMethod(methodInfo, arglist);
            Assert.That(typeArguments, Is.EqualTo(expected));
        }

        public static void SimpleGenericMethod<TType>(TType arg)
        {
        }

        public static void SimpleGenericMethod2<TType>(TType? arg)
            where TType : struct
        {
        }

        // Utility method for type-safe retrieval of a method info from a lambda.
        // Replacement for the upcoming nameof() operator in C# 6.
        private static MethodInfo GetMethodInfo(Expression<Action> methodSelector)
        {
            return (methodSelector.Body as MethodCallExpression).Method;
        }
    }
#endif
}
