using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossStopPoint : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    public static BossStopPoint instance;
   
    public static BossStopPoint Instance
    {
        get { return instance; }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }else
        {
            Destroy(this);
        }
        points = GetComponentsInChildren<Transform>();
    }

    public Vector3 GetPoint() => points[Random.Range(0, points.Length)].position;

    
    
}
