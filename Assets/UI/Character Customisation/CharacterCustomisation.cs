using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomisation : MonoBehaviour
{
    public GameObject[] characterModels; // Array to hold different character models
    public GameObject player; // Reference to the player GameObject
    public Material[] characterMaterials; // Array to hold different character materials
    public Texture[] truckColours; // Array to hold different truck colours
    public Texture[] threeWheelCarColours; // Array to hold different car colours
    public GameObject[] truckAccessories; // Array to hold different accessories
    public GameObject[] threeWheelCarAccessories; // Array to hold different accessories
    public static int currentCharacter;
    private Material currentMaterial; // Reference to the currently selected material
    public static int currentAccessory;
    public GameObject[] selectionMenus;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMaterial = characterMaterials[0]; // sets the current material to the first material in the array
        currentMaterial.SetTexture("_BaseMap", truckColours[0]);
        currentCharacter = 0; // sets the current character index to 0 for the truck character
        for (int i = 1; i < characterModels.Length; i++)
        {
            characterModels[i].SetActive(false); // deactivates all character models except truck at the start
        }
    }

    public void ChangeCharacter(int character)
    {
        currentCharacter = character; // sets the current character
        ChangeAccessory(currentAccessory); // updates the accessories to match the new character


        for (int i = 0; i < characterModels.Length; i++)
        {
            if (i == character)
            {
                characterModels[i].SetActive(true); // activates the selected character model
            }
            else
            {
                characterModels[i].SetActive(false); // deactivates the other character models
            }
        }
    }


    public void ChangeMaterialColour(int colour)
    {
        currentMaterial = characterMaterials[currentCharacter];
        if (currentCharacter == 0)
        {
            currentMaterial.SetTexture("_BaseMap", truckColours[colour]); // changes the character's material to selected colour
        }
        if (currentCharacter == 1)
        {
            currentMaterial.SetTexture("_BaseMap", threeWheelCarColours[colour]); // changes the character's material to selected colour
        }
    }

    public void ChangeAccessory(int accessory)
    {
        currentAccessory = accessory;

        for (int i = 1; i < truckAccessories.Length; i++)
        {
            truckAccessories[i].SetActive(false); // deactivates all truck accessories
        }
        for (int i = 1; i < threeWheelCarAccessories.Length; i++)
        {
            threeWheelCarAccessories[i].SetActive(false); // deactivates all car accessories
        }

        if (currentCharacter == 0)
        {
            for (int i = 1; i < truckAccessories.Length; i++)
            {
                if (i == accessory)
                {
                    truckAccessories[i].SetActive(true); // activates the selected accessory
                }
            }
        }
        if (currentCharacter == 1)
        {
            for (int i = 1; i < threeWheelCarAccessories.Length; i++)
            {
                if (i == accessory)
                {
                    threeWheelCarAccessories[i].SetActive(true); // activates the selected accessory
                }
            }
        }
    }

    public void EnterGame()
    {
        SceneManager.LoadScene("ALPHA with assets"); // loads the main menu scene
    }

    public void OpenCharacterSelectionMenu()
     {
        for (int i = 0; i < selectionMenus.Length; i++)
        {
            if (i == 0)
            {
                selectionMenus[i].SetActive(true); // opens the character selection menu
            }
            else
            {
                selectionMenus[i].SetActive(false); // closes the other selection menus
            }
        }
     }

    public void OpenColourSelectionMenu()
    {
        for (int i = 0; i < selectionMenus.Length; i++)
        {
            if (i == 1)
            {
                selectionMenus[i].SetActive(true); // opens the colour selection menu
            }
            else
            {
                selectionMenus[i].SetActive(false); // closes the other selection menus
            }
        }

    }

   

    public void OpenAccessorySelectionMenu()
    {
        for (int i = 0; i < selectionMenus.Length; i++)
        {
            if (i == 2)
            {
                selectionMenus[i].SetActive(true); // opens the accessory selection menu
            }
            else
            {
                selectionMenus[i].SetActive(false); // closes the other selection menus
            }
        }
    }
}
