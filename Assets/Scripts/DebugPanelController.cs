using System.Linq;
using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public class DebugManager : MonoBehaviour
{
    public HexapodController hexapodController; // Assign in inspector
    public TextMeshProUGUI debugPanelText; // Assign your TextMeshProUGUI component here

    private void Update()
    {
        UpdateDebugPanel();
    }

    private void UpdateDebugPanel()
    {
        var debugText = hexapodController.GetDebugInfo() + "\n\n";

        // Using tags or another method to identify each leg motor, append their info to the debugText
        debugText += hexapodController.tripod1.Concat(hexapodController.tripod2).Aggregate(
            "", (current, legMotor) => current + legMotor.gameObject.tag + ": " + legMotor.GetDebugInfo() + "\n\n");

        debugPanelText.text = debugText;
    }
}