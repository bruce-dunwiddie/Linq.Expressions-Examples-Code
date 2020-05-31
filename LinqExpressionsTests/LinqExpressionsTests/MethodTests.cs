using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

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
				
				// this is the parameter list being passed in to the lambda body

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

				// this is the parameter list being passed in to the lambda body

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

		[Test]
		public void Method_InstanceMethodWithNoReturn()
		{
			List<string> words = new List<string>() { "one", "two" };

			// locate the method to be called
			MethodInfo add = words
				.GetType()
				.GetMethod(
					"Add", 
					new Type[] { typeof(string) });

			// ParameterExpression is used to map a value from the caller
			// to the lambda body

			// this is the same for object instances as well as method
			// parameters
			ParameterExpression list = Expression.Parameter(
				words.GetType());

			ParameterExpression word = Expression.Parameter(
				typeof(string));

			// define the body of the lambda

			// we're going to just have a single method call within the body
			MethodCallExpression body = Expression.Call(
				list,
				add,
				new Expression[] { word });

			// Action<in T1, in T2>

			Expression<Action<List<string>, string>> lambda = Expression.Lambda<Action<List<string>, string>>(
				
				// this is going to be the function body/logic

				// the value returned from a function is the last expression in its body
				body,

				// this is the parameter list being passed in to the lambda body

				// have to use same ParameterExpression instances referenced in
				// expression body definition above to get them to map through Func call

				// remember to add instance parameter
				new ParameterExpression[] { list, word });

			// create the Action from the lambda
			Action<List<string>, string> action = lambda.Compile();

			// use Action to execute method
			action(words, "three");

			string wordsList = string.Join(",", words);

			Console.WriteLine(wordsList);

			Assert.AreEqual(3, words.Count);
			Assert.AreEqual("three", words[2]);
		}

		[Test]
		public void Method_InnerMethod()
		{
			List<string> dates = new List<string>();

			// locate the methods to be called
			MethodInfo add = dates
				.GetType()
				.GetMethod(
					"Add",
					new Type[] { typeof(string) });

			MethodInfo toString = typeof(DateTime)
				.GetMethod(
					"ToString",
					new Type[] { typeof(string) });

			// ParameterExpression is used to map a value from the caller
			// to the lambda body

			// this is the same for object instances as well as method
			// parameters
			ParameterExpression list = Expression.Parameter(
				dates.GetType());

			ParameterExpression date = Expression.Parameter(
				typeof(DateTime));

			// use Expression.Constant to convert a 'normal' value into an expression
			ConstantExpression format = Expression.Constant(
				"MMM d");

			MethodCallExpression toStringCall = Expression.Call(
				date,
				toString,
				new Expression[] { format });

			// define the body of the lambda

			// we're passing in the body of one method call in as a
			// parameter to another method call
			MethodCallExpression body = Expression.Call(
				list,
				add,
				new Expression[] { toStringCall });

			Expression<Action<List<string>, DateTime>> lambda = Expression.Lambda<Action<List<string>, DateTime>>(
				
				// this is going to be the function body/logic

				// the value returned from a function is the last expression in its body
				body,

				// this is the parameter list being passed in to the lambda body

				// have to use same ParameterExpression instances referenced in
				// expression body definition above to get them to map through Func call
				new ParameterExpression[] { list, date });

			// create the Action from the lambda
			Action<List<string>, DateTime> action = lambda.Compile();

			// use Action to execute method
			
			// this will format the date using the specified format
			// and add the formatted string to the list
			action(dates, DateTime.Parse("1/1/2020"));
			action(dates, DateTime.Parse("1/2/2020"));
			action(dates, DateTime.Parse("2/1/2020"));

			string datesAsString = string.Join(",", dates);

			Console.WriteLine(datesAsString);

			Assert.AreEqual(3, dates.Count);
			Assert.AreEqual("Jan 1", dates[0]);
			Assert.AreEqual("Jan 2", dates[1]);
			Assert.AreEqual("Feb 1", dates[2]);
		}

		[Test]
		public void Method_InstanceWithMultipleParameters()
		{
			StringBuilder content = new StringBuilder();

			// locate the method to be called
			
			// System.Text.StringBuilder Append(char value, int repeatCount)
			// Appends a specified number of copies of the string representation of a Unicode character to this instance.
			MethodInfo appendNChars = typeof(StringBuilder)
				.GetMethod(
					"Append",
					new Type[]
					{
						typeof(char), 
						typeof(int)
					});

			// ParameterExpression is used to map a value from the caller
			// to the lambda body

			// this is the same for object instances as well as method
			// parameters
			ParameterExpression builder = Expression.Parameter(
				typeof(StringBuilder));

			ParameterExpression characterToRepeat = Expression.Parameter(
				typeof(char));

			ParameterExpression numberOfRepeats = Expression.Parameter(
				typeof(int));

			// define the body of the lambda

			// we're going to just have a single method call within the body
			MethodCallExpression body = Expression.Call(
				builder,
				appendNChars,
				new Expression[]
				{
					characterToRepeat,
					numberOfRepeats
				});

			Expression<Action<StringBuilder, char, int>> lambda = Expression.Lambda<Action<StringBuilder, char, int>>(
				
				// this is going to be the function body/logic

				// the value returned from a function is the last expression in its body
				body,

				// this is the parameter list being passed in to the lambda body

				// have to use same ParameterExpression instances referenced in
				// expression body definition above to get them to map through Func call

				// remember to add instance parameter
				new ParameterExpression[]
				{
					builder,
					characterToRepeat,
					numberOfRepeats
				});

			// create the Action from the lambda
			Action<StringBuilder, char, int> action = lambda.Compile();

			// use Action to execute method
			action(content, 'a', 3);
			action(content, 'b', 4);

			Console.WriteLine(content.ToString());

			Assert.AreEqual("aaabbbb", content.ToString());
		}
	}
}