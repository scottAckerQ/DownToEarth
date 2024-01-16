using UnityEngine;

public class WateringCan : PickUp
{
    private Dirt selectedTile;

    public int capacity; //how much water is left in this tool
    public int maxCapacity;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Dirt")
        {
            selectedTile = col.gameObject.GetComponent<Dirt>();
            selectedTile.Water();
            if(capacity != 1)
                capacity--;
        }
    }

    

}
