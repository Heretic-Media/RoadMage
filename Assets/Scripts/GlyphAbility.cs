using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GlyphAbility : MonoBehaviour
{
    [SerializeField] private GameObject summonPrefab;

    [SerializeField] private GameObject glyphGuidePrefab;

    [SerializeField] private float length = 10;
    [SerializeField] private int sides = 5;

    private List<GameObject> glyphGuides = new List<GameObject>();
    private GameObject currentArea = null;
    private GameObject currentGoal = null;

    private Rigidbody rb;

    [Header("Input")]
    [SerializeField] private Key castingKey = Key.F;
    private bool casting;

    [SerializeField] private float castingTime = 1;
    private float castTimeDelta = 0;

    [Header("Debugging to see glyph casted bool")]
    [SerializeField] bool glyphCasted = false;

    [Header("Number of goals hit")]
    [SerializeField] int goalsHit = 0;

    private int glyphIndex = -1;

    [SerializeField] private Vector3 offset = Vector3.zero;
    

    void Start()
    {
        if (!rb) rb = GetComponentInParent<Rigidbody>();

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

        if (!glyphCasted) 
        {
            castTimeDelta += Time.deltaTime;
        }
        else
        {
            if (castTimeDelta > 0)
            {
                castTimeDelta -= Time.deltaTime;
            }
            else if (castTimeDelta < 0) 
            {
                castTimeDelta = 0;
            }
        }

        if (casting && !glyphCasted && castTimeDelta >= castingTime)
        {
            castTimeDelta = castingTime;
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
                    glyphGuides[i].transform.Find("Area").gameObject.GetComponent<GlyphTrigger>().Initialise(this);
                    glyphGuides[i].transform.Find("Goal").gameObject.GetComponent<GlyphTrigger>().Initialise(this);
                }
            }

            glyphGuides[0].transform.position = new Vector3(
                rb.transform.position.x,
                rb.transform.position.y,
                rb.transform.position.z);

            glyphGuides[0].transform.rotation = Quaternion.Euler(0, rb.transform.eulerAngles.y, 0);

            AlignGlyph();

            for (int i = 0; i < sides; i++)
            {
                currentArea = glyphGuides[i].transform.Find("Area").gameObject;
                currentGoal = glyphGuides[i].transform.Find("Goal").gameObject;

                GameObject prefabArea = glyphGuidePrefab.transform.Find("Area").gameObject;
                GameObject prefabGoal = glyphGuidePrefab.transform.Find("Goal").gameObject;

                /// Reactivate glyph guides that are deavtive
                glyphGuides[i].SetActive(true);
                currentArea.SetActive(true);
                currentGoal.SetActive(true);

                currentArea.transform.localScale = new Vector3(prefabArea.transform.localScale.x, prefabArea.transform.localScale.y, length);
                currentArea.transform.localPosition = new Vector3(
                    prefabArea.transform.localPosition.x,
                    prefabArea.transform.localPosition.y,
                    prefabArea.transform.localPosition.z + (length / 2) - 0.5f);

                currentGoal.transform.localPosition = new Vector3(
                    prefabGoal.transform.localPosition.x,
                    prefabGoal.transform.localPosition.y,
                    prefabGoal.transform.localPosition.z + length - 1);
            }

            glyphIndex = 0;
            currentArea = glyphGuides[glyphIndex].transform.Find("Area").gameObject;
            currentGoal = glyphGuides[glyphIndex].transform.Find("Goal").gameObject;
        }
        else if (casting && glyphCasted && castTimeDelta <= 0)
        {
            ResetGlyphCast();
        }

        if (glyphCasted)
        {
            Vector2 unitDirection = new Vector2(
                (float)Mathf.Cos(Mathf.Deg2Rad * glyphGuides[0].transform.rotation.eulerAngles.y),
                (float)Mathf.Sin(Mathf.Deg2Rad * glyphGuides[0].transform.rotation.eulerAngles.y));

            //Vector3 movementInDirection = new Vector3(
            //    (rb.transform.position.x - glyphGuides[0].transform.position.x) * directionY.x,
            //    0,
            //    (rb.transform.position.x - glyphGuides[0].transform.position.y) * directionY.y);

            Vector3 movementInDirection = new Vector3(offset.x * unitDirection.x, 0, offset.z * unitDirection.y);

            glyphGuides[0].transform.position = new Vector3(
                rb.transform.position.x + movementInDirection.x,
                rb.transform.position.y,
                rb.transform.position.z + movementInDirection.z);

            AlignGlyph();
        }
    }

    private void AlignGlyph() 
    {
        for (int i = 1; i < sides; i++) 
        {
            glyphGuides[i].transform.position = new Vector3(
                glyphGuides[i - 1].transform.Find("Goal").transform.position.x,
                glyphGuides[i - 1].transform.Find("Goal").transform.position.y,
                glyphGuides[i - 1].transform.Find("Goal").transform.position.z);

            glyphGuides[i].transform.rotation = Quaternion.Euler(
                glyphGuides[0].transform.rotation.eulerAngles.x,
                glyphGuides[0].transform.rotation.eulerAngles.y + (360f / sides) * i,
                glyphGuides[0].transform.rotation.eulerAngles.z);
        }
    }

    private void ResetGlyphCast() 
    {
        for (int i = 0; i < glyphGuides.Count; i++)
        {
            glyphGuides[i].SetActive(false);
        }

        glyphCasted = false;
    }

    public void PlayerEnterTrigger(Collider other)
    {
        //if (currentGoal != null && other == currentGoal.GetComponent<Collider>())
        //{
        //    currentGoal.SetActive(false);
        //    goalsHit++;
        //    glyphIndex++;
        //    if (glyphIndex < glyphGuides.Count)
        //    {
        //        currentArea = glyphGuides[glyphIndex].transform.Find("Area").gameObject;
        //        currentGoal = glyphGuides[glyphIndex].transform.Find("Goal").gameObject;

        //        rb.transform.rotation = Quaternion.Euler(
        //        rb.transform.rotation.eulerAngles.x,
        //        rb.transform.rotation.eulerAngles.y + (360f / sides),
        //        rb.transform.rotation.eulerAngles.z);
        //    }
        //    else
        //    {
        //        /// Cast glyph spell
        //        SummonSpell();

        //        ResetGlyphCast();
        //    }
        //}
    }

    public void PlayerExitTrigger(Collider other)
    {
        //if (currentArea != null && other == currentArea.GetComponent<Collider>())
        //{
        //    //glyphGuides[glyphIndex].SetActive(false);

        //    /// Completely fail the cast and cancel
        //    ResetGlyphCast();
        //}
    }

    private void SummonSpell()
    {
        if (summonPrefab == null)
            return;

        Vector3 spawnPos = rb.transform.position;
        GameObject summon = Instantiate(summonPrefab, spawnPos, Quaternion.identity);

        summon.transform.localScale = new Vector3(
            summon.transform.localScale.x * length,
            summon.transform.localScale.y * length,
            summon.transform.localScale.z * length);

        print("spell summoned");
    }
}
