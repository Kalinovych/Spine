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
	
	#region V3

	interface IService {
		
	}
	
	class Service : IService {
	}

	class Command {
		[Inject] public Service service;
	}

	class CommandWithIService {
		[Inject] public IService service;
	}

	[Test]
	public void Resolve_DI_MappedWithTypesAndInterface() {
		injector.Add<IService, Service>();
		var command = injector.Resolve<CommandWithIService>();
		Assert.NotNull( command.service );

		var anotherCommand = injector.Resolve<CommandWithIService>();
		Assert.AreSame( command.service, anotherCommand.service );
	}

	[Test]
	public void Resolve_DI_MappedWithTypes() {
		injector.Add<Service>();
		var command = injector.Resolve<Command>();
		Assert.NotNull( command.service );

		var anotherCommand = injector.Resolve<Command>();
		Assert.AreNotSame( command, anotherCommand );
		Assert.AreSame( command.service, anotherCommand.service );
	}

	[Test]
	public void Resolve_DI_MappedWithoutInstance() {
		var command = new Command();
		injector.Add<Service>();
		injector.Resolve( command );
		Assert.NotNull( command.service );

		var anotherCommand = new Command();
		injector.Resolve( anotherCommand );
		Assert.AreSame( command.service, anotherCommand.service );
	}

	[Test]
	public void Resolve_DI_WithTheSameProvidedInstance() {
		var service = new Service();
		var command = new Command();
		injector.Add( service );
		injector.Resolve( command );
		Assert.NotNull( command.service );
		Assert.AreSame( command.service, service );

		var anotherCommand = new Command();
		injector.Resolve( anotherCommand );
		Assert.AreSame( command.service, anotherCommand.service );
	}

	[Test]
	public void Resolve_DI_ByInterface_WithTheSameProvidedInstance() {
		var service = new Service();
		var command = new CommandWithIService();
		injector.Add<IService>( service );
		injector.Resolve( command );
		Assert.NotNull( command.service );
		Assert.AreSame( command.service, service );

		var anotherCommand = new CommandWithIService();
		injector.Resolve( anotherCommand );
		Assert.AreSame( command.service, anotherCommand.service );
	}

	#endregion V3

	[Test]
	public void Run() {
		Log( "Test Run!" );

		TypeDescriber.Example( typeof(InjectionTarget) );

		Assert.NotNull( injector );

		var subject = new Subject();
		var id = 1;

		injector.AutoMap( subject, id );

		var target = new InjectionTarget();

		injector.Resolve( target );

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
		injector.Resolve( target );
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
		injector.Resolve( target );
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

		injector.Resolve( target );
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
		injector.Resolve( target );

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

		var client = injector.Resolve<StructInjectionClient>();

		Assert.AreEqual( subject, client.subject );
	}

	[Test]
	public void InjectIntoStruct() {
		var subject = new Subject();

		injector.AutoMap( subject );

		var target = new StructInjectionClient();
		injector.Resolve( ref target );

		Assert.AreEqual( subject, target.subject );
	}
}
