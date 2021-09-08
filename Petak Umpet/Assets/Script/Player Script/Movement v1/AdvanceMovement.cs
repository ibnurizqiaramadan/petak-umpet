using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceMovement : MonoBehaviour
{
    // Start is called before the first frame update

    PlayerController playerController;
    public float speedBoost = 6.0f;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerSprint();
    }

    void PlayerSprint() {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            playerController.walkSpeed += speedBoost;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            playerController.walkSpeed -= speedBoost;
    }
}
