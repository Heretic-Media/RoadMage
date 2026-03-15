using UnityEngine;

public class SlimeRandomiser : MonoBehaviour
{
    [SerializeField] private Material[] slimeMaterials;
    private int chosenMaterialIndex;
    [SerializeField] private GameObject slimeModel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        chosenMaterialIndex = Random.Range(0, slimeMaterials.Length);
        slimeModel.GetComponent<Renderer>().material = slimeMaterials[chosenMaterialIndex];
    }

}
