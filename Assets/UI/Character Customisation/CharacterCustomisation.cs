using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomisation : MonoBehaviour
{
    [Header("Car Models")]
    [Tooltip("Store the prefab from the scene for the car models here (used to activate different models)")]
    [SerializeField] private GameObject[] characterModels; // Array to hold different character models
    [Tooltip("The models for the bodies of the cars (used for assigning materials)")]
    [SerializeField] private GameObject[] characterBodies; // Array to hold different character bodies for material changes
    [Tooltip("Reference to the player object in the scene if there is one")]
    [SerializeField] private GameObject player; // Reference to the player GameObject

    [Header("Materials")]
    [Tooltip("Store the main materials for the cars here (used for changing the alpha colour")]
    [SerializeField] private Material[] characterMaterials; // Array to hold different character materials
    [Tooltip("Add the colour variations for each car here")]
    [SerializeField] private Texture[] truckColours; // Array to hold different truck colours
    [Tooltip("Add the colour variations for each car here")]
    [SerializeField] private Texture[] threeWheelCarColours; // Array to hold different car colours
    [Tooltip("Add the colour variations for each car here")]
    [SerializeField] private Texture[] vanColours;

    [Header("Unlocked Options")]
    [Tooltip("Add the material variations for each car here")]
    [SerializeField] private Material[] truckSpecialMaterials; // Array to hold different special options for truck character
    [Tooltip("Add the material variations for each car here")]
    [SerializeField] private Material[] threeWheelCarSpecialMaterials; // Array to hold different special options for car character
    [Tooltip("Add the material variations for each car here")]
    [SerializeField] private Material[] vanSpecialMaterials; // Array to hold different special options for van character

    [Header("Accessories")]
    [Tooltip("Add the accessory variations for each car here (first in the array should always be none")]
    [SerializeField] private GameObject[] truckAccessories; // Array to hold different accessories
    [Tooltip("Add the accessory variations for each car here (first in the array should always be none")]
    [SerializeField] private GameObject[] threeWheelCarAccessories; // Array to hold different accessories
    [Tooltip("Add the accessory variations for each car here (first in the array should always be none")]
    [SerializeField] private GameObject[] vanAccessories;

    public static int currentCharacter;
    public static Material currentMaterial; // Reference to the currently selected material
    public static int currentAccessory;
    public static int truckColourIndex;
    public static int carColourIndex;
    public static int vanColourIndex;
    public GameObject[] selectionMenus;
    public UnityEngine.UI.Text starText; // Reference to star count text in UI

    public static bool rewardsPicked;
    public static int silverCategory;
    public static int silverCharacter;
    public static int silverIndex;
    public static int goldCategory;
    public static int goldCharacter;
    public static int goldIndex;

    void Start()
    {
        //specialMaterialSelected = false;

        // Load saved data
        GameSaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            currentCharacter = data.currentCharacter;
            currentAccessory = data.currentAccessory;
            truckColourIndex = data.characterColours[0];
            carColourIndex = data.characterColours[1];
            vanColourIndex = data.characterColours[2];
            rewardsPicked = data.rewardsPicked;
            silverCategory = data.silverCategory;
            silverCharacter = data.silverCharacter;
            silverIndex = data.silverIndex;
            goldCategory = data.goldCategory;
            goldCharacter = data.goldCharacter;
            goldIndex = data.goldIndex;
        }
        else
        {
            currentCharacter = 0;
            currentAccessory = 0;
            truckColourIndex = 0;
            carColourIndex = 2;
            vanColourIndex = 7;
            rewardsPicked = false;
        }

        if (!rewardsPicked)
        {
            PickRandomRewards();
            rewardsPicked = true;
            SaveSystem.SaveGame();
        }

        // Apply saved colours to materials
        currentMaterial = characterMaterials[1];
        currentMaterial.SetTexture("_BaseMap", threeWheelCarColours[carColourIndex]);

        currentMaterial = characterMaterials[2];
        currentMaterial.SetTexture("_BaseMap", vanColours[vanColourIndex]);

        currentMaterial = characterMaterials[0];
        currentMaterial.SetTexture("_BaseMap", truckColours[truckColourIndex]);

        // Set active character model
        for (int i = 0; i < characterModels.Length; i++)
        {
            characterModels[i].SetActive(i == currentCharacter);
        }

        UpdateStarText();
    }

    private void PickRandomRewards()
    {
        List<Vector3Int> pool = new List<Vector3Int>();

        for (int i = 0; i < truckColours.Length; i++)
            if (i != 0) pool.Add(new Vector3Int(0, 0, i));
        for (int i = 0; i < threeWheelCarColours.Length; i++)
            if (i != 2) pool.Add(new Vector3Int(0, 1, i));
        for (int i = 0; i < vanColours.Length; i++)
            if (i != 7) pool.Add(new Vector3Int(0, 2, i));

        for (int i = 0; i < truckSpecialMaterials.Length; i++)
            pool.Add(new Vector3Int(1, 0, i));
        for (int i = 0; i < threeWheelCarSpecialMaterials.Length; i++)
            pool.Add(new Vector3Int(1, 1, i));
        for (int i = 0; i < vanSpecialMaterials.Length; i++)
            pool.Add(new Vector3Int(1, 2, i));

        for (int i = 1; i < truckAccessories.Length; i++)
            pool.Add(new Vector3Int(2, 0, i));
        for (int i = 1; i < threeWheelCarAccessories.Length; i++)
            pool.Add(new Vector3Int(2, 1, i));
        for (int i = 1; i < vanAccessories.Length; i++)
            pool.Add(new Vector3Int(2, 2, i));

        if (pool.Count >= 2)
        {
            int silverPick = Random.Range(0, pool.Count);
            Vector3Int s = pool[silverPick];
            silverCategory = s.x;
            silverCharacter = s.y;
            silverIndex = s.z;

            pool.RemoveAt(silverPick);

            int goldPick = Random.Range(0, pool.Count);
            Vector3Int g = pool[goldPick];
            goldCategory = g.x;
            goldCharacter = g.y;
            goldIndex = g.z;
        }
        else if (pool.Count == 1)
        {
            Vector3Int s = pool[0];
            silverCategory = s.x;
            silverCharacter = s.y;
            silverIndex = s.z;
            goldCategory = -1;
            goldCharacter = -1;
            goldIndex = -1;
        }

        Debug.Log("Silver reward: category=" + silverCategory + " character=" + silverCharacter + " index=" + silverIndex);
        Debug.Log("Gold reward: category=" + goldCategory + " character=" + goldCharacter + " index=" + goldIndex);
    }

    private bool IsUnlocked(int category, int characterIndex, int itemIndex)
    {
        if (category == 0)
        {
            if (characterIndex == 0 && itemIndex == 0) return true;
            if (characterIndex == 1 && itemIndex == 2) return true;
            if (characterIndex == 2 && itemIndex == 7) return true;
        }
        if (category == 2 && itemIndex == 0) return true;

        if (CurrencyManager.stars >= 2 &&
            category == silverCategory && characterIndex == silverCharacter && itemIndex == silverIndex)
            return true;

        if (CurrencyManager.stars >= 3 &&
            category == goldCategory && characterIndex == goldCharacter && itemIndex == goldIndex)
            return true;

        return false;
    }

    public void UpdateStarText()
    {
        if (starText != null)
        {
            starText.text = CurrencyManager.stars.ToString();
        }
    }

    public void ChangeCharacter(int character)
    {
        currentCharacter = character;
        currentMaterial = characterMaterials[character];
        ChangeAccessory(currentAccessory);

        for (int i = 0; i < characterModels.Length; i++)
        {
            if (i == character)
            {
                characterModels[i].SetActive(true);
            }
            else
            {
                characterModels[i].SetActive(false);
            }
        }
        SaveSystem.SaveGame();
    }

    public void ChangeMaterialColour(int colour)
    {
        if (!IsUnlocked(0, currentCharacter, colour)) return;

        currentMaterial = characterMaterials[currentCharacter];
        if (currentCharacter == 0)
        {
            truckColourIndex = colour;
            characterBodies[0].GetComponent<Renderer>().material = characterMaterials[0];
            currentMaterial.SetTexture("_BaseMap", truckColours[colour]);
        }
        if (currentCharacter == 1)
        {
            carColourIndex = colour;
            characterBodies[1].GetComponent<Renderer>().material = characterMaterials[1];
            currentMaterial.SetTexture("_BaseMap", threeWheelCarColours[colour]);
        }
        if (currentCharacter == 2)
        {
            vanColourIndex = colour;
            characterBodies[2].GetComponent<Renderer>().material = characterMaterials[2];
            currentMaterial.SetTexture("_BaseMap", vanColours[colour]);
        }
        SaveSystem.SaveGame();
    }

    public void ChangeSpecialMaterials(int material)
    {
        if (!IsUnlocked(1, currentCharacter, material)) return;

        if (currentCharacter == 0)
        {
            currentMaterial = truckSpecialMaterials[material];
            characterBodies[0].GetComponent<Renderer>().material = truckSpecialMaterials[material];
        }
        if (currentCharacter == 1)
        {
            currentMaterial = threeWheelCarSpecialMaterials[material];
            characterBodies[1].GetComponent<Renderer>().material = threeWheelCarSpecialMaterials[material];
        }
        if (currentCharacter == 2)
        {
            currentMaterial = vanSpecialMaterials[material];
            characterBodies[2].GetComponent<Renderer>().material = vanSpecialMaterials[material];
        }
        SaveSystem.SaveGame();
    }

    public void ChangeAccessory(int accessory)
    {
        if (!IsUnlocked(2, currentCharacter, accessory)) return;

        currentAccessory = accessory;

        for (int i = 1; i < truckAccessories.Length; i++)
        {
            truckAccessories[i].SetActive(false);
        }
        for (int i = 1; i < threeWheelCarAccessories.Length; i++)
        {
            threeWheelCarAccessories[i].SetActive(false);
        }
        for (int i = 1; i < vanAccessories.Length; i++)
        {
            vanAccessories[i].SetActive(false);
        }

        if (currentCharacter == 0)
        {
            for (int i = 1; i < truckAccessories.Length; i++)
            {
                if (i == accessory)
                {
                    truckAccessories[i].SetActive(true);
                }
            }
        }
        if (currentCharacter == 1)
        {
            for (int i = 1; i < threeWheelCarAccessories.Length; i++)
            {
                if (i == accessory)
                {
                    threeWheelCarAccessories[i].SetActive(true);
                }
            }
        }
        if (currentCharacter == 2)
        {
            for (int i = 1; i < vanAccessories.Length; i++)
            {
                if (i == accessory)
                {
                    vanAccessories[i].SetActive(true);
                }
            }
        }
        SaveSystem.SaveGame();
    }

    public void EnterGame()
    {
        SceneManager.LoadScene("PortalTransition");
    }

    public void OpenCharacterSelectionMenu()
     {
        for (int i = 0; i < selectionMenus.Length; i++)
        {
            if (i == 0)
            {
                selectionMenus[i].SetActive(true);
            }
            else
            {
                selectionMenus[i].SetActive(false);
            }
        }
     }

    public void OpenColourSelectionMenu()
    {
        for (int i = 0; i < selectionMenus.Length; i++)
        {
            if (i == 1)
            {
                selectionMenus[i].SetActive(true);
            }
            else
            {
                selectionMenus[i].SetActive(false);
            }
        }
    }

    public void OpenAccessorySelectionMenu()
    {
        for (int i = 0; i < selectionMenus.Length; i++)
        {
            if (i == 2)
            {
                selectionMenus[i].SetActive(true);
            }
            else
            {
                selectionMenus[i].SetActive(false);
            }
        }
    }

    public void OpenSpecialOptionsMenu()
    {
        for (int i = 0; i < selectionMenus.Length; i++)
        {
            if (i == 3)
            {
                selectionMenus[i].SetActive(true);
            }
            else
            {
                selectionMenus[i].SetActive(false);
            }
        }
    }
}
