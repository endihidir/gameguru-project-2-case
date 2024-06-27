using UnityBase.EventBus;
using UnityEngine;

public struct StackSettleData : IEvent
{
    public int stackIndex;
    public SliceCase sliceCase;
    public Vector3 settledPos;
}