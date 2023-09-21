using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject startReference, endReference;
    public BoxCollider hiddenPlatform;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 direction = endReference.transform.position - startReference.transform.position; //iki referas nokta arasýndaki yön vektörünü elde ettik
        float distance = direction.magnitude;//iki referans noktasý arasýndaki mesafeyi veriyo
        direction = direction.normalized; //bu iki kodla artýk verilen yöleri zemine göre ayarlayabilirim.
        hiddenPlatform.transform.forward = direction;
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance);//yüne mesafeye göre collaidorümüzüde boyutlandýrmýþ olduk
        hiddenPlatform.transform.position = startReference.transform.position + direction * (distance / 2) + (new Vector3(0,-direction.z,direction.y) * hiddenPlatform.size.y / 2);


    }

}
