using UnityEngine;

/// <summary>
/// The basic building block for a hoe. Simply tills dirt blocks upon contact
/// </summary>
public class Hoe : PickUp
{
    //the tile that will be hit
    private Dirt selectedTile;

    //All hoes will till dirt when striking it
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Dirt")
        {
            selectedTile = col.gameObject.GetComponent<Dirt>();
            selectedTile.Till();
        }
    }

    
}
