using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform child;
    [SerializeField] Transform playerScaler;
    [SerializeField] Cinemachine.CinemachineFreeLook cam;

    [SerializeField] float scale = 1f;
    float oldScale;
    Vector3 relativeScale;

    private void Start()
    {
        relativeScale = playerScaler.localScale;
        oldScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = child.position;
        position.y = 0f;

        this.transform.position = position;

        if(scale != oldScale)
        {
            oldScale = scale;

            Vector3 scaleVec = scale * relativeScale;

            cam.transform.localScale = scaleVec;
            playerScaler.localScale = scaleVec;
            this.transform.localScale = scaleVec;
        }
    }

    
}
