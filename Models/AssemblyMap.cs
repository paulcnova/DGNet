
namespace DGNet.Models;

using LiteDB;

public sealed class AssemblyMap
{
	#region Properties
	
	[BsonId] public string TypeName { get; set; }
	public string AssemblyName { get; set; }
	
	#endregion // Properties
}
