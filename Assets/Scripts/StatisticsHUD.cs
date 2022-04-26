using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsHUD : MonoBehaviour
{
    HexapodController hex;
    HexapodLeg[] legs;
    TextMeshProUGUI[] updatables;
    TextMeshProUGUI targetVel;

    private void Awake()
    {
        hex = GameObject.FindWithTag("Hexapod").GetComponent<HexapodController>();
        targetVel = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        legs = hex.GetLegList();
        updatables = UpdatablesSetUp();
    }

    private void Update()
    {
        targetVel.SetText(legs[0].TargetVelocity().ToString("f2"));
        VelocityInserter();
    }


    private void VelocityInserter()
    {
        for(int i = 0; i <= legs.Length-1; i++)
        {
            updatables[i].SetText(updatables[i].name +": " + legs[i].Velocity().ToString("f2"));
        }
    }

    private TextMeshProUGUI[] UpdatablesSetUp()
    {
        List<TextMeshProUGUI> textMesh = new List<TextMeshProUGUI>();
        foreach (RectTransform child in transform)
        {
            if(child.tag == "LegList")
            {
                textMesh.Add(child.GetComponent<TextMeshProUGUI>());
                Debug.Log("Added " + child.name);
            }
        }
        return textMesh.ToArray();
    }



}
