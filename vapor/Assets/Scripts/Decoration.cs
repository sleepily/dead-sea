using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    public GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Beat()
    {
        Move();
    }

    void Move()
    {
        Vector3 pos = transform.position;

        if (pos.z < gm.playerZ)
            pos.z -= gm.stepZ * 8;
        else
            pos.z += gm.stepZ;

        transform.position = pos;
    }
}
