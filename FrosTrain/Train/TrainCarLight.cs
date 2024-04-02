using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TrainCarLight : MonoBehaviour
{
	#region PublicVariables
	public Light2D spriteLight;
	public Light2D spotLight;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    #endregion

    #region PrivateMethod
    private void Awake()
    {
        if (spriteLight == null) spriteLight = transform.Find("Sprite Light 2D").GetComponent<Light2D>();
        if (spotLight == null) spotLight = transform.Find("Spot Light 2D").GetComponent<Light2D>();
    }
    #endregion
}
