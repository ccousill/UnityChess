using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
[ExecuteAlways]
public class TileLabeler : MonoBehaviour
{
    TextMeshPro label;
    Vector2Int coordinates = new Vector2Int();
    // Start is called before the first frame update
    void Awake()
    {
        label = GetComponent<TextMeshPro>();
        DisplayLabel();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Application.isPlaying){
            DisplayLabel();
            UpdateName();
        }
    }

    void DisplayLabel(){
        coordinates.x = Mathf.RoundToInt(transform.parent.position.x);
        coordinates.y = Mathf.RoundToInt(transform.parent.position.z);
        label.text = coordinates.x + "," + coordinates.y;
    }
    void UpdateName(){
        transform.parent.name = coordinates.ToString();
    }
}
