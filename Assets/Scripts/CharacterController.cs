using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    /// <summary>
    /// The difference states that our character can be in
    /// </summary>

    public enum CharacterStates { Idle, Roaming, Waving, Playing, Fleeing }

    public CharacterStates currentCharacterState; // the current state that our character is in.

    // roaming state variables 
    public GameManager gameManager; // a reference to our game manager
    public Rigidbody rigidBody; // reference to our rigidbody

    // roaming state variables
    private Vector3 currentTargetPosition; // the target we are currently heading towards. 
    private Vector3 previousTargetPosition; // the last target we were heading towards.
    public float moveSpeed = 3; // how fast our character is moving.
    public float minDistanceToTarget = 1; // how close should we get to our target?

    // Waving state variables
    public float idleTime = 2; // Once we reach our target position how long should we wait till we get another position? 
    public float currentIdleWaitTime; // the time we are waiting till, we an move again. 

    // waving state variables
    public float waveTime = 2; // the time spent waving
    private float currentWaveTime; // the current time to wave till. 
    public float distanceToStartWavingFrom = 4f; // the distance that we will be checking to see if we are in range to wave at another character
    private CharacterController[] allCharactersInScene; // a collection of all charachters in our scene
    public float timeBetweenWaves = 5; // time between when allowed to wave again.
    private float currentTimeBetweenWaves; // the current time for our next wave to be initiated. 

    //fleeing state variables
    public float distanceThresholdOfPlayer = 5; // the distance that is "too" close for the player to be to us. 

    /// <summary>
    /// Returns the currentTargetPosition
    /// And Set, the new current position
    /// </summary>

    private Vector3 CurrentTargetPosition
    {
        get
        {
            return currentTargetPosition; // gets the current value
        }
        set
        {
            previousTargetPosition = currentTargetPosition; // assign our current position to our previous target position
            currentTargetPosition = value; // assign the new value to our current target position

        }

    }


    // Start is called before the first frame update
    void Start()
    {
        CurrentTargetPosition = gameManager.ReturnRandomPositionOnField(); // get a random starting position
        allCharactersInScene = FindObjectsOfType<CharacterController>(); // find the references to all characters in our scene. 
        currentCharacterState = CharacterStates.Roaming; // set the character by default start roaming
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Current Time is: "+ Time.time);
        LookAtTargetPosition(); //always look towards the position we are aiming for.
        HandleRoamingstate(); // call our roaming state funcion. 
        HandleIdleState(); // call our idle state function
        HandleWavingState(); // call our idle state function. 
        HandleFleeingState(); // call our fleeing state function
        HandlePlayingState(); // call our player state function
    }

    /// <summary>
    /// Handlesthe roaming state of our character
    /// </summary>
    private void HandleRoamingstate()
    {
        /// If we are still too far away move closer
        if (currentCharacterState == CharacterStates.Roaming && Vector3.Distance (transform.position, CurrentTargetPosition) > minDistanceToTarget)
        {
            Vector3 targetPosition = new Vector3(CurrentTargetPosition.x, transform.position.y, CurrentTargetPosition.z); // the position we want to move towards 
            Vector3 nextMovePosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); // the amount we should move towards that position
            rigidBody.MovePosition(nextMovePosition);
            currentIdleWaitTime = Time.time + idleTime;
        }
        else if (currentCharacterState == CharacterStates.Roaming) // so check to see if we're still roaming.
        {
            currentCharacterState = CharacterStates.Idle; // start idling
        }
    }

    /// <summary>
    /// handle the idle state of our character
    /// </summary>
    private void HandleIdleState()
    {
        if(currentCharacterState == CharacterStates.Idle)
            // we must be close enough to our target position.
            // we wait a couple seconds. 
            // then find a new position to move to. 
            if(Time.time > currentIdleWaitTime)
            {
                // let us find a new position
                CurrentTargetPosition = gameManager.ReturnRandomPositionOnField();
                currentCharacterState = CharacterStates.Roaming; // start roaming again. 
            }
    }

    /// <summary>
    /// Handles the fleeing state of our character
    /// </summary>
    private void HandleFleeingState()
    {
        if (currentCharacterState != CharacterStates.Fleeing && gameManager.IsPlayerTooCloseToCharacter(transform, distanceThresholdOfPlayer))
        {
            // we should be fleeing
            currentCharacterState = CharacterStates.Fleeing;
        }
        else if (currentCharacterState == CharacterStates.Fleeing && gameManager.IsPlayerTooCloseToCharacter(transform, distanceThresholdOfPlayer))
        {
            //we are fleeing now
            if (currentCharacterState == CharacterStates.Fleeing && Vector3.Distance(transform.position, CurrentTargetPosition) > minDistanceToTarget)
            {
                Vector3 targetPosition = new Vector3(CurrentTargetPosition.x, transform.position.y, CurrentTargetPosition.z); // the position we want to move towards 
                Vector3 nextMovePosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime *1.5f); // the amount we should move towards that position
                rigidBody.MovePosition(nextMovePosition);
            }
            else
            {
                CurrentTargetPosition = gameManager.ReturnRandomPositionOnField();
            }
        }
        else if (currentCharacterState == CharacterStates.Fleeing && !gameManager.IsPlayerTooCloseToCharacter(transform, distanceThresholdOfPlayer) == false)
        {
            //if we are still fleeing, then we want to transition back to our roaming state. 
            currentCharacterState = CharacterStates.Roaming;
            currentTargetPosition = gameManager.ReturnRandomPositionOnField();
        }
    }

    /// <summary>
    /// Handles the playing state of our character
    /// </summary>
    private void HandlePlayingState()
    {

    }

    /// <summary>
    /// Handles the waving state.
    /// </summary>
    private void HandleWavingState()
    {
        if (ReturnCharacterTransformToWaveAt()!= null && currentCharacterState != CharacterStates.Waving && Time.time > currentTimeBetweenWaves && currentCharacterState != CharacterStates.Fleeing)
        {
            // we should start waving!
            currentCharacterState = CharacterStates.Waving;
            currentWaveTime = Time.time + waveTime; // setup the time we should be waving till.
            CurrentTargetPosition = ReturnCharacterTransformToWaveAt().position; // set the current target position to the closest transform, so that way we also rotate towards it. 
        }
        if(currentCharacterState == CharacterStates.Waving && Time.time > currentWaveTime)
        {
            // stop waving
            CurrentTargetPosition = previousTargetPosition; // resume moving towards our random target position. 
            currentTimeBetweenWaves = Time.time + timeBetweenWaves; // the next time for when we can wave again. 
            currentCharacterState = CharacterStates.Roaming; // start roaming agains. 
        }
       
    }

