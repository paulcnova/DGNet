
namespace DGNet;

using LiteDB;

using System.IO;

public sealed class Database : System.IDisposable
{
	#region Properties
	
	public LiteDatabase DB { get; private set; }
	
	#endregion // Properties
	
	#region Public Methods
	
	public ILiteCollection<T> Collection<T>(string collectionName = null)
		=> string.IsNullOrEmpty(collectionName)
			? this.DB.GetCollection<T>()
			: this.DB.GetCollection<T>(collectionName);
	
	public ILiteQueryable<T> Query<T>(string collectionName = null)
		=> string.IsNullOrEmpty(collectionName)
			? this.DB.GetCollection<T>().Query()
			: this.DB.GetCollection<T>(collectionName).Query();
	
	public bool Connect(string path)
	{
		string fileName = Path.Combine(path, "temp.db");
		
		if(File.Exists(fileName)) { File.Delete(fileName); }
		this.DB = new LiteDatabase(this.GetConnectionString(fileName));
		
		return true;
	}
	
	public void Dispose()
	{
		if(this.DB != null)
		{
			this.DB.Dispose();
		}
	}
	
	#endregion // Public Methods
	
	#region Private Methods
	
	private ConnectionString GetConnectionString(string fileName)
	{
		ConnectionString str = new ConnectionString();
		
		str.Filename = fileName;
		
		return str;
	}
	
	#endregion // Private Methods
}
