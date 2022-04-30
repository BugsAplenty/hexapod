using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsHUD : MonoBehaviour
{
    private HexapodController _hex;
    private HexapodLeg[] _legs;
    private TextMeshProUGUI[] _updatables;
    private TextMeshProUGUI _targetVel;

    private void Awake()
    {
        _hex = GameObject.FindWithTag("Hexapod").GetComponent<HexapodController>();
        _targetVel = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _legs = _hex.GetLegList();
        _updatables = UpdatablesSetUp();
    }

    private void Update()
    {
        _targetVel.SetText(_legs[0].TargetVelocity().ToString("f2"));
        VelocityInserter();
    }


    private void VelocityInserter()
    {
        for(var i = 0; i <= _legs.Length-1; i++)
        {
            _updatables[i].SetText(_updatables[i].name +": " + _legs[i].Velocity().ToString("f2"));
        }
    }

    private TextMeshProUGUI[] UpdatablesSetUp()
    {
        var textMesh = new List<TextMeshProUGUI>();
        foreach (RectTransform child in transform)
        {
            if (!child.CompareTag("LegList")) continue;
            textMesh.Add(child.GetComponent<TextMeshProUGUI>());
            Debug.Log("Added " + child.name);
        }
        return textMesh.ToArray();
    }



}
