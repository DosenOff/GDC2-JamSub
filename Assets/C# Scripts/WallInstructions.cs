using UnityEngine;

public class WallInstructions : MonoBehaviour
{
    Animator anim;

    bool wasded = false;
    bool clicked = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
            wasded = true;

        if (Input.GetMouseButton(0))
            clicked = true;
    }

    public void CheckWASD()
    {
        if (wasded)
            anim.SetTrigger("WASD");
    }

    public void CheckClick()
    {
        if (clicked)
            anim.SetTrigger("Click");
    }
}
