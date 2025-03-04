using UnityEngine;

namespace Game
{
    public class WomanNPC : NPC, Interactable
    {
        public Dialouge dialouge;
        public Dialouge dialougeAfterBossKill;
        private bool followPlayer;

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
    }
}