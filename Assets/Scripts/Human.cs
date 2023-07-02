using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Human : MonoBehaviour
{
    public GameObject DropPrefab;
    public Vector2Int DropCountRandomRange;
    public Vector2 DropRange;

    public float MaxNestPointDistance;
    
    private NavMeshAgent agent;
    private Rigidbody2D rigidbody2D;


    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()	{
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        SetNewRandomDestination();
        
    }

    private void SetNewRandomDestination()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * MaxNestPointDistance;
        randomPoint.z = 0;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            
            agent.SetDestination(hit.position);     
        }
        else
        {
            // Debug.LogError("Could not find next destination!");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotate(agent.velocity.normalized);
        if (agent.remainingDistance <= 0.05f)
        {
            SetNewRandomDestination();
        }
    }
    
    private void Rotate(Vector2 relativeVector)
    {
        relativeVector.Normalize();

        float desiredAngle = Mathf.Atan2(relativeVector.y, relativeVector.x);
        desiredAngle *= Mathf.Rad2Deg;
        desiredAngle += 90;
        
        rigidbody2D.SetRotation(desiredAngle);
    }

    void DropBlood()
    {
        int dropCount = Random.Range(DropCountRandomRange.x, DropCountRandomRange.y + 1);

        for (int i = 0; i < dropCount; i++)
        {
            Vector3 PositionOffset = new Vector3(Random.Range(-DropRange.x, DropRange.x), Random.Range(-DropRange.y, DropRange.y), 0.0f);
            var go = GameObject.Instantiate(DropPrefab, transform.position + PositionOffset, quaternion.RotateZ(Random.Range(0.0f, 360.0f)));
            go.transform.localScale = new Vector3(Random.Range(0.8f, 1.1f), Random.Range(0.8f, 1.1f), 1.0f);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponentInParent<TopDownCarController>())
        {
            DropBlood();
            GameManager.Instance.HumansKilled++;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponentInParent<TopDownCarController>())
        {
            DropBlood();
            GameManager.Instance.HumansKilled++;
            Destroy(gameObject);
        }
    }
}
