
namespace DGNet.Data;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public sealed class FileDatabase : IDatabase
{
	#region Properties
	
	public Dictionary<System.Type, SerializationMap> Serialization { get; set; } = new Dictionary<System.Type, SerializationMap>();
	public string ErrorMessage { get; private set; }
	public string DBPath { get; private set; }
	public bool DeleteOnExit { get; private set; }
	#if DEBUG
		= false;
	#else
		= true;
	#endif // DEBUG
	
	#endregion // Properties
	
	#region Public Methods
	
	public string GetPath<T>(string fileName)
	{
		string path = Path.Combine(this.DBPath, typeof(T).FullName.Replace('.', '/'));
		
		this.EnsurePath(path);
		
		System.Console.WriteLine(fileName);
		
		return Path.Combine(path, $"{Regex.Replace(fileName, @"[\\\/:\?\*""\<\>\|]", "-")}.json");
	}
	
	public void InsertBulk<T>(IEnumerable<(string, T)> items, DatabaseInsertCallback<T> callback)
	{
		foreach((string, T) item in items)
		{
			this.Insert<T>(item.Item1, item.Item2, callback);
		}
	}
	
	public void Insert<T>(string id, T item, DatabaseInsertCallback<T> callback)
	{
		if(string.IsNullOrEmpty(id)) { return; }
		
		string path = this.GetPath<T>(id);
		
		callback?.Invoke(id, item);
		if(this.Serialization.TryGetValue(typeof(T), out SerializationMap map))
		{
			map.Serialize.DynamicInvoke(this, item);
			(map.Serialize as DatabaseSerialization<T>).Invoke(this, id, item);
		}
		try
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(item));
		} catch {}
	}
	
	public void Delete<T>(string id)
	{
		string path = this.GetPath<T>(id);
		
		File.Delete(path);
	}
	
	public T QueryOne<T>(string id)
	{
		string path = this.GetPath<T>(id);
		T item = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
		
		if(this.Serialization.TryGetValue(typeof(T), out SerializationMap map))
		{
			(map.Deserialize as DatabaseSerialization<T>).Invoke(this, id, item);
		}
		
		return item;
	}
	
	public List<T> Query<T>(params string[] ids)
	{
		List<T> list = new List<T>();
		
		foreach(string id in ids)
		{
			list.Add(this.QueryOne<T>(id));
		}
		
		return list;
	}
	
	public bool Setup(string path)
	{
		this.DBPath = Path.Combine(path, "db");
		if(Directory.Exists(this.DBPath)) { Directory.Delete(this.DBPath, true); }
		if(!Directory.Exists(this.DBPath)) { Directory.CreateDirectory(this.DBPath); }
		
		return true;
	}
	
	public string GetErrorMessage() => this.ErrorMessage;
	
	public void Dispose()
	{
		if(this.DeleteOnExit)
		{
			Directory.Delete(this.DBPath);
		}
	}
	
	#endregion // Public Methods
	
	#region Private Methods
	
	private FileStream EnsureFile(string fileName)
	{
		if(File.Exists(fileName)) { return File.Open(fileName, FileMode.Open); }
		return File.Create(fileName);
	}
	
	private void EnsurePath(string path)
	{
		if(Directory.Exists(path)) { return; }
		Directory.CreateDirectory(path);
	}
	
	#endregion // Private Methods
}
