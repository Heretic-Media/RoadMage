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
    private Vector3 lastPosition;

    [Header("Input")]
    [SerializeField] private Key castingKey = Key.F;
    private bool casting;

    [SerializeField] private float castingTime = 1;
    private float castTimeDelta = 0;

    [Header("Debugging to see glyph casted bool")]
    [SerializeField] bool glyphCasted = false;

    private int glyphIndex = -1;

    private Vector3 glyphOffset = Vector3.zero;
    private float glyphMovement = 0;
    

    void Start()
    {
        if (!rb) rb = GetComponentInParent<Rigidbody>();

        //glyphGuides.Add(null);
        //glyphGuides[0] = Instantiate(glyphGuidePrefab, rb.transform.position, Quaternion.identity);
    }

    void Update()
    {
        /// Inputs
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        casting =
        (gp != null && gp.triangleButton.isPressed) ||
        (kb != null && kb[castingKey].isPressed);

        /// Casting Timer
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

        /// Casting Glyph
        if (casting && !glyphCasted && castTimeDelta >= castingTime)
        {
            castTimeDelta = castingTime;
            glyphCasted = true;

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

            glyphGuides[0].transform.position = new Vector3(
                rb.transform.position.x,
                rb.transform.position.y,
                rb.transform.position.z);

            glyphGuides[0].transform.rotation = Quaternion.Euler(0, rb.transform.eulerAngles.y, 0);

            AlignGlyph();

            glyphIndex = 0;
            currentArea = glyphGuides[glyphIndex].transform.Find("Area").gameObject;
            currentGoal = glyphGuides[glyphIndex].transform.Find("Goal").gameObject;

            lastPosition = rb.transform.position;
        }
        /// Un-casting Glyph
        else if (casting && glyphCasted && castTimeDelta <= 0)
        {
            ResetGlyphCast();
        }

        /// Updating Active Glyph
        if (glyphCasted)
        {
            if (lastPosition != rb.transform.position)
            {
                Vector2 glyphUnitDirection = new Vector2(
                    (float)Mathf.Sin(Mathf.Deg2Rad * (glyphGuides[glyphIndex].transform.rotation.eulerAngles.y)),
                    (float)Mathf.Cos(Mathf.Deg2Rad * (glyphGuides[glyphIndex].transform.rotation.eulerAngles.y)));

                Vector3 movement = new Vector3(
                    (rb.transform.position.x - lastPosition.x),
                    0,
                    (rb.transform.position.z - lastPosition.z));

                Vector3 direction = new Vector3(glyphUnitDirection.x, 0, glyphUnitDirection.y);
                float projectedMagnitude = Vector3.Dot(movement, direction);

                float sideLength = (length - 1) * 2;
                if (glyphMovement + projectedMagnitude < 0)
                {
                    if (glyphMovement > 0)
                    {
                        Vector3 movementInDirection = direction * glyphMovement;

                        glyphMovement = 0;
                        glyphOffset += movementInDirection;
                    }
                }
                else if (glyphMovement + projectedMagnitude >= sideLength)
                {
                    if (glyphMovement < sideLength)
                    {
                        Vector3 movementInDirection = direction * (sideLength - glyphMovement);

                        //glyphMovement = sideLength;
                        glyphMovement = 0;
                        glyphOffset += movementInDirection;

                        glyphIndex++;
                        if (glyphIndex < glyphGuides.Count) 
                        { 
                        }
                        else 
                        {
                            SummonSpell();
                            ResetGlyphCast();
                        }
                    }
                }
                else
                {
                    Vector3 movementInDirection = direction * projectedMagnitude;

                    glyphMovement += projectedMagnitude;
                    glyphOffset += movementInDirection;
                }

                //glyphOffset += (movementInDirection / movementInDirection.magnitude) * movement.magnitude;

                //glyphOffset += new Vector3(
                //    (movementInDirection.x / movementInDirection.magnitude),
                //    (movementInDirection.y / movementInDirection.magnitude),
                //    (movementInDirection.z / movementInDirection.magnitude)) * movement.magnitude;

                glyphGuides[0].transform.position = new Vector3(
                    rb.transform.position.x - glyphOffset.x,
                    rb.transform.position.y,
                    rb.transform.position.z - glyphOffset.z);

                AlignGlyph();

                lastPosition = rb.transform.position;
            }
        }
    }

    /// Align glyph with glyphGuides[0] rotation
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
        glyphOffset = Vector3.zero;
        glyphMovement = 0;

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
            summon.transform.localScale.x * length * 2,
            summon.transform.localScale.y * length * 2,
            summon.transform.localScale.z * length * 2);

        print("spell summoned");
    }
}
