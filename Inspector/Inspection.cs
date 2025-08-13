
namespace DGNet.Inspector;

using System.Collections.Generic;

public abstract class Inspection
{
	#region Properties
	
	/// <summary>The path used for XML documentation.</summary>
	public string XPath { get; set; }
	
	/// <summary>The path used for database indexing.</summary>
	public string DPath { get; set; }
	
	/// <summary>The accessor of the method (such as internal, private, protected, public)</summary>
	public string Accessor { get; set; }
	
	/// <summary>Any modifiers of the method (such as static, virtual, override, etc.)</summary>
	public string Modifier { get; set; }
	
	/// <summary>Set to true if the method is abstract</summary>
	public bool IsAbstract { get; set; }
	
	/// <summary>Set to true if the method is a constructor</summary>
	public bool IsConstructor { get; set; }
	
	/// <summary>Set to true if the method is a conversion operator</summary>
	public bool IsConversionOperator { get; set; }
	
	/// <summary>Set to true if the method is an extension</summary>
	public bool IsExtension { get; set; }
	
	/// <summary>Set to true if the method is an operator</summary>
	public bool IsOperator { get; set; }
	
	/// <summary>Set to true if the method is overridden</summary>
	public bool IsOverridden { get; set; }
	
	/// <summary>Set to true if the method is static</summary>
	public bool IsStatic { get; set; }
	
	/// <summary>Set to true if the method is virtual</summary>
	public bool IsVirtual { get; set; }
	
	/// <summary>The type that the method is implemented in</summary>
	public QuickTypeInspection ImplementedType { get; set; }
	
	/// <summary>Gets and sets the type information of the field's type</summary>
	public QuickTypeInspection TypeInfo { get; set; }
	
	/// <summary>The type that the method returns</summary>
	public QuickTypeInspection ReturnType { get; set; }
	
	/// <summary>The attributes of the methods</summary>
	public List<AttributeInspection> Attributes { get; set; } = new List<AttributeInspection>();
	
	/// <summary>The parameters that the methods contains</summary>
	public List<ParameterInspection> Parameters { get; set; } = new List<ParameterInspection>();
	
	/// <summary>The generic parameters that the method uses</summary>
	public List<GenericParameterInspection> GenericParameters { get; set; } = new List<GenericParameterInspection>();
	
	/// <summary>The partial declaration of the method (without parameters) that can be found in the code</summary>
	public string Declaration { get; set; }
	
	/// <summary>The partial declaration of the generics that can be found in the code</summary>
	public string GenericDeclaration { get; set; }
	
	/// <summary>The partial declaration of the parameters that can be found in the code</summary>
	public string ParameterDeclaration { get; set; }
	
	/// <summary>The full declaration of the method that can be found in the code</summary>
	public string FullDeclaration { get; set; }
	
	/// <summary>Tells if the method is a property, used to remove it when it's irrelevant</summary>
	public bool IsProperty { get; set; }
	
	/// <summary>Tells if the method is an event, used to remove it when it's irrelevant</summary>
	public bool IsEvent { get; set; }
	
	/// <summary>Set to true if the method should be deleted / ignored.</summary>
	public bool ShouldIgnore { get; set; } = false;
	
	/// <summary>Gets and sets the name of the field</summary>
	public string Name { get; set; }
	
	/// <summary>Gets and sets the value of the field (if it's a constant)</summary>
	public string Value { get; set; }
	
	/// <summary>Gets and sets if the field is constant</summary>
	public bool IsConstant { get; set; }
	
	/// <summary>Gets and sets if the field is readonly</summary>
	public bool IsReadonly { get; set; }
	
	/// <summary>Set to true if the property has a getter method</summary>
	public bool HasAdder { get; set; }
	
	/// <summary>Set to true if the property has a setter method</summary>
	public bool HasRemover { get; set; }
	
	public string AdderID { get; set; }
	
	public string RemoverID { get; set; }
	
	/// <summary>Set to true if the property has a getter method</summary>
	public bool HasGetter { get; set; }
	
	/// <summary>Set to true if the property has a setter method</summary>
	public bool HasSetter { get; set; }
	
	public string GetterID { get; set; }
	
	public string SetterID { get; set; }
	
	/// <summary>The partial declaration of the property that determines the accessibility of the get and set methods as can be found in the code</summary>
	public string GetSetDeclaration { get; set; }
	
	/// <summary>Used to find duplicates.</summary>
	public string PartialFullName { get; set; }
	
	/// <summary>The name of the assembly where the type is found in</summary>
	public string AssemblyName { get; set; }
	
	/// <summary>Set to true if the type is a delegate declaration</summary>
	public bool IsDelegate { get; set; }
	
	/// <summary>Set to true if the type is a nested type</summary>
	public bool IsNested { get; set; }
	
	/// <summary>Set to true if the type is sealed and cannot be inherited from</summary>
	public bool IsSealed { get; set; }
	
	/// <summary>The object type of the type (such as class, struct, enum, or interface)</summary>
	public string ObjectType { get; set; }
	
	/// <summary>Set to true if the type is nested and has a parent type</summary>
	public bool HasDeclaringType { get; set; }
	
	/// <summary>The information of the base type that the type inherits</summary>
	public QuickTypeInspection BaseType { get; set; }
	
	/// <summary>The array of type information of interfaces that the type implements</summary>
	public List<QuickTypeInspection> Interfaces { get; set; } = new List<QuickTypeInspection>();
	
	public List<string> FieldIDs { get; set; } = new List<string>();
	public List<string> PropertyIDs { get; set; } = new List<string>();
	public List<string> EventIDs { get; set; } = new List<string>();
	public List<string> MethodIDS { get; set; } = new List<string>();
	
	#endregion // Properties
	
	#region Public Methods
	
	public abstract string GetXmlNameID();
	
	/// <summary>Compares the info with other infos for sorting</summary>
	/// <param name="other">The other object to look into</param>
	/// <returns>Returns a number that finds if it should be shifted or not (-1 and 0 for no shift; 1 for shift)</returns>
	public int CompareTo(object other)
	{
		if(other is TypeInspection)
		{
			return (this as TypeInspection).TypeInfo.Name.CompareTo((other as TypeInspection).TypeInfo.Name);
		}
		if(other is FieldInspection)
		{
			return (this as FieldInspection).Name.CompareTo((other as FieldInspection).Name);
		}
		if(other is PropertyInspection)
		{
			return (this as PropertyInspection).Name.CompareTo((other as PropertyInspection).Name);
		}
		if(other is MethodInspection)
		{
			return (this as MethodInspection).Name.CompareTo((other as MethodInspection).Name);
		}
		if(other is EventInspection)
		{
			return (this as EventInspection).Name.CompareTo((other as EventInspection).Name);
		}
		return 0;
	}
	
	#endregion // Public Methods
}
