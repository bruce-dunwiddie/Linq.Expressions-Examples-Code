using System;
using System.Linq.Expressions;
using System.Reflection;

using NUnit.Framework;

namespace LinqExpressionsTests
{
	[TestFixture]
	public class MethodTests
	{
		[Test]
		public void Method_SimpleStaticCallWithReturn()
		{
			// locate the method to be called
			MethodInfo parse = typeof(System.DateTime)
				.GetMethod(
					"Parse",
					new Type[] { typeof(string) });

			// ParameterExpression is used to map a value from the caller
			// to the lambda body
			ParameterExpression dateAsString = Expression.Parameter(
				typeof(string));

			// define the body of the lambda

			// we're going to just have a single method call within the body

			// we're using the overload with just two arguments because we're
			// calling a static method with no instance object
			MethodCallExpression body = Expression.Call(
				parse,
				new Expression[] { dateAsString });

			// Func<in T, out TResult>

			// first Type argument of Func definition will be Type of instance object
			// second Type argument of Func definition will be Type of return

			Expression<Func<string, DateTime>> lambda = Expression.Lambda<Func<string, DateTime>>(
				
				// this is going to be the function body/logic
				body,
				
				// these is the parameter list being passed in to the lambda body

				// have to use same ParameterExpression instances referenced in
				// expression body definition above to get them to map through Func call
				new ParameterExpression[] { dateAsString });

			// create the Func from the lambda
			Func<string, DateTime> func = lambda.Compile();

			// use Func to execute method
			DateTime date = func("1/2/2020 1:23:45.678");

			string formattedDate = date.ToString();

			Console.WriteLine(formattedDate);

			Assert.AreEqual(
				DateTime.Parse("1/2/2020 1:23:45.678"),
				date);
		}

		[Test]
		public void Method_InstanceMethodWithReturnHardcodedParameter()
		{
			DateTime testDate = DateTime.Parse("1/2/2020 1:23:45.678");

			// locate the method to be called
			MethodInfo toString = testDate
				.GetType()
				.GetMethod(
					"ToString",
					new Type[] { typeof(string) });

			// ParameterExpression is used to map a value from the caller
			// to the lambda body
			ParameterExpression date = Expression.Parameter(
				testDate.GetType());

			// use Expression.Constant to convert a 'normal' value into an expression
			ConstantExpression format = Expression.Constant(
				"hh:mm:ss");

			// define the body of the lambda

			// we're going to just have a single method call within the body
			MethodCallExpression body = Expression.Call(
				date,
				toString,
				new Expression[] { format });

			// Func<in T, out TResult>

			// first Type argument of Func definition will be Type of instance object
			// second Type argument of Func definition will be Type of return

			Expression<Func<DateTime, string>> lambda = Expression.Lambda<Func<DateTime, string>>(

				// this is going to be the function body/logic

				// the value returned from a function is the last expression in its body
				body,

				// these is the parameter list being passed in to the lambda body

				// have to use same ParameterExpression instances referenced in
				// expression body definition above to get them to map through Func call
				new ParameterExpression[] { date });

			// create the Func from the lambda
			Func<DateTime, string> func = lambda.Compile();

			// use Func to execute method
			string formattedDate = func(testDate);

			Console.WriteLine(formattedDate);

			Assert.AreEqual("01:23:45", formattedDate);
		}
	}
}