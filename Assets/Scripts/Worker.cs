using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    public Vector3 home;
    public Building building;
    public bool isReturning = false;

    [SerializeField] private NavMeshAgent agent;
    public void GoToStorage()
    {
        agent.SetDestination(new Vector3(-0.3f, 0, 2));
    }
    public void GoToHome()
    {
        agent.SetDestination(home);
    }
}
