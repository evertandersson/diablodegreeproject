using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class PlayerMovement : MonoBehaviour
    {
        public void SetDestination(Vector3 destinationPosition)
        {
            PlayerManager.Instance.Agent.isStopped = false;
            PlayerManager.Instance.Agent.SetDestination(destinationPosition);
        }
    }
}
