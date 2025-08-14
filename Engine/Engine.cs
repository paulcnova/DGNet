
namespace DGNet;

using DGNet.Data;
using DGNet.Inspector;
using DGNet.Models;

using Mono.Cecil;

using System.Collections.Generic;
using System.Xml;

public sealed partial class Engine : System.IDisposable
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
