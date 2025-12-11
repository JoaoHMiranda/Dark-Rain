using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour 
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // Velocidade do giro

    private Vector2 input;
    private Animator anim;

    void Start() 
    {
        anim = GetComponentInChildren<Animator>();
    }

    void OnMove(InputValue val) 
    {
        input = val.Get<Vector2>();
    }

    void Update() 
    {
        // --- TRAVA DE PAUSE ---
        // Se o jogo estiver pausado (tempo parado), não faça nada.
        if (Time.timeScale == 0) return;
        // ----------------------

        // Zona Morta
        if(input.magnitude < 0.1f) input = Vector2.zero;

        Vector3 dir = new Vector3(input.x, 0, input.y);
        
        if(dir != Vector3.zero)
        {
            transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
        
        if(anim) 
        {
            bool isMoving = dir.magnitude > 0;
            anim.SetBool("IsRunning", isMoving);
        }
    }
}