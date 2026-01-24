using System;
using UnityEngine;

public class GlyphTrigger : MonoBehaviour
{
    GlyphAbility glyphAbility;
    public void Initialise(GlyphAbility _glyphAbility)
    {
        glyphAbility = _glyphAbility;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (glyphAbility == null) 
        {
            //Debug.Log("Glyph Ability is null");
        }
        else if (collision.CompareTag("Player")) 
        {
            glyphAbility.PlayerEnterTrigger(GetComponent<Collider>());

            //Debug.Log("Player Enter " + GetComponent<Collider>().name);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (glyphAbility == null)
        {
            //Debug.Log("Glyph Ability is null");
        }
        else if (collision.CompareTag("Player"))
        {
            glyphAbility.PlayerExitTrigger(GetComponent<Collider>());

            //Debug.Log("Player Exit " + GetComponent<Collider>().name);
        }
    }
}
