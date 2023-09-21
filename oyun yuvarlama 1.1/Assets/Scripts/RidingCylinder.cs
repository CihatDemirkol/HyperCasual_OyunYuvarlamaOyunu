using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    //S�rece�imiz silindirleri kontrol etmek i�in bir s�n�f olu�turaca��z.

    private bool _filled; // silindir yeterli y�ke ula�t�m�
    private float _value; //tuttu�um silindirin say�sal de�eri.

    public void IncrementCylinderValume(float value)//silindirin b�y�y�p k���lmesini sa�layan fonk
    {
        _value += value;

        if (_value > 1)
        {
            // silindirin boyutunu tam olarak 1 yap ve birden ne kadar b�y�kse o b�y�kl�kte yeni bir silindir yarat.
            float leftValue = _value - 1;
            int cylinderCaunt = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x,-0.5f*(cylinderCaunt-1) - 0.25f , transform.localPosition.z);//olu�an silindirlerin konumlar�n� playerin aya��n�n alt�na getiriyoruz.
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);//boyut de�i�imini uygulad�k

            PlayerController.Current.CreateCylinder(leftValue);
        }
        else if(_value<0)
        {
            //karakterimize bu silindiri yok etmesini s�yleyece�iz.
            PlayerController.Current.DestroyCylinder(this);
        }
        else
        {
            //_value bu aral�kta ise, silindirin boyutunu g�ncelleyece�iz(b�y�t-k���lt) 
            int cylinderCaunt = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCaunt - 1) - 0.25f * _value, transform.localPosition.z);//olu�an silindirlerin konumlar�n� playerin aya��n�n alt�na getiriyoruz.
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);//boyut de�i�imini uygulad�k
        }

    }
}
