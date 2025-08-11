
namespace DGNet;

using Mono.Cecil;

using System.Collections.Generic;
using System.Linq;

public sealed class Engine : System.IDisposable
{
	#region Properties
	
	public const string Type_StaticHTML = "static-html";
	
	private string errorMessage;
	
	public ProjectEnvironment Environment { get; private set; }
	public Database Database { get; private set; }
	
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
	
	public AssemblyDefinition GetAssemblyDefinition(string typeName) => this.AssemblyDefinitions[this.AssemblyMap[typeName]];
	
	public bool Init()
	{
		this.Phase = Phase.Init;
		this.Database = new Database();
		if(!this.Database.Connect(this.Environment.InputPath))
		{
			this.errorMessage = "Could not connect to LiteDB database.";
			return false;
		}
		this.SetupData();
		return true;
	}
	
	public bool Inspect()
	{
		this.Phase = Phase.Inspect;
		// TODO: Add inspect logic
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
		
	}
	
	#endregion // Private Methods
}
