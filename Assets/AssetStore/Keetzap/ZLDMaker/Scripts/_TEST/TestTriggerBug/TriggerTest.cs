using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class TriggerTest : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            //if (other.CompareTag("Player") || (other.CompareTag("Draggable")))
            {
                Debug.Log("ENTER --> " + other.gameObject.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //if (other.CompareTag("Player") || (other.CompareTag("Draggable")))
            {
                Debug.Log("EXIT --> " + other.gameObject.name);
               
            }
        }
    }   
}
