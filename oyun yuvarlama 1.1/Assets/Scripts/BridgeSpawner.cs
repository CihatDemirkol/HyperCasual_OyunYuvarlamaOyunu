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
        Vector3 direction = endReference.transform.position - startReference.transform.position; //iki referas nokta aras�ndaki y�n vekt�r�n� elde ettik
        float distance = direction.magnitude;//iki referans noktas� aras�ndaki mesafeyi veriyo
        direction = direction.normalized; //bu iki kodla art�k verilen y�leri zemine g�re ayarlayabilirim.
        hiddenPlatform.transform.forward = direction;
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance);//y�ne mesafeye g�re collaidor�m�z�de boyutland�rm�� olduk
        hiddenPlatform.transform.position = startReference.transform.position + direction * (distance / 2) + (new Vector3(0,-direction.z,direction.y) * hiddenPlatform.size.y / 2);


    }

}
