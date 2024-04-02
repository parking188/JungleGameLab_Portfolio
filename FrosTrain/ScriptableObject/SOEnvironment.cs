using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Environment", menuName = "ScriptableObjects/Environment")]
public class SOEnvironment : ScriptableObject
{
    #region PublicVariables
    public Environment environment;
    public string environmentName; 
    public string informationText;
    public float effectValue;
    public Sprite icon;
    public Color colorAreaTextureSetter;
    public bool isTunnel;

    [Header("Text_Eng")]
    public string environmentName_eng;
    public string informationText_eng;
    #endregion
    #region PrivateVariables
    #endregion
    #region PublicMethod
    #endregion
    #region PrivateMethod
    #endregion
}