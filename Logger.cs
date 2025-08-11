
namespace DGNet;

using Microsoft.Build.Utilities;

public static class Logger
{
	#region Properties
	
	public static TaskLoggingHelper LogHelper { get; set; }
	
	#endregion // Properties
	
	#region Public Methods
	
	public static void LogError(string str)
	{
		if(LogHelper == null)
		{
			System.Console.WriteLine(str);
		}
		else
		{
			LogHelper.LogError(str);
		}
	}
	
	public static bool Error(Engine engine)
	{
		LogError(engine.Phase switch {
			Phase.Init => "Could not initialize the documentation engine.",
			Phase.Inspect => "Could not inspect the code.",
			Phase.Parse => "Could not parse the documentation.",
			Phase.Render => "Could not render the data.",
			Phase.Generate => "Could not generate rendered data.",
			_ => "Unknown error.",
		});
		LogError(engine.GetErrorMessage());
		return false;
	}
	
	#endregion // Public Methods
}
