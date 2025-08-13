
namespace DGNet.Data;

using DGNet.Inspector;
using DGNet.Models;

using System.Collections.Generic;

public sealed class MemoryDatabase : IDatabase
{
	#region Properties
	
	private string errorMessage;
	
	public Dictionary<System.Type, SerializationMap> Serialization { get; set; } = new Dictionary<System.Type, SerializationMap>();
	public Dictionary<string, AssemblyMap> Map { get; set; } = new Dictionary<string, AssemblyMap>();
	public Dictionary<string, AssemblyTypes> Types { get; set; } = new Dictionary<string, AssemblyTypes>();
	public Dictionary<string, Inspection> Inspections { get; set; } = new Dictionary<string, Inspection>();
	
	#endregion // Properties
	
	#region Public Methods
	
	public string GetErrorMessage() => this.errorMessage;
	
	public bool Setup(string path) => true;
	
	public void Insert<T>(string id, T item, DatabaseInsertCallback<T> callback)
	{
		if(item is Inspection inspection)
		{
			callback?.Invoke(id, item);
			this.Inspections.TryAdd(id, inspection);
		}
		else if(item is AssemblyTypes types)
		{
			callback?.Invoke(id, item);
			this.Types.TryAdd(id, types);
		}
		else if(item is AssemblyMap map)
		{
			callback?.Invoke(id, item);
			this.Map.TryAdd(id, map);
		}
	}
	
	public void Delete<T>(string id)
	{
		if(typeof(T) == typeof(Inspection))
		{
			this.Inspections.Remove(id);
		}
		else if(typeof(T) == typeof(AssemblyTypes))
		{
			this.Types.Remove(id);
		}
		else if(typeof(T) == typeof(AssemblyMap))
		{
			this.Map.Remove(id);
		}
	}
	
	public T QueryOne<T>(string id)
	{
		if(typeof(T) == typeof(AssemblyMap) && this.Map.TryGetValue(id, out AssemblyMap map)) { return (T)(object)map; }
		if(typeof(T) == typeof(AssemblyTypes) && this.Types.TryGetValue(id, out AssemblyTypes types)) { return (T)(object)types; }
		if(typeof(T) == typeof(Inspection) && this.Inspections.TryGetValue(id, out Inspection inspection)) { return (T)((object)inspection); }
		
		return default;
	}
	
	public void Dispose()
	{
		this.Serialization.Clear();
		this.Inspections.Clear();
		this.Map.Clear();
		this.Types.Clear();
	}
	
	#endregion // Public Methods
}
