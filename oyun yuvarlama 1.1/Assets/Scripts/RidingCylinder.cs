using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    //Süreceðimiz silindirleri kontrol etmek için bir sýnýf oluþturacaðýz.

    private bool _filled; // silindir yeterli yüke ulaþtýmý
    private float _value; //tuttuðum silindirin sayýsal deðeri.

    public void IncrementCylinderValume(float value)//silindirin büyüyüp küçülmesini saðlayan fonk
    {
        _value += value;

        if (_value > 1)
        {
            // silindirin boyutunu tam olarak 1 yap ve birden ne kadar büyükse o büyüklükte yeni bir silindir yarat.
            float leftValue = _value - 1;
            int cylinderCaunt = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x,-0.5f*(cylinderCaunt-1) - 0.25f , transform.localPosition.z);//oluþan silindirlerin konumlarýný playerin ayaðýnýn altýna getiriyoruz.
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);//boyut deðiþimini uyguladýk

            PlayerController.Current.CreateCylinder(leftValue);
        }
        else if(_value<0)
        {
            //karakterimize bu silindiri yok etmesini söyleyeceðiz.
            PlayerController.Current.DestroyCylinder(this);
        }
        else
        {
            //_value bu aralýkta ise, silindirin boyutunu güncelleyeceðiz(büyüt-küçült) 
            int cylinderCaunt = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCaunt - 1) - 0.25f * _value, transform.localPosition.z);//oluþan silindirlerin konumlarýný playerin ayaðýnýn altýna getiriyoruz.
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);//boyut deðiþimini uyguladýk
        }

    }
}
