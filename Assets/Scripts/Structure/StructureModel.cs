using UnityEngine;

public class StructureModel : MonoBehaviour
{
    float yHeight = 0f;

    public void CreateModel(GameObject model)
    {
        var structure = Instantiate(model, transform.position, Quaternion.identity, transform);
        structure.transform.localPosition = new Vector3(0, structure.transform.localPosition.y, 0);
        yHeight = structure.transform.position.y;
    }

    public void SwapModel(GameObject model, Quaternion rotation)
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        var structure = Instantiate(model, transform.position, rotation, transform);
        structure.transform.localPosition = new Vector3(0, yHeight, 0);
        structure.transform.localRotation = rotation;
    }
}
