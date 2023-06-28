using UnityEngine;

public class SetDisplayNameAttribute : PropertyAttribute
{
	public string NewName { get; private set; }
	public SetDisplayNameAttribute(string name)
	{
		NewName = name;
	}
}