using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GlyphAbility : MonoBehaviour
{
    [SerializeField] private GameObject summonPrefab;

    [SerializeField] private GameObject glyphGuidePrefab;

    [SerializeField] private float length = 10;
    [SerializeField] private int sides = 5;

    private List<GameObject> glyphGuides = new List<GameObject>( );
    private GameObject area = null;
    private GameObject goal = null;

    private Rigidbody rb;

    [Header("Input")]
    [SerializeField] private Key castingKey = Key.F;
    private bool casting;

    [Header("Debugging to see glyph casted bool")]
    [SerializeField] bool glyphCasted = false;

    [Header("Number of goals hit")]
    [SerializeField] int goalsHit = 0;

    private int glyphIndex = -1;
    

    void Start()
    {
       rb = GetComponentInParent<Rigidbody>();

        //glyphGuides.Add(null);
        //glyphGuides[0] = Instantiate(glyphGuidePrefab, rb.transform.position, Quaternion.identity);
    }

    void Update()
    {
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        casting =
        (gp != null && gp.triangleButton.isPressed) ||
        (kb != null && kb[castingKey].isPressed);

        if (casting && !glyphCasted)
        {
            glyphCasted = true;

            goalsHit = 0;

            //if (rb.linearVelocity.magnitude > 2) 
            //{
            //    length = (rb.linearVelocity.magnitude * 4.2f) / sides;
            //}

            /// Create more glyph guides if needed
            if (glyphGuides.Count + 1 < sides)
            {
                for (int i = glyphGuides.Count; i < sides; i++)
                {
                    glyphGuides.Add(null);
                    glyphGuides[i] = Instantiate(glyphGuidePrefab, rb.transform.position, Quaternion.identity);
                }
            }

            /// Set glyph guides position in a circle
            for (int i = 0; i < sides; i++)
            {
                if (i == 0)
                {
                    glyphGuides[0].transform.position = new Vector3(
                        rb.transform.position.x,
                        rb.transform.position.y,
                        rb.transform.position.z);

                    glyphGuides[0].transform.rotation = Quaternion.Euler(
                        0,
                        180 + rb.transform.eulerAngles.y,
                        0);
                }
                else if (i > 0)
                {
                    glyphGuides[i].transform.position = new Vector3(
                        goal.transform.position.x,
                        goal.transform.position.y,
                        goal.transform.position.z);

                    glyphGuides[i].transform.rotation = Quaternion.Euler(
                        glyphGuides[0].transform.rotation.eulerAngles.x,
                        glyphGuides[0].transform.rotation.eulerAngles.y + (360f / sides) * i,
                        glyphGuides[0].transform.rotation.eulerAngles.z);
                }

                area = glyphGuides[i].transform.Find("Area").gameObject;
                goal = glyphGuides[i].transform.Find("Goal").gameObject;

                GameObject prefabArea = glyphGuidePrefab.transform.Find("Area").gameObject;
                GameObject prefabGoal = glyphGuidePrefab.transform.Find("Goal").gameObject;

                /// Reactivate glyph guides that are deavtive
                glyphGuides[i].SetActive(true);
                area.SetActive(true);
                goal.SetActive(true);

                area.transform.localScale = new Vector3(prefabArea.transform.localScale.x, prefabArea.transform.localScale.y, length);
                area.transform.localPosition = new Vector3(
                    prefabArea.transform.localPosition.x,
                    prefabArea.transform.localPosition.y,
                    prefabArea.transform.localPosition.z - (length / 2) + 0.5f);

                goal.transform.localPosition = new Vector3(
                    prefabGoal.transform.localPosition.x,
                    prefabGoal.transform.localPosition.y,
                    prefabGoal.transform.localPosition.z - length + 1);
            }

            glyphIndex = 0;
            area = glyphGuides[glyphIndex].transform.Find("Area").gameObject;
            goal = glyphGuides[glyphIndex].transform.Find("Goal").gameObject;
        }
    }

    void FixedUpdate()
    {
        if (goal != null)
        {
            if (IsPlayerInside(goal))
            {
                HandleGoalEnter();
            }
        }

        if (area != null)
        {
            if (!IsPlayerInside(area))
            {
                HandleAreaExit();
            }
        }
    }

    bool IsPlayerInside(GameObject region)
    {
        Collider regionCollider = region.GetComponent<Collider>();
        Collider playerCollider = rb.GetComponent<Collider>(); // parent collider

        return Physics.ComputePenetration(
            regionCollider, regionCollider.transform.position, regionCollider.transform.rotation,
            playerCollider, playerCollider.transform.position, playerCollider.transform.rotation,
            out _, out _
        );
    }

    private void ResetGlyphCast() 
    {
        for (int i = 0; i < glyphGuides.Count; i++)
        {
            glyphGuides[i].SetActive(false);
        }

        area = null;
        goal = null;

        glyphCasted = false;
    }

    private void HandleGoalEnter()
    {
        goal.SetActive(false);
        goalsHit++;
        glyphIndex++;
        if (glyphIndex < glyphGuides.Count)
        {
            area = glyphGuides[glyphIndex].transform.Find("Area").gameObject;
            goal = glyphGuides[glyphIndex].transform.Find("Goal").gameObject;

            rb.transform.rotation = Quaternion.Euler(
            rb.transform.rotation.eulerAngles.x,
            rb.transform.rotation.eulerAngles.y + (360f / sides),
            rb.transform.rotation.eulerAngles.z);
        }
        else
        {
            /// Cast glyph spell here
            SummonSpell();
            /// 

            ResetGlyphCast();

            print("entered goal");
        }
    }

    private void HandleAreaExit()
    {
        //glyphGuides[glyphIndex].SetActive(false);

        /// Completely fail the cast and cancel
        ResetGlyphCast();

        print("exited area");
    }

    private void SummonSpell()
    {
        if (summonPrefab == null)
            return;

        Vector3 spawnPos = goal.transform.position;
        GameObject summon = Instantiate(summonPrefab, spawnPos, Quaternion.identity);

        summon.transform.localScale = new Vector3(
            summon.transform.localScale.x * length,
            summon.transform.localScale.y * length,
            summon.transform.localScale.z * length);

        print("spell summoned");
    }
}
