
namespace DGNet;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using System.Collections.Generic;
using System.IO;

public class GenerateDocumentation : Task
{
	#region Properties
	
	[Required] public string Type { get; set; }
	[Required] public string InputPath { get; set; }
	[Required] public string OutputPath { get; set; }
	
	public string ProjectName { get; set; }
	public string TargetAssemblyNames { get; set; }
	public string DatabaseType { get; set; } = "";
	public bool IncludePrivate { get; set; } = false;
	
	#endregion // Properties
	
	#region Public Methods
	
	public override bool Execute()
	{
		Logger.LogHelper = this.Log;
		
		ProjectEnvironment environment = this.GetEnvironment();
		
		using(Engine engine = new Engine(environment))
		{
			if(!engine.Init()) { return Logger.Error(engine); }
			if(!engine.Inspect()) { return Logger.Error(engine); }
			if(!engine.Parse()) { return Logger.Error(engine); }
			if(!engine.Render()) { return Logger.Error(engine); }
			if(!engine.Generate()) { return Logger.Error(engine); }
		}
		
		return true;
	}
	
	#endregion // Public Methods
	
	#region Private Methods
	
	private ProjectEnvironment GetEnvironment()
	{
		string[] asmFiles = Directory.GetFiles(this.InputPath, "*.dll", SearchOption.AllDirectories);
		string[] xmlFiles = Directory.GetFiles(this.InputPath, "*.xml", SearchOption.AllDirectories);
		
		return new ProjectEnvironment()
		{
			Type = this.Type,
			InputPath = this.InputPath,
			IgnorePrivate = !this.IncludePrivate,
			AssembliesToInspect = new List<string>() { this.TargetAssemblyNames },
			DatabaseType = this.DatabaseType,
			Assemblies = asmFiles,
			XmlFiles = xmlFiles,
			
			// ProjectName = this.ProjectName,
			// AssemblyPath = this.AssembliesPath,
			// OriginalAssembly = this.OriginalAssembly,
			// Assemblies = new List<string>(asmFiles),
			// XmlFiles = new List<string>(xmlFiles),
			// GeneratorType = this.GeneratorType,
			// OutputDirectory = this.OutputDirectory,
			// IncludePrivate = this.IncludePrivate,
		};
	}
	
	#endregion // Private Methods
}
