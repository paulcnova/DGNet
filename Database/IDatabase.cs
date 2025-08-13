
namespace DGNet.Data;

using System.Collections.Generic;

public delegate void DatabaseInsertCallback<T>(string id, T item);
public delegate void DatabaseSerialization<T>(IDatabase database, string id, T item);

public sealed class SerializationMap
{
	#region Properties
	
	public System.MulticastDelegate Serialize { get; set; }
	public System.MulticastDelegate Deserialize { get; set; }
	
	public SerializationMap(System.MulticastDelegate serialize, System.MulticastDelegate deserialize)
	{
		this.Serialize = serialize;
		this.Deserialize = deserialize;
	}
	
	#endregion // Properties
}

public interface IDatabase : System.IDisposable
{
	#region Properties
	
	Dictionary<System.Type, SerializationMap> Serialization { get; set; }
	
	#endregion // Properties
	
	#region Public Methods
	
	string GetErrorMessage();
	bool Setup(string path);
	void Serialize<T>(System.Action<IDatabase, string, T> serialize, System.Action<IDatabase, string, T> deserialize)
	{
		System.Type type = typeof(T);
		
		if(!this.Serialization.TryAdd(type, new SerializationMap(serialize, deserialize )))
		{
			this.Serialization[type] = new SerializationMap(serialize, deserialize);
		}
	}
	
	void Insert<T>(string id, T item) => this.Insert<T>(id, item, null);
	void Insert<T>(string id, T item, DatabaseInsertCallback<T> callback);
	
	void InsertBulk<T>(params (string, T)[] items) => this.InsertBulk<T>(items, null);
	void InsertBulk<T>(IEnumerable<(string, T)> items) => this.InsertBulk<T>(items, null);
	void InsertBulk<T>(IEnumerable<(string, T)> items, DatabaseInsertCallback<T> callback);
	
	void Delete<T>(string id);
	void DeleteBulk<T>(params string[] ids)
	{
		foreach(string id in ids)
		{
			this.Delete<T>(id);
		}
	}
	
	T QueryOne<T>(string id);
	List<T> Query<T>(params string[] ids);
	
	#endregion // Public Methods
}
