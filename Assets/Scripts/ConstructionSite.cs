using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : MonoBehaviour
{
    // Must be eight meters on X axis, outside face on negative Z side
    public GameObject fencePrefab;

    // A dictionary of known construction sites, keyed by their center position.
    Hashtable sites = new Hashtable();

    public void CreatePloppableSite(Vector3 center, float xTiles, float zTiles)
    {
        // Create a game object to store the construction site, and record it.
        GameObject site = new GameObject();
        site.transform.position = center;
        site.name = "Ploppable Construction Site";
        sites.Add(center, site);

        // The Y position of the fences is constant.
        float yPos = center.y + (fencePrefab.transform.localScale.y / 2);

        // Draw the fence pieces along the Z sides and make them children of the site.
        float zXPos = 4 + center.x - (xTiles / 2) * 8;
        float negZZPos = center.z - (zTiles * 8) / 2;
        float posZZPos = center.z + (zTiles * 8) / 2;
        for (int i = 0; i < xTiles;  i++)
        {
            Vector3 negZPos = new Vector3(zXPos, yPos, negZZPos);
            Instantiate(fencePrefab, negZPos, Quaternion.identity).transform.parent = site.transform;

            Vector3 posZPos = new Vector3(zXPos, yPos, posZZPos);
            Instantiate(fencePrefab, posZPos, Quaternion.Euler(0,180,0)).transform.parent = site.transform;

            zXPos += 8;
        }

        // Draw the fence pieces along the X sides and make them children of the site.
        float xZPos = 4 + center.z - (zTiles / 2) * 8;
        float negXXPos = center.x - (xTiles * 8) / 2;
        float posXXPos = center.x + (xTiles * 8) / 2;
        for (int i = 0; i < zTiles; i++)
        {
            Vector3 negXPos = new Vector3(negXXPos, yPos, xZPos);
            Instantiate(fencePrefab, negXPos, Quaternion.Euler(0, 90, 0)).transform.parent = site.transform;

            Vector3 posXPos = new Vector3(posXXPos, yPos, xZPos);
            Instantiate(fencePrefab, posXPos, Quaternion.Euler(0, -90, 0)).transform.parent = site.transform;

            xZPos += 8;
        }
    }

    public void DestroyPloppableSite(Vector3 center)
    {
        // Look up the site in the dictionary.
        GameObject site = (GameObject)sites[center];
        // Remove it from the dictionary and destroy it.
        sites.Remove(center);
        if (site != null)
        {
            Destroy(site);
        }

        // At some point, an object pool will probably be more performant.
    }
}
