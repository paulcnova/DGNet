
namespace DGNet;

using Npgsql;

using System.IO;

public sealed class Database : System.IDisposable
{
	#region Properties
	
	public string ErrorMessage { get; private set; }
	
	public NpgsqlDataSource Source { get; private set; }
	public NpgsqlConnection Connection { get; private set; }
	
	#endregion // Properties
	
	#region Public Methods
	
	public bool TryConnect(string path)
	{
		try
		{
			string fileName = Path.Combine(path, "temp.db");
			
			if(File.Exists(fileName)) { File.Delete(fileName); }
			
			NpgsqlConnectionStringBuilder connectionStr = this.GetConnectionString(fileName);
			NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(connectionStr.ConnectionString);
			
			this.Source = builder.UseJsonNet().Build();
			this.Connection = this.Source.OpenConnection();
		}
		catch(System.Exception e)
		{
			this.ErrorMessage = e.ToString();
			return false;
		}
		
		return true;
	}
	
	public void Dispose()
	{
		if(this.Source != null) { this.Source.Dispose(); }
		if(this.Connection != null) { this.Connection.Dispose(); }
	}
	
	#endregion // Public Methods
	
	#region Private Methods
	
	private NpgsqlConnectionStringBuilder GetConnectionString(string fileName)
	{
		NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
		
		builder.Database = "postgres";
		builder.Host = "localhost";
		
		return builder;
	}
	
	#endregion // Private Methods
}
