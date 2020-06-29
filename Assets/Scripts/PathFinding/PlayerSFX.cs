using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private AudioClip[] _metalClashClips;
    [SerializeField] private AudioClip[] _dashClips;
    [SerializeField] private AudioClip[] _slashClips;
    [SerializeField] private AudioClip[] _enemyDeathSFXClips;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        Mover.OnFirstMove += PlayNormalDash;
        UnitKillHandler.UnitKillPointReached += PlaySlashDash;
        UnitKillHandler.OnAnyEnemyDeath += PlayEnemyDeath;
        MovementPackage.OnCollisionWithEnemyWhileMovingThroughIntersection += PlayMetalClash;
    }

    private void OnDestroy()
    {
        Mover.OnFirstMove -= PlayNormalDash;
        UnitKillHandler.UnitKillPointReached -= PlaySlashDash;
        UnitKillHandler.OnAnyEnemyDeath -= PlayEnemyDeath;
        MovementPackage.OnCollisionWithEnemyWhileMovingThroughIntersection -= PlayMetalClash;
    }
    
    private void PlayMetalClash()
    {
        var metalClashSFX = _metalClashClips[Random.Range(0, _metalClashClips.Length - 1)];
        PlaySFX(metalClashSFX);
    }

    
    private void PlayNormalDash()
    {
        var dashSFX = _dashClips[Random.Range(0, _dashClips.Length - 1)];
        PlaySFX(dashSFX);
    } 
    
    private void PlaySlashDash()
    {
        var slashSFX = _slashClips[Random.Range(0, _slashClips.Length - 1)];
        PlaySFX(slashSFX);
    }

    private void PlayEnemyDeath()
    {
        // var enemyDeathSFX = _enemyDeathSFXClips[Random.Range(0, _enemyDeathSFXClips.Length - 1)];
        // PlaySFX(enemyDeathSFX);
    }

    private void PlaySFX(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip);
    }
}