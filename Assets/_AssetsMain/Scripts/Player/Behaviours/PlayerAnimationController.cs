using UnityEngine;

public class PlayerAnimationController : IPlayerAnimationController
{
    private const string RUN = "Run";
    private const string RUN_SPEED_MULTIPLIER = "RunSpeedMultiplier";
    private const string DANCE = "Dance";
    
    private readonly IAnimationEntity _animationEntity;
    private readonly int _runSpeedMultiplier = Animator.StringToHash(RUN_SPEED_MULTIPLIER);
    
    public PlayerAnimationController(IAnimationEntity animationEntity) => _animationEntity = animationEntity;
    public void PlayRunAnim(float animSpeedMultiplier)
    {
        _animationEntity.Animator.Play(RUN);
        _animationEntity.Animator.SetFloat(_runSpeedMultiplier, animSpeedMultiplier);
    }
    public void PlayDanceAnim() => _animationEntity.Animator.Play(DANCE);

    public void Dispose()
    {
        
    }
}

public interface IPlayerAnimationController
{
    public void PlayDanceAnim();
    public void PlayRunAnim(float animSpeedMultiplier);
    public void Dispose();
}

public interface IAnimationEntity
{
    public Animator Animator { get; }
}