/// <summary>
///  Return a transform if they are in range of the player to be waved at. 
/// </summary>
/// <returns></returns>
    private Transform ReturnCharacterTransformToWaveAt()
    {
        //loopingthrough all the characters in our scene
        for (int i = 0; i
            < allCharactersInScene.Length; i++)
        {
            // if the current elementwe are upto is not equal to instance our character
            if (allCharactersInScene[i] != this)
            {
                // check the distance between them, if they are close enough return that other character
                if (Vector3.Distance(transform.position, allCharactersInScene[i].transform.position) < distanceToStartWavingFrom)
                {
                    // but also let's return the character we should be moving at.
                    return allCharactersInScene[i].transform;

                }
            }

        }
        return null;
    }

    /// <summary>
    ///  Rotatesour character to always face the direction we are heading
    /// </summary>
    private void LookAtTargetPosition()
    {
        Vector3 directionToLookAt = CurrentTargetPosition - transform.position; // not the direction we should be looking at. 
        directionToLookAt.y = transform.position.y; // don't change the Y posiion
        Quaternion rotationOfDirection = Quaternion.LookRotation(directionToLookAt); // get a rotation that we can use to look towards.
        transform.rotation = rotationOfDirection; // set our current rotation to our rotation to face towards.
    }

    private void OnDrawGizmosSelected()
    {
        // draw a blue spere on the position we are moving towards
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CurrentTargetPosition, 0.5f); 
    }

}
