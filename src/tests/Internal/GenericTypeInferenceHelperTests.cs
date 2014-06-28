using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Internal
{
#if !NETCF
    [TestFixture]
    public class GenericTypeInferenceHelperTests
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
            () => SimpleGenericMethodWith2Args(0L, 0M), 
            () => SimpleGenericMethodWith2Args(0L, 0.0F), 
            () => SimpleGenericMethodWith2Args(0.0F, 0.0F), 
            () => SimpleGenericMethodWith2Args(0.0F, 0.0), 
            //() => SimpleGenericMethodWith2Args(0.0, 0M), 
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
            MethodCallExpression methodCallExpression = (genericMethodInvocation.Body as MethodCallExpression);
            
            MethodInfo methodInfo = methodCallExpression.Method;
            object[] actualArguments =
                (from argumentExpresssion in methodCallExpression.Arguments
                select Expression.Lambda(argumentExpresssion).Compile().DynamicInvoke()
                ).ToArray();

            Type[] expectedTypeArguments = methodInfo.GetGenericArguments();
            Type[] actualTypeArguments = GenericTypeInferenceHelper.InferTypeArguments(methodInfo, actualArguments);

            Assert.That(actualTypeArguments, Is.EqualTo(expectedTypeArguments));
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
