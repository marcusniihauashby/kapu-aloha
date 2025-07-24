using UnityEngine;
using UnityEngine.UI; // For UI elements

public class CubePlacer : MonoBehaviour
{
    public GameObject cube;         // Assign your cube
    public GameObject uiPrompt;     // Assign your "E to place" UI
    public float interactRange = 3f;

    private Camera cam;
    private bool isHovering = false;

    void Start()
    {
        cam = Camera.main;
        cube.SetActive(false);       // Cube starts hidden
        uiPrompt.SetActive(false);   // UI starts hidden
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        isHovering = false;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.CompareTag("PlaceTrigger"))  // Tag the object to hover on
            {
                isHovering = true;
                uiPrompt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    cube.SetActive(true);
                }
            }
        }

        if (!isHovering)
        {
            uiPrompt.SetActive(false);
        }
    }
}
