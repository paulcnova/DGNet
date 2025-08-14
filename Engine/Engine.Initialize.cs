
namespace DGNet;

using DGNet.Inspector;

public sealed partial class Engine
{
	#region Public Methods
	
	public bool Init()
	{
		this.Phase = Phase.Init;
		this.Database = this.Environment.CreateDatabase();
		if(!this.Database.Setup(this.Environment.InputPath))
		{
			this.errorMessage = $"Could not connect to database.\n{this.Database.GetErrorMessage()}";
			return false;
		}
		this.Database.Serialize<Inspection>(
			(db, id, item) => {
				if(item is PropertyInspection prop)
				{
					if(prop.HasGetter)
					{
						// TODO: Have this be done on the actual class.
						prop.GetterID = prop.Getter.XPath;
						db.Insert(prop.Getter.XPath, prop.Getter);
					}
					if(prop.HasSetter)
					{
						// TODO: Have this be done on the actual class.
						prop.SetterID = prop.Setter.XPath;
						db.Insert(prop.Setter.XPath, prop.Setter);
					}
				}
				if(item is EventInspection ev)
				{
					if(ev.HasAdder)
					{
						// TODO: Have this be done on the actual class.
						ev.AdderID = ev.Adder.XPath;
						db.Insert(ev.Adder.XPath, ev.Adder);
					}
					if(ev.HasRemover)
					{
						// TODO: Have this be done on the actual class.
						ev.RemoverID = ev.Remover.XPath;
						db.Insert(ev.Remover.XPath, ev.Remover);
					}
				}
			},
			(db, id, item) => {
				if(item is PropertyInspection prop)
				{
					if(prop.HasGetter)
					{
						prop.Getter = db.QueryOne<Inspection>(prop.GetterID);
					}
					if(prop.HasSetter)
					{
						prop.Setter = db.QueryOne<Inspection>(prop.SetterID);;
					}
				}
				if(item is EventInspection ev)
				{
					if(ev.HasAdder)
					{
						ev.Adder = db.QueryOne<Inspection>(ev.AdderID);
					}
					if(ev.HasRemover)
					{
						ev.Remover = db.QueryOne<Inspection>(ev.AdderID);
					}
				}
			}
		);
		this.SetupData();
		return true;
	}
	
	#endregion // Public Methods
}
