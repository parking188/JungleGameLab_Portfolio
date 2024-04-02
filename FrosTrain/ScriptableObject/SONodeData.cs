using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeData", menuName = "ScriptableObjects/NodeData")]
public class SONodeData : ScriptableObject
{
    public SOEventData normalNodeEvent;
    public List<SOEventData> middleNodeEvent = new List<SOEventData>();
    public List<SOEventData> landMarkEvent = new List<SOEventData>();
    public Sprite endNodeIcon;
}