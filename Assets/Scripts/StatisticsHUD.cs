using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsHUD : MonoBehaviour
{
    private HexapodController hex;
    private HexapodLeg[] legs;
    private TextMeshProUGUI[] updatables;
    private TextMeshProUGUI targetVel;

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
        for(var i = 0; i <= legs.Length-1; i++)
        {
            updatables[i].SetText(updatables[i].name +": " + legs[i].Velocity().ToString("f2"));
        }
    }

    private TextMeshProUGUI[] UpdatablesSetUp()
    {
        var textMesh = new List<TextMeshProUGUI>();
        foreach (RectTransform child in transform)
        {
            if (!child.CompareTag("LegList")) continue;
            textMesh.Add(child.GetComponent<TextMeshProUGUI>());
            //Debug.Log("Added " + child.name);
        }
        return textMesh.ToArray();
    }



}
