using UnityEngine;

namespace vnc.AI.Samples
{
    public class Rotator : MonoBehaviour
    {
        public float speed = 6;

        void Update()
        {
            transform.rotation *= Quaternion.Euler(0, Time.deltaTime * speed, 0);
        }
    }
}
