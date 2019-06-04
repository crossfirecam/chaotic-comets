using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopControls : MonoBehaviour {
    
    // Ship movement & teleport variables
    private float verticalInput, horizontalInput;
    public int playerNumber;
    private string inputNameInsert;

    // Start is called before the first frame update
    void Start() {
        InputChoice();
    }

    void Update() {
        GetInputs();
        ShipMovement();
    }

    // Contains code for receiving inputs from player
    private void GetInputs() {
        // Get axis-based inputs
        verticalInput = Input.GetAxis("Thrust" + inputNameInsert);
        horizontalInput = Input.GetAxis("Rotate Ship" + inputNameInsert);

        // If fire button is pressed, and ship is not teleporting, not dead, and able to fire, then fire
        if (Input.GetButton("Primary Fire" + inputNameInsert)) {
            ConfirmLogic();
        }
        // If power button is pressed, and ship has full power with colliders enabled, and level has no asteroids, then use power
        if (Input.GetButtonDown("Power" + inputNameInsert)) {
        }
    }

    // If rapid shot or triple shot, shoot uniquely. If not, shoot typical projectile
    private void ConfirmLogic() {

    }

    private void ShipMovement() {
        // Left or right menu movement
        if (horizontalInput != 0) {

        }

        // Up or down menu movement
        if (verticalInput != 0) {

            // If thrust is less than 0
            if (verticalInput > 0) {

            }
            else {

            }
        }
    }

    // Alters inputTypeAdd string, so that only buttons on the selected controller work
    void InputChoice() {
        if (BetweenScenesScript.ControlTypeP1 == 0 && playerNumber == 1) {
            inputNameInsert = " (P1joy)";
        }
        else if (BetweenScenesScript.ControlTypeP1 == 1 && playerNumber == 1) {
            inputNameInsert = " (P1key)";
        }
        else if (BetweenScenesScript.ControlTypeP2 == 0 && playerNumber == 2) {
            inputNameInsert = " (P2joy)";
        }
        else if (BetweenScenesScript.ControlTypeP2 == 1 && playerNumber == 2) {
            inputNameInsert = " (P2key)";
        }
        else {
            Debug.Log("Invalid player/controller configuration.");
        }
    }
}
