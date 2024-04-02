using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Text", fileName = "New Text")]
public class SOText : ScriptableObject
{
	#region PublicVariables
	[TextArea]
	public string text;

    [TextArea]
    public string text_eng;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public SOText Clone()
	{
		return Instantiate(this);
	}
	#endregion

	#region PrivateMethod
	#endregion
}
