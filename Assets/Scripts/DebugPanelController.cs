using System.Linq;
using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public class DebugPanelController : MonoBehaviour
{
    public HexapodController hexapodController; // Assign in inspector
    public TextMeshProUGUI debugPanelText; // Assign your TextMeshProUGUI component here
    private void Start()
    {
        // Assuming debugPanelText is your TextMeshProUGUI component
        debugPanelText.enableAutoSizing = true;
        debugPanelText.fontSizeMin = 2; // Minimum font size
        debugPanelText.fontSizeMax = 40; // Maximum font size
    }

    private void Update()
    {
        UpdateDebugPanel();
    }

    private void UpdateDebugPanel()
    {
        var debugText = "";
    
        // Using tags or another method to identify each leg motor, append their info to the debugText
        debugText += hexapodController.LegMotors.Aggregate(
            "", (current, legMotor) => current + legMotor.gameObject.tag + ": " + legMotor.GetDebugInfo() + "\n\n");
    
        debugPanelText.text = debugText;
    }
}