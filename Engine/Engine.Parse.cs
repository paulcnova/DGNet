
namespace DGNet;

using DGNet.Inspector;
using DGNet.Models;

using System.Collections.Generic;
using System.Xml;

public sealed partial class Engine
{
	#region Public Methods
	
	public bool Parse()
	{
		this.Phase = Phase.Parse;
		XmlDocument document = new XmlDocument();
		List<string> inherits = new List<string>();
		
		foreach(string file in this.Environment.XmlFiles)
		{
			document.Load(file);
			foreach(XmlElement member in document.GetElementsByTagName("member"))
			{
				XContent content = new XContent();
				
				content.XPath = member.GetAttribute("name");
				if(member.ChildNodes.Count == 1 && member.ChildNodes[0].Name == "inheritdoc")
				{
					XmlElement elem = member.ChildNodes[0] as XmlElement;
					
					content.InheritPath = elem.HasAttribute("cref")
						? elem.GetAttribute("cref")
						: elem.HasAttribute("href")
							? elem.GetAttribute("href")
							: this.GetInheritPath(content.XPath);
					inherits.Add(content.XPath);
				}
				else
				{
					content.Content = member.InnerXml;
				}
				
				this.Database.Insert<XContent>(content.XPath, content);
			}
		}
		
		foreach(string inherit in inherits)
		{
			XContent content = this.Database.QueryOne<XContent>(inherit);
			XContent nested = this.Database.QueryOne<XContent>(inherit);
			
			while(!string.IsNullOrEmpty(nested.InheritPath))
			{
				nested = this.Database.QueryOne<XContent>(nested.InheritPath);
			}
			
			content.Content = nested.Content;
		}
		
		return true;
	}
	
	#endregion // Public Methods
	
	#region Private Methods
	
	private string GetInheritPath(string typePath)
	{
		return typePath;
		// TODO: When adding a type, figure out how to place a clean path
		// Inspection type = this.Database.QueryOne<Inspection>(typePath);
		
		// type = this.Database.QueryOne<Inspection>($"T:{type.ImplementedType.UnlocalizedName}");
		// type = this.Database.QueryOne<Inspection>($"T:{type.BaseType.UnlocalizedName}");
		
		// return type.XPath;
	}
	
	#endregion // Private Methods
}
