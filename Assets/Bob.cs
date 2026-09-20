using UnityEngine;
using DG.Tweening;

public class Bob : MonoBehaviour
{
    [SerializeField] private bool rotate = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bobUpAndDown();   
    }

    private void bobUpAndDown()
    {
        if(rotate) transform.DORotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), 10);
        transform.DOPunchPosition(Vector3.up * 0.1f, 10, 1).OnComplete(()=> {bobUpAndDown();});
    }
}
