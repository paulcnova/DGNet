
namespace DGNet.Data;

using System.Collections.Generic;

public interface IDatabase : System.IDisposable
{
	#region Public Methods
	
	string GetErrorMessage();
	bool Setup(string path);
	void Insert<T>(string id, T item);
	void InsertBulk<T>(params (string, T)[] items);
	void Delete<T>(string id);
	void DeleteBulk<T>(params string[] ids);
	T QueryOne<T>(string id);
	List<T> Query<T>(params string[] ids);
	
	#endregion // Public Methods
}
