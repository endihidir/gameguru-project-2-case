
public class StackBehaviour : IStackBehaviour
{
    public int Index { get; }
    public bool IsConstructed { get; set; }
    public IStackInitializer StackInitializer { get; set; }
    public IStackSliceController StackSliceController { get; set; }
    public IStackAnimationController StackAnimationController { get; set; }
    public StackBehaviour(int index) => Index = index;

    public void Reset()
    {
        IsConstructed = false;
        StackInitializer?.Reset();
        StackSliceController?.Reset();
        StackAnimationController?.Reset();
    }

    public void Dispose()
    {
        IsConstructed = false;
        StackInitializer?.Dispose();
        StackSliceController?.Dispose();
        StackAnimationController?.Dispose();
    }
}

public interface IStackBehaviour
{
    public int Index { get; }
    public bool IsConstructed { get; set; }
    public IStackInitializer StackInitializer { get; set; }
    public IStackSliceController StackSliceController { get; set; }
    public IStackAnimationController StackAnimationController { get; set; }
    public void Reset();
    public void Dispose();
}