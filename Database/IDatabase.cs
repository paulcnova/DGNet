
namespace DGNet.Data;

using System.Collections.Generic;

public delegate void DatabaseInsertCallback<T>(string id, T item);

public interface IDatabase : System.IDisposable
{
	#region Public Methods
	
	string GetErrorMessage();
	bool Setup(string path);
	
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
