
namespace DGNet;

using System.Collections.Generic;

public sealed class ProjectEnvironment
{
	#region Properties
	
	public string Type { get; set; }
	public string InputPath { get; set; }
	public List<string> AssembliesToInspect { get; set; } = new List<string>();
	
	public string[] Assemblies { get; set; }
	public string[] XmlFiles { get; set; }
	public bool IgnorePrivate { get; set; }
	
	#endregion // Properties
}
