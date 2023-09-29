using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunScript : MonoBehaviour
{
    public int speedMult;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) speedMult += 1;
        if(Input.GetKeyDown(KeyCode.O)) speedMult -= 1;

        Time.timeScale = speedMult;
    }
}
