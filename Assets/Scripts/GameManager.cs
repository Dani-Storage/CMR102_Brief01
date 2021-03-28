using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform soccerField; // a reference to our soccer field
    public Vector3 moveArea; // the size of our area where we can move
    public Transform arCamera; // reference to our arcamera

    public GameObject soccerballPrefab; // a reference to the soccer ball in our scene.
    private GameObject currentSoccerBallInstance; // the current soccerball that has been spawned in. 
    private Transform aRContentParent; // reference o the overall parent of the as content.  

    public Transform ARContentParent { get => aRContentParent; set => aRContentParent = value; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log ("New Random Position of:" + ReturnRandomPositionOnField());
    }

    // Update is called once per frame

    void Update()
    {
        ReturnRandomPositionOnField();
    }

    /// <summary>
    /// Reutrns a random position within our move area/
    /// </summary>
    /// <returns></returns> 

    public Vector3 ReturnRandomPositionOnField()
    {
        float xPosition = Random.Range(-moveArea.x / 2, moveArea.x / 2);// gives us a random number between negative moveArea X and positive moveArea X.
        float yPosition = soccerField.position.y; // our soccer fields y transformed position
        float zPosition = Random.Range(-moveArea.z / 2, moveArea.z / 2);// gives us a random number between negative moveArea Z and positive moveArea Z.
        
        return new Vector3(xPosition, yPosition, zPosition);
    }

    /// <summary>
    /// this is a debug function, that lets us draw objects in our scene view, its not viewable in the game view.
    /// </summary>

    private void OnDrawGizmosSelected()
    { 
        // if the user has just put a soccer field in, just get out of this function
    if(soccerField == null)
        {
            return;
        }
        Gizmos.color = Color.red; // sets my gizmo to red.
        Gizmos.DrawCube(soccerField.position + new Vector3(0,0.05f,0), moveArea); // draws a cube the at the soccer fields position, and to the size of our move area. 
    }

    /// <summary>
    /// Returns true or false if we are too close, or not close enough to our AR camera. 
    /// </summary>
    /// <param name="character"></param>
    /// <param name="distancethreshold"></param>
    /// <returns></returns>
    public bool IsPlayerTooCloseToCharacter(Transform character, float distancethreshold)
    {
        if(Vector3.Distance(arCamera.position, character.position) <= distancethreshold)
        {
            // returns true if we are too close. 
            return true; 
        }
        else
        {
            return false; 
        }
        // returns false if we are too far away
    }

    /// <summary>
    ///  Spawns in the new soccer ball based on the position provided. If a soccer ball already exists in the world, we just want to move it to the new position. 
    /// </summary>
    /// <param name="positionToSpawn"></param>
    
    public void SpawnOnMoveSoccerBall (Vector3 positionToSpawn)
    { 
        if (soccerballPrefab == null)
        { 
        Debug.LogError("Something is wrong there is no soccerball assigned in the inspector");
        return;
        }

        // if the soccer ball isn't spawned into the world yet
        if (currentSoccerBallInstance == null)
        {
            // spawn in and store a refernce to our soccer ball and parent it to our ar content parent
            currentSoccerBallInstance = Instantiate(soccerballPrefab, positionToSpawn, soccerballPrefab.transform.rotation, aRContentParent);
            currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; // sets the velocity of the soccer ball to 0
            currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // sets the anglular velocity of the soccer ball to 0
            AlertCharactersToSoccerBallSpawningIn(); // tell everyone the ball is spawned
        }
        else
        {
            // the soccerball already exists, so let us just move it. 
            currentSoccerBallInstance.transform.position = positionToSpawn; // move our soccerball to the position to spawn.
            currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; // sets the velocity of the soccer ball to 0
            currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // sets the anglular velocity of the soccer ball to 0
        }
    }

    /// <summary>
    /// Findsa all characters in the scene and loops through them and tells them that there is a soccerball
    /// </summary>
    private void AlertCharactersToSoccerBallSpawningIn()
    {
        CharacterController[] mice = FindObjectsOfType<CharacterController>(); // Find all instances of our character controller class in our scene.
        for(int i=0; i<mice.Length; i++)
        {
            //tell the characters the ball is spawned in. 
            mice[i].SoccerBallSpawned(currentSoccerBallInstance.transform);
        }
    }
}