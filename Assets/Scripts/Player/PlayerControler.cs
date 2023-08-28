using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{

    [SerializeField] GameObject[] deathZones;
    private BlockDirection blockDirection;
    public BlockDirection Blockdirection
    {
        get
        {
            return blockDirection;
        }
        set
        {
            blockDirection = value;
        }
    }

    PlayerControls playerControls;

    private Vector2 block;
    int compte = 0;

    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Gameplay.Block.started += ctx => OnBlock(ctx);
        playerControls.Gameplay.Block.canceled += ctx => LeaveBlock(ctx);
    }

    public void OnBlock(InputAction.CallbackContext ctx)
    {
        
        block = ctx.ReadValue<Vector2>();
        

        Debug.Log("Touche " + compte + " : " + block);

        compte++;

        if(block == new Vector2(0, 1))
        {
            deathZones[0].GetComponent<LightUp>().Activate();
        }
        else if(block == new Vector2(0, -1))
        {
            deathZones[1].GetComponent<LightUp>().Activate();
        }
        else if (block == new Vector2(1, 0))
        {
            deathZones[2].GetComponent<LightUp>().Activate();
        }
        else if (block == new Vector2(-1, 0))
        {
            deathZones[3].GetComponent<LightUp>().Activate();
        }


        Debug.Log(blockDirection);
    }

    public void LeaveBlock(InputAction.CallbackContext ctx)
    {
        blockDirection = BlockDirection.Null;
        //foreach (var deathZone in deathZones)
        //{
            //deathZone.GetComponent<LightUp>().Deactivate();
        //}
    }

    public enum BlockDirection
    {
        North,
        South,
        East,
        West,
        Null
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

}
