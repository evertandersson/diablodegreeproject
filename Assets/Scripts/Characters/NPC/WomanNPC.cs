using System.Collections;
using UnityEngine;

namespace Game
{
    public class WomanNPC : NPC, Interactable
    {
        public Dialouge dialouge;
        public Dialouge dialougeAfterBossKill;

        private bool followPlayer;

        private DoorNPC doorToOpen;
        private bool isOpeningDoor = false;

        private Quaternion originalRotation;

        private float rotateSpeed = 2f;

        protected override void Start()
        {
            Initialize();
            originalRotation = transform.rotation;
            CharacterAnimator = GetComponent<Animator>();
        }

        public Vector3 GetCenterPoint()
        {
            return transform.position;
        }

        public void Trigger()
        {
            if (GolemBoss.isGolemKilled)
            {
                DialougeManager.Instance.StartDialouge(dialougeAfterBossKill, this, OnDialogueComplete);
                return;
            }

            DialougeManager.Instance.StartDialouge(dialouge, this);
        }

        private void OnDialogueComplete()
        {
            followPlayer = true;
        }

        private void Update()
        {
            SetFloatRunSpeed();

            // Handle follow player logic here
            if (followPlayer)
            {
                HandleMovement();
                return;
            }
            if (doorToOpen != null)
            {
                if (Vector3.Distance(transform.position, doorToOpen.transform.position) < 2f && isOpeningDoor)
                {
                    isOpeningDoor = false;
                    StartCoroutine(OpenDoor());
                }
            }

            // Handle talk to NPC logic here
            if (DialougeManager.Instance.currentNPC == this)
            {
                Vector3 direction = (PlayerManager.Instance.transform.position - transform.position).normalized;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, rotateSpeed * Time.deltaTime);
            }
        }

        private void HandleMovement()
        {
            if (Vector3.Distance(transform.position, PlayerManager.Instance.transform.position) > 3f)
            {
                Agent.isStopped = false;
                Agent.SetDestination(PlayerManager.Instance.transform.position);
            }
            else
            {
                Agent.isStopped = true;
            }
        }

        public void SetTargetDoor(DoorNPC doorToOpen)
        {
            followPlayer = false;
            this.doorToOpen = doorToOpen;
            isOpeningDoor = true;
            Agent.SetDestination(this.doorToOpen.GetCenterPoint());
        }

        private IEnumerator OpenDoor()
        {
            float openDoorTime = 1f; // Duration of rotation
            float elapsedTime = 0f;

            Vector3 lookDirection = (doorToOpen.GetCenterPoint() - transform.position).normalized;
            lookDirection.y = 0;
            Quaternion lookAt = Quaternion.LookRotation(lookDirection);

            // Stop movement while rotating
            Agent.isStopped = true;

            while (elapsedTime < openDoorTime)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, elapsedTime / openDoorTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure exact final rotation
            transform.rotation = lookAt;

            // Open door
            doorToOpen.TriggerByNPC();

            // Resume movement
            followPlayer = true;
            Agent.isStopped = false;
            doorToOpen = null;
        }

    }
}