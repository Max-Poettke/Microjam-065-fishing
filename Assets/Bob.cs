using UnityEngine;
using DG.Tweening;

public class Bob : MonoBehaviour
{
    [SerializeField] private bool rotate = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOPunchScale(Vector3.one * 0.02f, Random.Range(0.1f, 1f)).OnComplete(() => {bobUpAndDown();});
    }

    private void bobUpAndDown()
    {
        if(rotate) transform.DORotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), 10);
        transform.DOPunchPosition(Vector3.up * Random.Range(0.1f, 0.18f), 10, 1).OnComplete(()=> {bobUpAndDown();});
    }
}
