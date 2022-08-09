using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rabbiter.Core
{
	public static class TypeExtensions
	{
		public static object GetInstance(this Type type)
		{
			return GetInstance<TypeToIgnore>(type, null);
		}
		
		public static object GetInstance<TArg>(
			this Type type,
			TArg argument)
		{
			return GetInstance<TArg, TypeToIgnore>(type, argument, null);
		}
		
		public static object GetInstance<TArg1, TArg2>(
			this Type type,
			TArg1 argument1,
			TArg2 argument2)
		{
			return GetInstance<TArg1, TArg2, TypeToIgnore>(
				type, argument1, argument2, null);
		}
		
		public static object GetInstance<TArg1, TArg2, TArg3>(
			this Type type,
			TArg1 argument1,
			TArg2 argument2,
			TArg3 argument3)
		{
			return GetInstance<TArg1, TArg2, TArg3, TypeToIgnore>(
				type, argument1, argument2, argument3, null);
		}

		public static object GetInstance<TArg1, TArg2, TArg3, TArg4>(
			this Type type,
			TArg1 argument1,
			TArg2 argument2,
			TArg3 argument3,
			TArg4 argument4)
		{
			return GetInstance<TArg1, TArg2, TArg3, TArg4, TypeToIgnore>(
				type, argument1, argument2, argument3, argument4, null);
		}
		
		public static object GetInstance<TArg1, TArg2, TArg3, TArg4, TArg5>(
			this Type type,
			TArg1 argument1,
			TArg2 argument2,
			TArg3 argument3,
			TArg4 argument4,
			TArg5 argument5)
		{
			return GetInstance<TArg1, TArg2, TArg3, TArg4, TArg5, TypeToIgnore>(
				type, argument1, argument2, argument3, argument4, argument5, null);
		}
		
		public static object GetInstance<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
			this Type type,
			TArg1 argument1,
			TArg2 argument2,
			TArg3 argument3,
			TArg4 argument4,
			TArg5 argument5,
			TArg6 argument6)
		{
			return InstanceCreationFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
				.CreateInstanceOf(type, argument1, argument2, argument3, argument4, argument5, argument6);
		}

		private class TypeToIgnore
		{
		}

		private static class InstanceCreationFactory<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
		{
			private static readonly 
				Dictionary<Type, Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, object>> _instanceCreationMethods = new();

			public static object CreateInstanceOf(
				Type type,
				TArg1 arg1,
				TArg2 arg2,
				TArg3 arg3,
				TArg4 arg4,
				TArg5 arg5,
				TArg6 arg6)
			{
				CacheInstanceCreationMethodIfRequired(type);

				return _instanceCreationMethods[type]
					.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
			}

			private static void CacheInstanceCreationMethodIfRequired(
				Type type)
			{
				if (_instanceCreationMethods.ContainsKey(type))
				{
					return;
				}

				var argumentTypes = new[]
				{
					typeof(TArg1), typeof(TArg2), typeof(TArg3),
					typeof(TArg4), typeof(TArg5), typeof(TArg6),
				};

				Type[] constructorArgumentTypes = argumentTypes
					.Where(t => t != typeof(TypeToIgnore))
					.ToArray();
				
				var constructor = type.GetConstructor(
					BindingFlags.Instance | BindingFlags.Public,
					null,
					CallingConventions.HasThis,
					constructorArgumentTypes,
					new ParameterModifier[0]);
				
				var lamdaParameterExpressions = new[]
				{
					Expression.Parameter(typeof(TArg1), "param1"),
					Expression.Parameter(typeof(TArg2), "param2"),
					Expression.Parameter(typeof(TArg3), "param3"),
					Expression.Parameter(typeof(TArg4), "param4"),
					Expression.Parameter(typeof(TArg5), "param5"),
					Expression.Parameter(typeof(TArg6), "param6"),
				};
				
				var constructorParameterExpressions =
					lamdaParameterExpressions
						.Take(constructorArgumentTypes.Length)
						.ToArray();

				var constructorCallExpression = Expression
					.New(constructor, constructorParameterExpressions);
				
				var constructorCallingLambda = Expression
					.Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, object>>(
						constructorCallExpression,
						lamdaParameterExpressions)
					.Compile();

				_instanceCreationMethods[type] = constructorCallingLambda;
			}
		}
	}
}
