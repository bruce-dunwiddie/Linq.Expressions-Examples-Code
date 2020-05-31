using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Framework;

namespace LinqExpressionsTests
{
	[TestFixture]
	public class PropertyTests
	{
		[Test]
		public void Property_Getter()
		{
			XmlDocument xml = new XmlDocument();

			xml.InnerXml = "<document />";

			// locate the property
			PropertyInfo innerXml = xml
				.GetType()
				.GetProperty(
					"InnerXml");

			// ParameterExpression is used to map a value from the caller
			// to the lambda body
			ParameterExpression document = Expression.Parameter(
				xml.GetType());

			// define the body of the lambda
			
			// MemberExpression body of compiled expression calls the Get method of the property

			// could also create a MethodCallExpression using the MethodInfo returned by PropertyInfo.GetGetMethod()
			MemberExpression body = Expression.Property(
				document,
				innerXml);

			// Func<in T, out TResult>

			// first Type argument of Func definition will be Type of instance object
			// second Type argument of Func definition will be Type of return

			Expression<Func<XmlDocument, string>> lambda = Expression.Lambda<Func<XmlDocument, string>>(

				// this is going to be the function body/logic
				body,

				// this is the parameter list being passed in to the lambda body

				// have to use same ParameterExpression instances referenced in
				// expression body definition above to get them to map through Func call
				new ParameterExpression[] { document });

			// create the Func from the lambda
			Func<XmlDocument, string> func = lambda.Compile();

			// use Func to get value
			string content = func(xml);

			Console.WriteLine(content);

			Assert.AreEqual("<document />", content);
		}

		[Test]
		public void Property_Setter()
		{
			XmlDocument xml = new XmlDocument();

			// locate the property
			PropertyInfo innerXml = xml
				.GetType()
				.GetProperty(
					"InnerXml");

			// ParameterExpression is used to map a value from the caller
			// to the lambda body
			ParameterExpression document = Expression.Parameter(
				xml.GetType());

			ParameterExpression content = Expression.Parameter(
				typeof(string));

			// define the body of the lambda
			MethodCallExpression body = Expression.Call(
				document,
				innerXml.GetSetMethod(),
				new Expression[]
				{
					content
				});

			// Action<in T1, in T2>

			// first Type argument of Func definition will be Type of instance object
			// second Type argument of Func definition will be Type of value

			Expression<Action<XmlDocument, string>> lambda = Expression.Lambda<Action<XmlDocument, string>>(

				// this is going to be the function body/logic
				body,

				// this is the parameter list being passed in to the lambda body

				// have to use same ParameterExpression instances referenced in
				// expression body definition above to get them to map through Action call
				new ParameterExpression[]
				{
					document,
					content
				});

			// create the Action from the lambda
			Action<XmlDocument, string> action = lambda.Compile();

			// use Action to set value
			action(
				xml,
				"<document />");

			Console.WriteLine(xml.InnerXml);

			Assert.AreEqual("<document />", xml.InnerXml);
		}

		[Test]
		public void Property_IndexerGetter()
		{
			List<string> numbers = new List<string>
			{
				"one",
				"two"
			};

			// locate the property
			PropertyInfo indexer = numbers
				.GetType()
				.GetProperty(
					"Item",
					new Type[]
					{
						typeof(int)
					});

			// ParameterExpression is used to map a value from the caller
			// to the lambda body
			ParameterExpression list = Expression.Parameter(
				numbers.GetType());

			ConstantExpression index = Expression.Constant(1);

			// define the body of the lambda
			MethodCallExpression body = Expression.Call(
				list,
				indexer.GetGetMethod(),
				new Expression[]
				{
					index
				});

			// Func<in T, out TResult>

			// first Type argument of Func definition will be Type of instance object
			// second Type argument of Func definition will be Type of return

			Expression<Func<List<string>, string>> lambda = Expression.Lambda<Func<List<string>, string>>(

				// this is going to be the function body/logic
				body,

				// this is the parameter list being passed in to the lambda body

				// have to use same ParameterExpression instances referenced in
				// expression body definition above to get them to map through Func call
				new ParameterExpression[]
				{
					list
				});

			// create the Func from the lambda
			Func<List<string>, string> func = lambda.Compile();

			// use Func to get value
			string number = func(numbers);

			Console.WriteLine(number);

			Assert.AreEqual(
				"two",
				number);
		}

		[Test]
		public void Property_IndexerSetter()
		{
			List<string> numbers = new List<string>
			{
				"one",
				"two"
			};

			// locate the property
			PropertyInfo indexer = numbers
				.GetType()
				.GetProperty(
					"Item",
					new Type[]
					{
						typeof(int)
					});

			// ParameterExpression is used to map a value from the caller
			// to the lambda body
			ParameterExpression list = Expression.Parameter(
				numbers.GetType());

			ConstantExpression index = Expression.Constant(1);

			ParameterExpression number = Expression.Parameter(
				typeof(string));

			// define the body of the lambda
			MethodCallExpression body = Expression.Call(
				list,
				indexer.GetSetMethod(),
				new Expression[]
				{
					index,
					number
				});

			// Action<in T1, in T2>

			// first Type argument of Func definition will be Type of instance object
			// second Type argument of Func definition will be Type of value

			Expression<Action<List<string>, string>> lambda = Expression.Lambda<Action<List<string>, string>>(

				// this is going to be the function body/logic
				body,

				// this is the parameter list being passed in to the lambda body

				// have to use same ParameterExpression instances referenced in
				// expression body definition above to get them to map through Action call
				new ParameterExpression[]
				{
					list,
					number
				});

			// create the Action from the lambda
			Action<List<string>, string> action = lambda.Compile();

			// use Action to set value
			action(
				numbers,
				"three");

			Console.WriteLine(numbers[1]);

			Assert.AreEqual(
				"three",
				numbers[1]);
		}
	}
}
