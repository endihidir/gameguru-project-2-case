using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stack/StackConfig")]
public class StackConfigSO : ScriptableObject
{
    public float movementDuration;

    public int stackCount;

    public Vector3 stackSize = new Vector3(3f, 0.5f, 3f);
    
    public ColorSO[] stacks;
    
    [Button]
    public void ResizeArray() => Array.Resize(ref stacks, stackCount);
}