using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Katamari
{
    public class ResetLegoBlock : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out LegoInPlayerBall block))
            {
                Debug.Log("Catching a block");
                block.transform.localPosition = Vector3.zero;
            }
        }
    }
}
