using System.Text.RegularExpressions;
using NUnit.Framework;
using Spine.DI;
using UnityEngine.TestTools;

public class Injector_Test {
	public class Subject {
	}

	public class InjectionTarget {
		[Inject]
		public Subject subject;

		[Inject]
		public int id;

		[Inject( optional: true )]
		public string Name { get; set; }
	}

	private Injector injector;

	public static void Log(object msg) {
		UnityEngine.Debug.Log( msg );
	}

	[SetUp]
	public void BeforeEachTest() {
		injector = new Injector();
	}

	[Test]
	public void Run() {
		Log( "Test Run!" );

		TypeDescriber.Example( typeof(InjectionTarget) );

		Assert.NotNull( injector );

		var subject = new Subject();
		var id = 1;

		injector.AutoMap( subject, id );

		var target = new InjectionTarget();

		injector.InjectInto( target );

		Assert.AreEqual( subject, target.subject );
		Assert.AreEqual( id, target.id );
	}

	/*[Test]
	public void Performance()
	{
		var subject = new Subject();
		var id = 1;
		injector.AutoMap(subject, id);

		int iters = 10000;

		var targets = new InjectionTarget[iters];
		for (var i = 0; i < iters; i++)
		{
			targets[i] = new InjectionTarget();
		}

		var sw = Stopwatch.StartNew();
		for (var i = 0; i < iters; i++)
		{
			injector.InjectInto(targets[i]);
		}
		Log($">> [v1] {iters} injection in {sw.ElapsedMilliseconds}ms");
	}*/

	[Test]
	public void AutoMapUniqueTypes_Passes() {
		var subject = new Subject();
		var id = 42;

		injector.AutoMap( subject, id );

		var target = new InjectionTarget();
		injector.InjectInto( target );
		Assert.AreEqual( subject, target.subject );
		Assert.AreEqual( id, target.id );
	}

	public class PrivateInjectTarget {
		[Inject]
		private string text;

		public string GetText() => text;
	}

	[Test]
	public void InjectIntoPrivateField_Passes() {
		var text = "test_text";
		injector.AutoMap( text );
		var target = new PrivateInjectTarget();
		injector.InjectInto( target );
		Assert.AreEqual( text, target.GetText() );
	}

	class OptionalInjectTarget {
		[Inject] Subject subject;

		public Subject GetSubject() => subject;
	};

	[Test]
	public void MissingRequiredDependency_Throws() {
		var target = new OptionalInjectTarget();

		LogAssert.Expect( UnityEngine.LogType.Error, new Regex( @"(?<error>)", RegexOptions.IgnoreCase ) );

		injector.InjectInto( target );
	}

	class PropertyInjectTarget {
		[Inject]
		public Subject Subject { get; private set; }
	};

	[Test]
	public void InectIntoProperty_Passes() {
		var subject = new Subject();

		injector.AutoMap( subject );

		var target = new PropertyInjectTarget();
		injector.InjectInto( target );

		Assert.AreEqual( subject, target.Subject );
	}

	struct StructInjectionClient {
		[Inject]
		public Subject subject;
	}

	[Test]
	public void InjectStruct() {
		var subject = new Subject();

		injector.AutoMap( subject );

		var client = injector.Inject<StructInjectionClient>();

		Assert.AreEqual( subject, client.subject );
	}

	[Test]
	public void InjectIntoStruct() {
		var subject = new Subject();

		injector.AutoMap( subject );

		var target = new StructInjectionClient();
		injector.InjectInto( ref target );

		Assert.AreEqual( subject, target.subject );
	}
}
