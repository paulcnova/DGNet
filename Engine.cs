
namespace DGNet;

using DGNet.Data;
using DGNet.Inspector;
using DGNet.Models;

using Mono.Cecil;

using System.Collections.Generic;

public sealed class Engine : System.IDisposable
{
	#region Properties
	
	public const string Type_StaticHTML = "static-html";
	
	private string errorMessage;
	
	public ProjectEnvironment Environment { get; private set; }
	public IDatabase Database { get; private set; }
	
	public Phase Phase { get; private set; } = Phase.Init;
	public Dictionary<string, string> AssemblyMap { get; set; } = new Dictionary<string, string>();
	public Dictionary<string, AssemblyDefinition> AssemblyDefinitions { get; private set; } = new Dictionary<string, AssemblyDefinition>();
	public Dictionary<string, TypeDefinition> TypeDefinitions { get; private set; } = new Dictionary<string, TypeDefinition>();
	
	public Engine(ProjectEnvironment environment)
	{
		this.Environment = environment;
	}
	
	#endregion // Properties
	
	#region Public Methods
	
	public AssemblyDefinition GetAssemblyDefinition(string typeName)
		=> this.AssemblyDefinitions[this.Database.QueryOne<AssemblyMap>(typeName).AssemblyName];
	
	public bool Init()
	{
		this.Phase = Phase.Init;
		this.Database = new FileDatabase();
		if(!this.Database.Setup(this.Environment.InputPath))
		{
			this.errorMessage = $"Could not connect to database.\n{this.Database.GetErrorMessage()}";
			return false;
		}
		this.Database.Serialize<Inspection>(
			(db, id, item) => {
				if(item is PropertyInspection prop)
				{
					if(prop.HasGetter)
					{
						// TODO: Have this be done on the actual class.
						prop.GetterID = prop.Getter.XPath;
						db.Insert(prop.Getter.XPath, prop.Getter);
					}
					if(prop.HasSetter)
					{
						// TODO: Have this be done on the actual class.
						prop.SetterID = prop.Setter.XPath;
						db.Insert(prop.Setter.XPath, prop.Setter);
					}
				}
				if(item is EventInspection ev)
				{
					if(ev.HasAdder)
					{
						// TODO: Have this be done on the actual class.
						ev.AdderID = ev.Adder.XPath;
						db.Insert(ev.Adder.XPath, ev.Adder);
					}
					if(ev.HasRemover)
					{
						// TODO: Have this be done on the actual class.
						ev.RemoverID = ev.Remover.XPath;
						db.Insert(ev.Remover.XPath, ev.Remover);
					}
				}
			},
			(db, id, item) => {
				if(item is PropertyInspection prop)
				{
					if(prop.HasGetter)
					{
						prop.Getter = db.QueryOne<Inspection>(prop.GetterID);
					}
					if(prop.HasSetter)
					{
						prop.Setter = db.QueryOne<Inspection>(prop.SetterID);;
					}
				}
				if(item is EventInspection ev)
				{
					if(ev.HasAdder)
					{
						ev.Adder = db.QueryOne<Inspection>(ev.AdderID);
					}
					if(ev.HasRemover)
					{
						ev.Remover = db.QueryOne<Inspection>(ev.AdderID);
					}
				}
			}
		);
		this.SetupData();
		return true;
	}
	
	public bool Inspect()
	{
		this.Phase = Phase.Inspect;
		List<AssemblyTypes> asmTypes = this.Database.Query<AssemblyTypes>(this.Environment.AssembliesToInspect.ToArray());
		
		foreach(AssemblyTypes asmType in asmTypes)
		{
			foreach(string type in asmType.Types)
			{
				TypeInspection inspection = new TypeInspection(type, this);
				
				this.Database.InsertBulk(
					inspection.GetFields(this)
						.ConvertAll(item => (item.XPath, item as Inspection))
						.ToArray(),
					(id, item) => inspection.FieldIDs.Add(id)
				);
				this.Database.InsertBulk(
					inspection.GetProperties(this)
						.ConvertAll(item => (item.XPath, item as Inspection))
						.ToArray(),
					(id, item) => inspection.PropertyIDs.Add(id)
				);
				this.Database.InsertBulk(
					inspection.GetEvents(this)
						.ConvertAll(item => (item.XPath, item as Inspection))
						.ToArray(),
					(id, item) => inspection.EventIDs.Add(id)
				);
				this.Database.InsertBulk(
					inspection.GetMethods(this)
						.ConvertAll(item => (item.XPath, item as Inspection))
						.ToArray(),
					(id, item) => inspection.MethodIDS.Add(id)
				);
				
				this.Database.Insert(inspection.XPath, inspection);
			}
		}
		
		return true;
	}
	
	public bool Parse()
	{
		this.Phase = Phase.Parse;
		// TODO: Add parse logic
		return true;
	}
	
	public bool Render()
	{
		this.Phase = Phase.Render;
		// TODO: Add render logic
		return true;
	}
	
	public bool Generate()
	{
		this.Phase = Phase.Generate;
		// TODO: Add generate logic
		return true;
	}
	
	public string GetErrorMessage() => this.errorMessage;
	
	public void Dispose()
	{
		this.Database.Dispose();
	}
	
	#endregion // Public Methods
	
	#region Private Methods
	
	private void SetupData()
	{
		List<AssemblyDefinition> definitions = new List<AssemblyDefinition>();
		Dictionary<string, List<string>> types = new Dictionary<string, List<string>>();
		
		foreach(string assembly in this.Environment.Assemblies)
		{
			AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(assembly);
			
			definitions.Add(asm);
		}
		
		KnownAssemblyResolver resolver = new KnownAssemblyResolver(definitions);
		ReaderParameters readerParameters = new ReaderParameters() { AssemblyResolver = resolver };
		
		foreach(string assembly in this.Environment.Assemblies)
		{
			AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(assembly, readerParameters);
			int index = System.Math.Max(assembly.LastIndexOf('/'), assembly.LastIndexOf('\\'));
			string asmName = (index == -1 ? assembly : assembly.Substring(index + 1));
			
			if(!types.ContainsKey(asmName))
			{
				types.Add(asmName, new List<string>());
			}
			this.AssemblyDefinitions.Add(asmName, asm);
			
			foreach(ModuleDefinition module in asm.Modules)
			{
				foreach(TypeDefinition type in module.GetTypes())
				{
					if(type.FullName.Contains('<') && type.FullName.Contains('>'))
					{
						continue;
					}
					if(this.Environment.IgnorePrivate)
					{
						if(type.IsNotPublic) { continue; }
						if(type.IsNestedAssembly || type.IsNestedPrivate) { continue; }
						
						TypeDefinition nestedType = type;
						
						while(nestedType.IsNested)
						{
							nestedType = nestedType.DeclaringType;
						}
						
						if(nestedType.IsNotPublic) { continue; }
					}
					types[asmName].Add(type.FullName);
					this.TypeDefinitions.Add(type.FullName, type);
					this.AssemblyMap.Add(type.FullName, asmName);
					this.Database.Insert(type.FullName, new AssemblyMap()
					{
						TypeName = type.FullName,
						AssemblyName = asmName,
					});
				}
			}
			this.Database.Insert(asmName, new AssemblyTypes()
			{
				AssemblyName = asmName,
				Types = types[asmName],
			});
		}
	}
	
	#endregion // Private Methods
}
