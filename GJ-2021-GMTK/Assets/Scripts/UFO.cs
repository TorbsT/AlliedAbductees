using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{

    [SerializeField] public Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void abduct()
    {
        GM.getAudioManager().play("Abduction");
    }
    public void setPos(float x)
    {
        transform.position = new Vector3(x - offset.x, offset.y, 6);
    }

}
