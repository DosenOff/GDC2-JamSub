using UnityEngine;

public class Skateboard : MonoBehaviour
{
    private Transform skateboardLoc;
    private Player player;

    void Start()
    {
        skateboardLoc = GameObject.Find("SkateBoardLocation").GetComponent<Transform>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        transform.position = skateboardLoc.transform.position;
        transform.rotation = player.transform.rotation;
    }
}
