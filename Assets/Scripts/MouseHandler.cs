using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{

    public LayerMask layersToHit; // the layers are going to allowed to hit.
    public GameManager gameManager; // a reference to our gamemanager;


    // Update is called once per frame
    void Update()
    {
        GetMouseInput();
    }

    /// <summary>
    /// Gets Input from the player mouse/tap on the screen
    /// </summary>
    void GetMouseInput()
    {
        if (Input.GetMouseButtonDown(0))// primary mouse input/touch input. 
        {
            RaycastHit hit; // data stored based on what we've hit. 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // draws a ray from my camera to my mouse position in the world

            // Does our ray cast hit something or blocks the ray, store the data in hit. 
            if (Physics.Raycast(ray, out hit, layersToHit))
            {
                gameManager.SpawnOnMoveSoccerBall(hit.point); //the point in the wrld where the ray has hit, spawn of our soccerball, or move it.
            }
        }
    }
}
