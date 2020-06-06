using UnityEngine;
using System.Collections;

public class AnchorConnector:MonoBehaviour
{


	[GenerationTagsEnum]
	public string[] Tags;

	//public string[] Tags;
	public	GameObject[] PossibleConnections;
	public bool IsConnected;


	public int AnchorGenerationLevel = 1;

	public bool NeedToBeConnected;
	//public  Element DefaltElement;

	public Color GizmoColor= Color.yellow;


	public float radiuse = 0.4f;
	public float scale=1;
	void OnDrawGizmos()
	{
		//	var scale = 1.0f;

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * scale);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position - transform.right * scale);
		Gizmos.DrawLine(transform.position, transform.position + transform.right * scale);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + Vector3.up * scale);

		Gizmos.color = GizmoColor;
		Gizmos.DrawSphere(transform.position, radiuse);
	}

}
