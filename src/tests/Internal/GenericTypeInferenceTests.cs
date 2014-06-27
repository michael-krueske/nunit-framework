using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Internal
{
#if !NETCF
    [TestFixture]
    public class GenericTypeInferenceTests
    {
        private static readonly Expression<Action>[] TestCasesThatShouldWorkExactlyLikeTheCompiler =
        {
            () => SimpleGenericMethod(0), 
            () => SimpleGenericMethod(0L), 
            () => SimpleGenericMethod(0M), 
            () => SimpleGenericMethod(0.0), 
            () => SimpleGenericMethod(0.0F), 
            () => SimpleGenericMethod(new object()), 
            () => SimpleGenericMethod("Hello, World!"), 
            () => SimpleGenericMethodWith2Args(0, 0), 
            () => SimpleGenericMethodWith2Args(0, 0L), 
            () => SimpleGenericMethodWith2Args(0L, 0.0), 
            () => MoreComplexGenericMethod((int?)0), 
        };

        private static readonly Expression<Action>[] TestCasesThatShouldFail =
        {
            // should fail...
            () => SimpleGenericMethodWith2Args<object>(0.0, 0m), 
        };

        // todo make a new test method
        // todo this 
        [Test]
        [TestCaseSource("TestCasesThatShouldWorkExactlyLikeTheCompiler")]
        public void GetTypeArgumentsForMethod2(Expression<Action> genericMethodInvocation)
        {
            var methodCallExpression = (genericMethodInvocation.Body as MethodCallExpression);
            var methodInfo = methodCallExpression.Method;
            
            var expected = methodInfo.GetGenericArguments();

            var arglist = methodCallExpression.Arguments.AsEnumerable().Select(x => Expression.Lambda(x).Compile().DynamicInvoke()).ToArray();

            Type[] typeArguments = GenericTypeInferenceHelper.GetTypeArgumentsForMethod(methodInfo, arglist);
            Assert.That(typeArguments, Is.EqualTo(expected));
        }

        public static void SimpleGenericMethod<TType>(TType arg)
        {
        }

        public static void SimpleGenericMethodWith2Args<TType>(TType arg1, TType arg2)
        {
        }

        public static void MoreComplexGenericMethod<TType>(TType? arg)
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
