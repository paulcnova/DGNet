
namespace DGNet;

using Newtonsoft.Json;

using System.IO;

public sealed class Database : System.IDisposable
{
	#region Properties
	
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
		
		return Path.Combine(path, $"{fileName.Replace('/', '.')}.json");
	}
	
	public void Insert<T>(string id, T item)
	{
		string path = this.GetPath<T>(id);
		
		File.WriteAllText(path, JsonConvert.SerializeObject(item));
	}
	
	public void Delete<T>(string id)
	{
		string path = this.GetPath<T>(id);
		
		File.Delete(path);
	}
	
	public T Query<T>(string id)
	{
		string path = this.GetPath<T>(id);
		
		return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
	}
	
	public bool Setup(string path)
	{
		this.DBPath = Path.Combine(path, "db");
		if(Directory.Exists(this.DBPath)) { Directory.Delete(this.DBPath, true); }
		if(!Directory.Exists(this.DBPath)) { Directory.CreateDirectory(this.DBPath); }
		
		return true;
	}
	
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
