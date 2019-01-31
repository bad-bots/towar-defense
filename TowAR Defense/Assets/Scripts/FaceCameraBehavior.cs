using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraBehavior : MonoBehaviour
{
    public Transform currentCamera;
    public GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - currentCamera.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
