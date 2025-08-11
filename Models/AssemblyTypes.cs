
namespace DGNet.Models;

using LiteDB;

using System.Collections.Generic;

public sealed class AssemblyTypes
{
	#region Properties
	
	[BsonId] public string AssemblyName { get; set; }
	public List<string> Types { get; set; }
	
	#endregion // Properties
}
