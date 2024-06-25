using UnityEngine;

public class StackInitializer : IStackInitializer
{
    public readonly IStackInitEntity _stackInitEntity;
    
    private MaterialPropertyBlock _materialPropertyBlock = new();
    public StackInitializer(IStackInitEntity stackInitEntity) => _stackInitEntity = stackInitEntity;

    public Vector3 GetPos() => _stackInitEntity.StackTransform.position;
    public void SetPos(Vector3 pos) => _stackInitEntity.StackTransform.position = pos;

    public Vector3 GetScale() => _stackInitEntity.StackMeshTransform.localScale;
    public void SetScale(Vector3 scale) => _stackInitEntity.StackMeshTransform.localScale = scale;

    public void SetColor(Color color)
    {
        _materialPropertyBlock.SetColor("_BaseColor", color);
        _stackInitEntity.StackMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        _stackInitEntity.StackPieceMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    public void Dispose()
    {
        
    }
}

public interface IStackInitializer
{
    public Vector3 GetPos();
    public void SetPos(Vector3 pos);
    public Vector3 GetScale();
    public void SetScale(Vector3 scale);
    public void SetColor(Color color);
    public void Dispose();
}

public interface IStackInitEntity
{
    public Transform StackTransform { get; }
    public Transform StackMeshTransform { get; }
    public Transform PieceTransform { get; }
    public MeshRenderer StackMeshRenderer { get; }
    public MeshRenderer StackPieceMeshRenderer { get; }
}