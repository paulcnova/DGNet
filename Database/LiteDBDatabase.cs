
namespace DGNet.Data;

using LiteDB;

using System.Collections.Generic;
using System.IO;

public sealed class LiteDBDatabase : IDatabase
{
	#region Properties
	
	private string errorMessage;
	
	public Dictionary<System.Type, SerializationMap> Serialization { get; set; } = new Dictionary<System.Type, SerializationMap>();
	public LiteDatabase DB { get; set; }
	
	#endregion // Properties
	
	#region Public Methods
	
	public string GetErrorMessage() => this.errorMessage;
	
	public bool Setup(string path)
	{
		if(File.Exists(Path.Combine(path, "temp.db"))) { File.Delete(Path.Combine(path, "temp.db")); }
		
		this.DB = new LiteDatabase(this.GetConnectionString(path));
		return true;
	}
	
	public void Insert<T>(string id, T item, DatabaseInsertCallback<T> callback)
	{
		callback?.Invoke(id, item);
		try { this.DB.GetCollection<T>().Insert(id, item); } catch {}
	}
	
	public void Delete<T>(string id) => this.DB.GetCollection<T>().Delete(id);
	
	public T QueryOne<T>(string id) => this.DB.GetCollection<T>().FindOne(@$"$._id = ""{id}""");
	
	public void Dispose()
	{
		if(this.DB != null)
		{
			this.DB.Dispose();
		}
	}
	
	#endregion // Public Methods
	
	#region Private Methods
	
	private ConnectionString GetConnectionString(string path)
	{
		ConnectionString connection = new ConnectionString();
		
		connection.Filename = System.IO.Path.Combine(path, "temp.db");
		
		return connection;
	}
	
	#endregion // Private Methods
}
