using System.Collections.Generic;
using UnityEngine;

public class createRuins : MonoBehaviour
{
    private int destructionAmount = 0;

    private void CreateRuins()
    {
        destructionAmount = Random.Range(5, (transform.childCount - 5));

        List<Transform> children = new List<Transform>(transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        children.Sort((a, b) => b.position.y.CompareTo(a.position.y));

        for (int i = 0; i < destructionAmount; i++)
        {
            if (i >= children.Count)
            {
                break;
            }

            Transform child = children[i];
            Destroy(child.gameObject);
        }

        destructionAmount = Random.Range(1, (transform.childCount));
        for (int i = 0; i < destructionAmount; i++)
        {
            int randomIndex = Random.Range(0, transform.childCount);
            Transform child = transform.GetChild(randomIndex);
            Destroy(child.gameObject);
        }
    }
    private void Start()
    {
        CreateRuins();
    }
}
