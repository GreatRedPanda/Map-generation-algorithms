using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public event System.Action<Enemy> OnDied;
    public event System.Action<Enemy> OnReturnedToSpawn;

    public float StopFollowDistance;
    public float RangeAttackDistance;

   
    [HideInInspector]
    public GameObject StartPoint;
    [HideInInspector]
    public Transform Target;


    NavMeshAgent agent;

    System.Action currentAction;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        currentAction = followTarget;
    }

    void decideOnAction()
    {

     

        float distance = Vector2.Distance(Target.position, transform.position);
        
        bool targetTooClose = distance < agent.stoppingDistance;
        bool targetTooFar = distance > StopFollowDistance;
        bool targetInRangeAttack = distance < RangeAttackDistance;
        if (!targetTooFar && !targetTooClose)
        {
            currentAction = followTarget;
        }
        else
        {
            if (targetTooClose)
                currentAction = meleeAttack;
            else if (targetInRangeAttack)
                currentAction = rangeAttack;
            else if (targetTooFar)
                currentAction = returnToSpawn;
        }


    }
    void followTarget()
    {
        Debug.Log("Following target" );
        agent.SetDestination(Target.position);

        Vector3 diff = Target.position - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);


     
    }
    void Update()
    {
        decideOnAction();
        currentAction?.Invoke();
    }


    void meleeAttack()
    {
        Debug.Log("Melee attack"); 
    }
    void rangeAttack()
    {

        Debug.Log("Range attack");
    }
    void returnToSpawn()
    {

        agent.SetDestination(StartPoint.transform.position);

        float distance = Vector2.Distance(transform.position, StartPoint.transform.position);
        if (distance < agent.stoppingDistance)
        {
            OnReturnedToSpawn?.Invoke(this);
        }
    }
}
