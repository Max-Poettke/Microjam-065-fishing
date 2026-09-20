using UnityEngine;
using DG.Tweening;

public enum TrapType
{
    Catch,
    Kill
}

public class Trap : MonoBehaviour
{
    public TrapType trapType;
    [SerializeField] private ParticleSystem catchParticles;
    [SerializeField] private AudioClip catchSound;
    [SerializeField] private AudioSource audioSource;

    private Fish caughtFish;

    public void Catch(Fish fish)
    {
        if(trapType == TrapType.Kill)
        {
            caughtFish = fish;
            Kill();
            return;
        }

        caughtFish = fish;
        caughtFish.SetCaught(true);
        audioSource.clip = catchSound;
        audioSource.Play();
        catchParticles.Play();
    }

    private void Kill()
    {
        transform.DOPunchScale(Vector3.one * 1.3f, 0.3f);
        audioSource.clip = catchSound;
        audioSource.Play();
        catchParticles.Play();
        EmptyOut(true);
    }

    private void DeleteBoth(bool killed)
    {
        GameMaster.instance.RemoveFish(caughtFish, killed);
        Destroy(gameObject);
    }

    public void EmptyOut(bool killed)
    {
        if(!caughtFish) return;
        transform.DOPunchScale(Vector3.one * 1.3f, 0.3f).OnComplete(()=>{DeleteBoth(killed);});
    }

}
