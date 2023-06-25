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

    public Transform DestiantionPos;
    
    // Start is called before the first frame update
    void Start()	{
        var agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(new Vector3(DestiantionPos.position.x, DestiantionPos.position.y, transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
    }

    void DropBlood()
    {
        int dropCount = Random.Range(DropCountRandomRange.x, DropCountRandomRange.y + 1);

        for (int i = 0; i < dropCount; i++)
        {
            Vector3 PositionOffset = new Vector3(Random.Range(-DropRange.x, DropRange.x), Random.Range(-DropRange.y, DropRange.y), 0.0f);
            GameObject.Instantiate(DropPrefab, transform.position + PositionOffset, quaternion.identity);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponentInParent<TopDownCarController>())
        {
            DropBlood();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponentInParent<TopDownCarController>())
        {
            DropBlood();
            Destroy(gameObject);
        }
    }
}
