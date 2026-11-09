using UnityEngine;

public class InputGate : IMoveInput, IActionInput
{
    private readonly IMoveInput moveSource;
    private readonly IActionInput actionSource;
    private int lockCount;

    public InputGate(IMoveInput moveSource, IActionInput actionSource)
    {
        this.moveSource = moveSource;
        this.actionSource = actionSource;
    }

    public bool IsLocked => lockCount > 0;

    public void Lock() => lockCount++;
    public void Unlock() => lockCount = Mathf.Max(0, lockCount - 1);
    public void ResetLock() => lockCount = 0;

    public Vector2 Move => IsLocked ? Vector2.zero : (moveSource != null ? moveSource.Move : Vector2.zero);
    public bool IsRunning => !IsLocked && moveSource != null && moveSource.IsRunning;
    public bool InteractPressed => !IsLocked && actionSource != null && actionSource.InteractPressed;
}
