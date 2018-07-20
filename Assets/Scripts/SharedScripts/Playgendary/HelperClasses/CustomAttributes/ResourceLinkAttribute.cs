using UnityEngine;

public class ResourceLinkAttribute : PropertyAttribute {

	public System.Type resourceType;


	public ResourceLinkAttribute() : this(typeof(GameObject)){}
	public ResourceLinkAttribute(System.Type resourceType)
	{
		this.resourceType = resourceType;
	}
}
