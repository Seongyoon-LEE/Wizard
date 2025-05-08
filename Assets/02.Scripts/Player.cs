using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(CharacterController))]
// ���� ī�޶󿡼� ray�� ��� �÷��̾ �װ����� �̵��ϴ� ��ũ��Ʈ
public class Player : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private Transform tr;
    [SerializeField] // private���� �����Ѱ� �ν����Ϳ��� ������
    [Tooltip("�ִϸ����� ������Ʈ")]
    private Animator animator;
    [SerializeField]
    [Tooltip("������̼� �޽� ������Ʈ ������Ʈ")]
    private NavMeshAgent agent;
    [SerializeField] private float m_doubleClickSecond = 0.25f;
    [SerializeField] private bool isOneClick = false; // ��Ŭ�� ����Ŭ�� ����
    [SerializeField] private float m_Timer = 0f; // Ÿ�̸� �ð�����
    private Vector3 target = Vector3.zero;
    private string speed = "Speed_F"; // ""�� �״�� ������ �����Ҵ��� �Ǿ ���ſ���
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
            agent.isStopped = true; // ����
            agent.speed = 0f; // �ӵ� 0���� �ʱ�ȭ
            agent.ResetPath(); // �̵� ��� �ʱ�ȭ
            agent.velocity = Vector3.zero; // �ӵ� 0���� ����
            animator.SetTrigger(skill);
        }
        else if(Input.GetKeyUp(KeyCode.Q))
        {
            agent.isStopped = false; // ���� 
            animator.SetFloat(speed, agent.speed, 0.0002f,Time.deltaTime); // ���ǵ� 0���� ���̵� �ִϸ��̼� ���
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
            agent.isStopped = true; // ����
            agent.speed = 0f; // �ӵ� 0���� �ʱ�ȭ
            agent.ResetPath(); // �̵� ��� �ʱ�ȭ
            agent.velocity = Vector3.zero; // �ӵ� 0���� ����
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
            if (Physics.Raycast(ray, out hit, 100, 1 << 8)) // ������ ������
            {
                if (isOneClick)
                {
                    agent.speed = 3f;
                    agent.isStopped = false; // �׺���̼��� �׻� ����
                }
                else
                {
                    agent.speed = 6f; // ����Ŭ���� �̵��ӵ�
                    agent.isStopped = false; // ����
                }
                target = hit.point; // ���콺 ����Ʈ ���� ��ġ�� Ÿ������ 
                agent.destination = target; // ������Ʈ�� �������� Ÿ������ 
                animator.SetFloat(speed, agent.speed, 0.0002f, Time.deltaTime);
            }
        }
        else
        {
            if (agent.remainingDistance <= 0.5) // ��ǥ������ �����ϸ�
            {
                agent.isStopped = true; // ����
                animator.SetFloat(speed, 0f, 0.002f, Time.deltaTime); // �ִϸ��̼� ���� 
            }
        }
    }

    private void CameraRay()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //������ ����ī�޶󿡼� ��� ���콺������ ��ǥ�� �˾Ƴ���.
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);
    }

    private void ClickDoubleClick()
    {
        if (isOneClick && (Time.time - m_Timer) > m_doubleClickSecond) // �帥�ð� > 0.25f ��Ŭ��
        {
            Debug.Log("��Ŭ��!");
            isOneClick = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!isOneClick) // false
            {
                m_Timer = Time.time;
                isOneClick = true;
            }
            else if (isOneClick && (Time.time - m_Timer) < m_doubleClickSecond) // �帥�ð� < 0.25f ����Ŭ��
            {
                Debug.Log("����Ŭ��!");
                isOneClick = false;
            }
        }
    }
}
