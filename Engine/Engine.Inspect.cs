
namespace DGNet;

using DGNet.Inspector;
using DGNet.Models;

using System.Collections.Generic;

public sealed partial class Engine
{
	#region Public Methods
	
	public bool Inspect()
	{
		this.Phase = Phase.Inspect;
		List<AssemblyTypes> asmTypes = this.Database.Query<AssemblyTypes>(this.Environment.AssembliesToInspect.ToArray());
		
		foreach(AssemblyTypes asmType in asmTypes)
		{
			foreach(string type in asmType.Types)
			{
				TypeInspection inspection = new TypeInspection(type, this);
				
				this.Database.InsertBulk(
					inspection.GetFields(this)
						.ConvertAll(item => (item.XPath, item as Inspection))
						.ToArray(),
					(id, item) => inspection.FieldIDs.Add(id)
				);
				this.Database.InsertBulk(
					inspection.GetProperties(this)
						.ConvertAll(item => (item.XPath, item as Inspection))
						.ToArray(),
					(id, item) => inspection.PropertyIDs.Add(id)
				);
				this.Database.InsertBulk(
					inspection.GetEvents(this)
						.ConvertAll(item => (item.XPath, item as Inspection))
						.ToArray(),
					(id, item) => inspection.EventIDs.Add(id)
				);
				this.Database.InsertBulk(
					inspection.GetMethods(this)
						.ConvertAll(item => (item.XPath, item as Inspection))
						.ToArray(),
					(id, item) => inspection.MethodIDS.Add(id)
				);
				
				this.Database.Insert(inspection.XPath, inspection);
			}
		}
		
		return true;
	}
	
	#endregion // Public Methods
}
