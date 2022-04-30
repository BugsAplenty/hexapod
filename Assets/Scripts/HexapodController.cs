using System.Linq;
using UnityEngine;

public class HexapodController : MonoBehaviour
{

    public HexapodLeg[] legs;
    [SerializeField] public float velForward = 90f;


    private void Awake()
    {
        LegSetup();      
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveForward();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            StopMovement();
        }
    }

    private void StopMovement()
    {
        // Debug.Log("Trying to stop rotation");
        foreach(var leg in legs)
        {
            leg.StopRotation();
        }
        
    }

    private void MoveForward()
    { 
        // Debug.Log("Trying to move Forward");
        foreach (var leg in legs)
        {
            leg.ContinuousRotation(velForward);
        }
    }

    private void LegSetup()
    {
        legs = (
            from Transform child in transform 
            where child.CompareTag("LegAnchor") 
            select child.GetChild(0).GetComponent<HexapodLeg>()
            ).ToArray();
    }

    public HexapodLeg[] GetLegList()
    {
        return legs;
    }
}
