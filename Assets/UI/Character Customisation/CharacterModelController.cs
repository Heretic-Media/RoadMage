using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

public class CharacterModelController : MonoBehaviour
{
    public GameObject[] models; // Array to hold different character models
    public GameObject[] characterBodies; // Array to hold different character bodies for material changes


    public GameObject[] truckAccessories; // Array to hold different truck accessories
    public GameObject[] threeWheeledCarAccessories; // Array to hold different car accessories
    public GameObject[] vanAccessories; // Array to hold different van accessories

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < models.Length; i++)
        {
            models[i].SetActive(false); // Deactivates all character models 
        }

        models[CharacterCustomisation.currentCharacter].SetActive(true); // Activates the currently selected character model

        if (CharacterCustomisation.currentMaterial != null) // Checks if a material has been selected
        {
            characterBodies[CharacterCustomisation.currentCharacter].GetComponent<Renderer>().material = CharacterCustomisation.currentMaterial; // Sets the material of the character to the currently selected material
        }

        if (CharacterCustomisation.currentCharacter == 0) // If the current character is the truck
        {

            for (int i = 1; i < truckAccessories.Length; i++)
            {
                truckAccessories[i].SetActive(false); // Deactivates all truck accessories
            }
            truckAccessories[CharacterCustomisation.currentAccessory].SetActive(true); // Activates the currently selected truck accessory
        }
        else if (CharacterCustomisation.currentCharacter == 1) // If the current character is the three-wheeled car
        {
            for (int i = 1; i < threeWheeledCarAccessories.Length; i++)
            {
                threeWheeledCarAccessories[i].SetActive(false); // Deactivates all car accessories
            }
            threeWheeledCarAccessories[CharacterCustomisation.currentAccessory].SetActive(true); // Activates the currently selected car accessory
        }
        else if (CharacterCustomisation.currentCharacter == 2) // If the current character is the van
        {
            for (int i = 1; i < vanAccessories.Length; i++)
            {
                vanAccessories[i].SetActive(false); // Deactivates all van accessories
            }
            vanAccessories[CharacterCustomisation.currentAccessory].SetActive(true); // Activates the currently selected van accessory
        }
    }
}
