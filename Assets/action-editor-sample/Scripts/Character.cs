using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ActionEditor.Sample
{
    using Runtime;

    public class Character : MonoBehaviour
    {
        enum State
        {
            Moving,
            LookAtTarget,
            Action,
            Attacking,
            Idle,
            Damaged,
        }

        [SerializeField] ActionPoint[] m_Points;
        [SerializeField] BlendableAnimator m_Animator;
        [SerializeField] NavMeshAgent m_Agent;
        [SerializeField] State m_Status = State.Idle;
        [SerializeField] Transform m_Target;

        int m_CurrentIndex;
        float m_LookAtRemainTime;
        ActionPoint m_CurrentPoint;
        PlayableSequenceContext m_Context;

        private void Awake()
        {
            m_Status = State.Idle;
        }

        private void Update()
        {
            m_Animator.Animator.SetFloat("Speed", m_Agent.velocity.magnitude);

            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit))
                {
                    if(hit.collider.gameObject == this.gameObject)
                    {
                        Damage();
                    }
                }
            }

            switch (m_Status)
            {
                case State.Idle:
                    if(SetNextPoint())
                    {
                        m_Status = State.Moving;
                    }
                    break;

                case State.Moving:
                    var delta = Vector3.Distance(m_CurrentPoint.transform.position, transform.position);
                    if (delta <= 0.3f)
                    {
                        m_Status = State.LookAtTarget;
                        m_LookAtRemainTime = 0.3f;
                    }
                    break;

                case State.LookAtTarget:
                    LookAtTarget(m_Target.position);
                    m_LookAtRemainTime -= Time.deltaTime;
                    if(m_LookAtRemainTime <= 0f)
                    {
                        m_Status = State.Action;
                    }
                    break;

                case State.Action:
                    Action();
                    break;
            }
            

            if (m_CurrentPoint == null)
                return;

            
        }

        void Damage()
        {
            m_Agent.isStopped = true;
            m_Agent.velocity = Vector3.zero;
            var prevState = m_Status;
            var ctx = m_Animator.PlayAnimation("Damaged");
            ctx.OnCompleted += () => {
                m_Agent.isStopped = false;
                m_Status = prevState;
            };
            m_Status = State.Damaged;
        }
        bool SetNextPoint()
        {
            if (m_Agent.pathStatus != NavMeshPathStatus.PathInvalid)
            {
                m_Agent.isStopped = false;
                m_CurrentIndex = (m_CurrentIndex + 1) % m_Points.Length;
                m_CurrentPoint = m_Points[m_CurrentIndex];
                m_Agent.SetDestination(m_CurrentPoint.transform.position);
                return true;
            }

            return false;
        }

        void LookAtTarget(Vector3 destination)
        {
            Vector3 lookPos = destination - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.25f);
        }

        void Action()
        {
            m_Agent.isStopped = true;
            var actionPoint = m_Points[m_CurrentIndex];
            var ctx = m_Animator.PlaySequence(actionPoint.Sequence);
            ctx.OnCompleted += () => {
                m_Status = State.Idle;
            };
            m_Status = State.Attacking;
        }
    }
}