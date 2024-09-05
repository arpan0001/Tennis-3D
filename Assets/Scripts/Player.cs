using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector2 moveVector;
    float speed = 3f;
    public float moveSpeed = 3f;
    // Start is called before the first frame update
    public void InputPlayer(InputAction.CallbackContext _context){
        moveVector = _context.ReadValue<Vector2>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    private  void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

       if (h != 0 || v != 0)
        {
            transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime); 
        }

        Vector3 movement = new Vector3(moveVector.x, 0, moveVector.y);
        movement.Normalize();
        transform.Translate(moveSpeed * movement * Time.deltaTime);
        
    }
}
