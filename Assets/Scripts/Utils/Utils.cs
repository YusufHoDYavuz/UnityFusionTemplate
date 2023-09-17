using UnityEngine;

public static class Utils
{
    /*public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-5, 5), 2, Random.Range(-5, 5));
    }*/

    public static void SetRenderLayerInChildren(Transform transform, int layerNumber)
    {
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>(true))
        {
            if (trans.CompareTag("LayerChangeIgnore"))
                continue;
            
            trans.gameObject.layer = layerNumber;
        }
    }
}