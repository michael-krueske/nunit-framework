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
            // todo ooops, I think my method is smarter than the compiler!
            () => SimpleGenericMethodWith2Args<Animal>(new Cat(), new Mouse()), 
            () => SimpleGenericMethodWith2Args(new Animal(), new Mouse()), 
            //() => SimpleGenericMethodWith2Args(0.0, 0M), 
            // todo inference ?
            () => GenericMethodWithNullableArgument((int?)0), 
            () => GenericMethodWithGenericArgument(new Generic<Human>()), 
        };

        // todo make a new test method
        // todo this 
        [Test]
        [TestCaseSource("TestCasesThatShouldWorkExactlyLikeTheCompiler")]
        public void InferTypeArguments_FromActualArguments(Expression<Action> genericMethodInvocation)
        {
            MethodCallExpression methodCallExpression = (genericMethodInvocation.Body as MethodCallExpression);
            
            MethodInfo methodInfo = methodCallExpression.Method;
            Type[] argumentTypes = methodCallExpression.Arguments.Select(x => x.Type).ToArray();

            Type[] expectedTypeArguments = methodInfo.GetGenericArguments();

            var inferredMethod = 
                methodInfo.GetGenericMethodDefinition()
                    .InferTypeArguments()
                    .FromArgumentTypes(argumentTypes);

            Type[] actualTypeArguments = inferredMethod.GetGenericArguments();

            Assert.That(actualTypeArguments, Is.EqualTo(expectedTypeArguments));
        }

        public static void SimpleGenericMethod<TType>(TType arg)
        {
        }

        private static void SimpleGenericMethodWith2Args<TType>(TType arg1, TType arg2)
        {
        }

        private static void GenericMethodWithNullableArgument<TType>(TType? arg)
            where TType : struct
        {
        }

        private static void GenericMethodWithGenericArgument<TType>(Generic<TType> arg)
        {
        }

        private class Human
        {
        }

        private class Generic<TType>
        {
            
        }

        private class Animal
        {
        }

        private class Cat : Animal
        {
        }

        private class Mouse : Animal
        {
        }
    }
#endif
}
