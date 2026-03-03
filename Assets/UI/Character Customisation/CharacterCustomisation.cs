using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomisation : MonoBehaviour
{
    public GameObject[] characterModels; // Array to hold different character models
    public GameObject[] characterBodies; // Array to hold different character bodies for material changes
    public GameObject player; // Reference to the player GameObject
    public Material[] characterMaterials; // Array to hold different character materials

    public Texture[] truckColours; // Array to hold different truck colours
    public Texture[] threeWheelCarColours; // Array to hold different car colours
    public Texture[] vanColours;

    static bool specialMaterialSelected; // Static variable to track if a special option is selected
    public Material[] truckSpecialMaterials; // Array to hold different special options for truck character
    public Material[] threeWheelCarSpecialMaterials; // Array to hold different special options for car character
    public Material[] vanSpecialMaterials; // Array to hold different special options for van character

    public GameObject[] truckAccessories; // Array to hold different accessories
    public GameObject[] threeWheelCarAccessories; // Array to hold different accessories
    public GameObject[] vanAccessories;

    

    public static int currentCharacter;
    public static Material currentMaterial; // Reference to the currently selected material
    public static int currentAccessory;
    public GameObject[] selectionMenus;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        specialMaterialSelected = false; // sets the shiny material flag to false at the start of the game
        currentMaterial = characterMaterials[1]; // sets the current material to the first material in the array
        currentMaterial.SetTexture("_BaseMap", threeWheelCarColours[2]); //resets the material to the default colour for the car character

        currentMaterial = characterMaterials[2]; // sets the current material to the first material in the array
        currentMaterial.SetTexture("_BaseMap", vanColours[7]); //resets the material to the default colour for the van character

        currentMaterial = characterMaterials[0]; // sets the current material to the first material in the array
        currentMaterial.SetTexture("_BaseMap", truckColours[0]); //resets the material to the default colour for the truck character

        currentCharacter = 0; // sets the current character index to 0 for the truck character
        currentAccessory = 0; // sets the current accessory to be none
        for (int i = 1; i < characterModels.Length; i++)
        {
            characterModels[i].SetActive(false); // deactivates all character models except truck at the start
        }
    }



    public void ChangeCharacter(int character)
    {
        currentCharacter = character; // sets the current character
        currentMaterial = characterMaterials[character]; // sets the current material to the default material for the selected character
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
            characterBodies[0].GetComponent<Renderer>().material = characterMaterials[0]; // changes the truck character's material to the default material
            currentMaterial.SetTexture("_BaseMap", truckColours[colour]); // changes the character's material to selected colour
        }
        if (currentCharacter == 1)
        {
            characterBodies[1].GetComponent<Renderer>().material = characterMaterials[1]; // changes the car character's material to the default material
            currentMaterial.SetTexture("_BaseMap", threeWheelCarColours[colour]); // changes the character's material to selected colour
        }
        if (currentCharacter == 2)
        {
            characterBodies[2].GetComponent<Renderer>().material = characterMaterials[2]; // changes the van character's material to the default material
            currentMaterial.SetTexture("_BaseMap", vanColours[colour]); // changes the character's material to selected colour
        }
    }

    public void ChangeSpecialMaterials(int material)
    {
        if (currentCharacter == 0)
        {
            currentMaterial = truckSpecialMaterials[material]; // sets the current material to the selected special option
            characterBodies[0].GetComponent<Renderer>().material = truckSpecialMaterials[material]; // changes the truck character's material to selected special option
        }
        if (currentCharacter == 1)
        {
            currentMaterial = threeWheelCarSpecialMaterials[material]; // sets the current material to the selected special option
            characterBodies[1].GetComponent<Renderer>().material = threeWheelCarSpecialMaterials[material]; // changes the car character's material to selected special option
        }
        if (currentCharacter == 2)
        {
            currentMaterial = vanSpecialMaterials[material];
            characterBodies[2].GetComponent<Renderer>().material = vanSpecialMaterials[material]; // changes the van character's material to selected special option
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
        for (int i = 1; i < vanAccessories.Length; i++)
        {
            vanAccessories[i].SetActive(false); // deactivates all van accessories
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
        if (currentCharacter == 2)
        {
            for (int i = 1; i < vanAccessories.Length; i++)
            {
                if (i == accessory)
                {
                    vanAccessories[i].SetActive(true); // activates the selected accessory
                }
            }
        }
    }

    public void EnterGame()
    {
        SceneManager.LoadScene("PortalTransition"); // loads the game via transition screen
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

    public void OpenSpecialOptionsMenu()
    {
        for (int i = 0; i < selectionMenus.Length; i++)
        {
            if (i == 3)
            {
                selectionMenus[i].SetActive(true); // opens the special options menu
            }
            else
            {
                selectionMenus[i].SetActive(false); // closes the other selection menus
            }
        }
    }
}
