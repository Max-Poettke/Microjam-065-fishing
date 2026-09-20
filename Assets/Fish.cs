using UnityEngine;
using DG.Tweening;

public class Fish : MonoBehaviour
{

    public enum FishType
    {
        CFish,
        LFish,
        Shark
    }

    [SerializeField] private Transform meshTransform; //used for bounciness in animation
    [SerializeField] private AudioClip moveClip;
    [SerializeField] private AudioSource audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
