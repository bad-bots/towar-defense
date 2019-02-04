using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceCameraBehavior : MonoBehaviour
{
    public Transform currentCamera;

    [Tooltip("Whether or not to continue updating rotation")]
    public bool tracking;

    // Start is called before the first frame update
    void Start()
    {
        if (currentCamera == null)
            currentCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

        UpdateRotation();
    }

    // Update is called once per frame
    void Update()
    {
        if (tracking) UpdateRotation();
    }

    private void UpdateRotation()
    {
        if (currentCamera != null)
            transform.rotation = Quaternion.LookRotation(transform.position - currentCamera.transform.position);
    }
}
