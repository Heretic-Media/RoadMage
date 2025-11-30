using System.Collections.Generic;
using UnityEngine;

public class GlyphAbility : MonoBehaviour
{
    [SerializeField] private GameObject glyphGuidePrefab;

    private List<GameObject> glyphGuides = new List<GameObject>();
    private GameObject area;
    private GameObject goal;
    [SerializeField] private float length = 10;
    [SerializeField] private int sides = 5;

    private Rigidbody rb;

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();

        glyphGuides.Add(Instantiate(glyphGuidePrefab, rb.transform.position, Quaternion.identity));

        area = glyphGuides[0].transform.Find("Area").gameObject;
        goal = glyphGuides[0].transform.Find("Goal").gameObject;

        area.transform.localScale = new Vector3(area.transform.localScale.x, area.transform.localScale.y, length);
        area.transform.localPosition = new Vector3(
            area.transform.localPosition.x, 
            area.transform.localPosition.y, 
            area.transform.localPosition.z - (length / 2) + 0.5f);

        goal.transform.localPosition = new Vector3(
            goal.transform.localPosition.x,
            goal.transform.localPosition.y,
            goal.transform.localPosition.z - length + 1);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == goal.GetComponent<Collider>())
        {
            for (int i = 0; i < glyphGuides.Count; i++)
            {
                goal.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == area.GetComponent<Collider>()) 
        {
            for (int i = 0; i < glyphGuides.Count; i++)
            {
                glyphGuides[i].SetActive(false);
            }
        }
    }
}
