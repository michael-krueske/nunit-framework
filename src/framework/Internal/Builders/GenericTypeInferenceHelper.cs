using System;
using System.Reflection;

namespace NUnit.Framework.Internal.Builders
{
#if !NETCF
    /// <summary>
    /// Utility class for supporting generic type inference.
    /// </summary>
    public static class GenericTypeInferenceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arglist"></param>
        /// <returns></returns>
        public static Type[] InferTypeArguments(MethodInfo method, object[] arglist)
        {
            Type[] typeParameters = method.GetGenericArguments();
            Type[] typeArguments = new Type[typeParameters.Length];
            ParameterInfo[] parameters = method.GetParameters();

            for (int typeIndex = 0; typeIndex < typeArguments.Length; typeIndex++)
            {
                Type typeParameter = typeParameters[typeIndex];

                for (int argIndex = 0; argIndex < parameters.Length; argIndex++)
                {
                    if (parameters[argIndex].ParameterType == typeParameter)
                    {
                        // If a null arg is provided, pass null as the Type
                        // BestCommonType knows how to deal with this
                        Type argType = arglist[argIndex] != null
                            ? arglist[argIndex].GetType()
                            : null;
                        typeArguments[typeIndex] = TypeHelper.BestCommonType(
                            typeArguments[typeIndex],
                            argType);
                    }
                }
            }

            return typeArguments;
        }
    }
#endif
}