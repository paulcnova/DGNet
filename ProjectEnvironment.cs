
namespace DGNet;

using DGNet.Data;

using System.Collections.Generic;

public sealed class ProjectEnvironment
{
	#region Properties
	
	public string Type { get; set; }
	public string InputPath { get; set; }
	public List<string> AssembliesToInspect { get; set; } = new List<string>();
	public string DatabaseType { get; set; }
	
	public string[] Assemblies { get; set; }
	public string[] XmlFiles { get; set; }
	public bool IgnorePrivate { get; set; }
	
	#endregion // Properties
	
	#region Public Methods
	
	public IDatabase CreateDatabase() => this.DatabaseType switch
	{
		"file" => new FileDatabase(),
		_ => new MemoryDatabase(),
	};
	
	#endregion // Public Methods
}
