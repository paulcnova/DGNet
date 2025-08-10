
namespace DGNet;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public class GenerateDocumentation : Task
{
	#region Properties
	
	public string ProjectName { get; set; }
	public string InputPath { get; set; }
	public string OutputPath { get; set; }
	public string TargetAssemblyNames { get; set; }
	public string Type { get; set; }
	
	#endregion // Properties
	
	#region Public Methods
	
	public override bool Execute()
	{
		System.Console.WriteLine("TODO: Generate documentation!");
		System.Console.WriteLine(this.ProjectName);
		System.Console.WriteLine(this.InputPath);
		System.Console.WriteLine(this.OutputPath);
		System.Console.WriteLine(this.TargetAssemblyNames);
		System.Console.WriteLine(this.Type);
		return true;
	}
	
	#endregion // Public Methods
}
