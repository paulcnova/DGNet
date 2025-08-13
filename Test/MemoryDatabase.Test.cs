
namespace DGNet.Test;

using DGNet;
using DGNet.Data;
using DGNet.Models;

using NUnit.Framework;

public class MemoryDatabaseTests
{
	#region Properties
	
	public IDatabase Database { get; set; }
	
	[SetUp] public void Setup()
	{
		this.Database = new MemoryDatabase();
	}
	
	[TearDown] public void TearDown()
	{
		this.Database.Dispose();
	}
	
	#endregion // Properties
	
	#region Unit Tests
	
	[Test] public void InsertDatabase()
	{
		this.Database.Insert("test", new AssemblyMap() { TypeName = "DGNet.Test.MemoryDatabaseTests", AssemblyName = "test.dll", });
		Assert.Pass();
	}
	
	[Test] public void QueryOneDatabase()
	{
		this.InsertDatabase();
		
		AssemblyMap map = this.Database.QueryOne<AssemblyMap>("test");
		
		Assert.Equals(map.TypeName, "DGNet.Test.MemoryDatabaseTests");
		Assert.Equals(map.AssemblyName, "test.dll");
	}
	
	#endregion // Unit Tests
}
