
namespace DGNet.Data;

using DGNet.Inspector;

using System.Collections.Generic;

public sealed class MemoryDatabase : IDatabase
{
	#region Properties
	
	private string errorMessage;
	
	public Dictionary<System.Type, SerializationMap> Serialization { get; set; } = new Dictionary<System.Type, SerializationMap>();
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
			this.Inspections.Add(id, inspection);
		}
	}
	
	public void Delete<T>(string id)
	{
		if(typeof(T) == typeof(Inspection))
		{
			this.Inspections.Remove(id);
		}
	}
	
	public T QueryOne<T>(string id)
	{
		if(typeof(T) == typeof(Inspection) && this.Inspections.TryGetValue(id, out Inspection result))
		{
			return (T)((object)result);
		}
		return default;
	}
	
	public void Dispose()
	{
		this.Serialization.Clear();
		this.Inspections.Clear();
	}
	
	#endregion // Public Methods
}
