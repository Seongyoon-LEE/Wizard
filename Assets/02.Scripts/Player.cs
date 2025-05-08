using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(CharacterController))]
// 메인 카메라에서 ray를 쏘면 플레이어가 그곳으로 이동하는 스크립트
public class Player : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private Transform tr;
    [SerializeField] // private으로 선언한걸 인스펙터에서 보여줌
    [Tooltip("애니메이터 컴포넌트")]
    private Animator animator;
    [SerializeField]
    [Tooltip("내비게이션 메쉬 에이전트 컴포넌트")]
    private NavMeshAgent agent;
    [SerializeField] private float m_doubleClickSecond = 0.25f;
    [SerializeField] private bool isOneClick = false; // 원클릭 더블클릭 구분
    [SerializeField] private float m_Timer = 0f; // 타이머 시간저장
    private Vector3 target = Vector3.zero;
    private string speed = "Speed_F"; // ""을 그대로 넣으면 동적할당이 되어서 무거워짐
    private string jump = "Jump_T";
    private string attack = "Attack_T";
    private string skill = "Skill_T";


    void Start()
    {
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>(); 
        agent = GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        CameraRay();
        ClickDoubleClick();
        MouseMovement();
        Attack();
        Skill();
        Jump();



    }

    private void Skill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            agent.isStopped = true; // 정지
            agent.speed = 0f; // 속도 0으로 초기화
            agent.ResetPath(); // 이동 경로 초기화
            agent.velocity = Vector3.zero; // 속도 0으로 설정
            animator.SetTrigger(skill);
        }
        else if(Input.GetKeyUp(KeyCode.Q))
        {
            agent.isStopped = false; // 시작 
            animator.SetFloat(speed, agent.speed, 0.0002f,Time.deltaTime); // 스피드 0으로 아이들 애니메이션 재생
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(jump);
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            agent.isStopped = true; // 정지
            agent.speed = 0f; // 속도 0으로 초기화
            agent.ResetPath(); // 이동 경로 초기화
            agent.velocity = Vector3.zero; // 속도 0으로 설정
            animator.SetTrigger(attack);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetFloat(speed, agent.speed, 0.00002f, Time.deltaTime);
        }
    }

    private void MouseMovement()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, 100, 1 << 8)) // 광선이 맞으면
            {
                if (isOneClick)
                {
                    agent.speed = 3f;
                    agent.isStopped = false; // 네비게이션은 항상 추적
                }
                else
                {
                    agent.speed = 6f; // 더블클릭시 이동속도
                    agent.isStopped = false; // 추적
                }
                target = hit.point; // 마우스 포인트 찍은 위치를 타겟으로 
                agent.destination = target; // 에이전트의 목적지를 타겟으로 
                animator.SetFloat(speed, agent.speed, 0.0002f, Time.deltaTime);
            }
        }
        else
        {
            if (agent.remainingDistance <= 0.5) // 목표지점에 도착하면
            {
                agent.isStopped = true; // 정지
                animator.SetFloat(speed, 0f, 0.002f, Time.deltaTime); // 애니메이션 정지 
            }
        }
    }

    private void CameraRay()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //광선을 메인카메라에서 쏘고 마우스포지션 좌표를 알아낸다.
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);
    }

    private void ClickDoubleClick()
    {
        if (isOneClick && (Time.time - m_Timer) > m_doubleClickSecond) // 흐른시간 > 0.25f 원클릭
        {
            Debug.Log("원클릭!");
            isOneClick = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!isOneClick) // false
            {
                m_Timer = Time.time;
                isOneClick = true;
            }
            else if (isOneClick && (Time.time - m_Timer) < m_doubleClickSecond) // 흐른시간 < 0.25f 더블클릭
            {
                Debug.Log("더블클릭!");
                isOneClick = false;
            }
        }
    }
}
