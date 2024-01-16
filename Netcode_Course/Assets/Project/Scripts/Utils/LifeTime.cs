using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    private void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }
}
