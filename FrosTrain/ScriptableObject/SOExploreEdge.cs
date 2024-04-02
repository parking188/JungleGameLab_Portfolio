using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Explore/ExploreRoute", fileName ="New Explore Route")]
public class SOExploreEdge : ScriptableObject
{
	#region PublicVariables
	public SOExploreNode a;
	public SOExploreNode b;
	public Environment environment;
	public float environmentValue;
	public float distance;
	public float arrivalTime;
	#endregion

	#region PrivateVariables
	#endregion

	#region PublicMethod
	#endregion

	#region PrivateMethod
	#endregion
}

