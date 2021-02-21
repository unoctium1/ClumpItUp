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

    private void Start()
    {
        oldScale = playerScaler.localScale.x;
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

            Vector3 scaleVec = scale * Vector3.one;

            cam.transform.localScale = scaleVec;
            playerScaler.localScale = scaleVec;
            this.transform.localScale = scaleVec;
        }
    }

    
}
