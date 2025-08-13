
namespace DGNet;

using Microsoft.Build.Utilities;

public static class Logger
{
	#region Properties
	
	public static TaskLoggingHelper LogHelper { get; set; }
	
	#endregion // Properties
	
	#region Public Methods
	
	public static void Write(object obj)
	{
		if(LogHelper == null)
		{
			System.Console.WriteLine(obj);
		}
		else
		{
			System.Console.WriteLine(obj);
		}
	}
	
	public static void LogError(object obj)
	{
		if(LogHelper == null)
		{
			System.Console.WriteLine(obj);
		}
		else
		{
			LogHelper.LogError($"{obj}");
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
