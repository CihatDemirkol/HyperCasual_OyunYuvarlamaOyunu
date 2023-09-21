using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //1.1
    public static PlayerController Current;//di�er silindirlerin playere eri�ebilmesi i�in bir de�i�ken yarat�yoruz.


    public GameObject RidingCylinderPrefab;//silindir prefabumu tutacak bir dei�ken yarat�yoruz.
    public List<RidingCylinder> cylinders; //playerin alt�ndaki silindirleri tutmak i�in bir lis olu�turduk

    public float limitX; // player ekrandan ��kmamas� i�in s�n�rlama.platformdan d��ar� ��kmamal�. Bunlar� unityde ayarlayaca�z
    public float runningSpeed;//ko�ma h�z�
    public float XSpeed;//Sa�a sola ne kadar h�zla gidecek
    private float _currentRunningSpeed;//ko�ma h�z�

    private bool _spawningBridge;
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;

    private float _creatingBridgeTimer;

    // Start is called before the first frame update
    void Start()
    {
        Current = this;//de�i�keni �u anda ba�l� oldu�umuz playere e�itledik.aktard�k.
        _currentRunningSpeed = runningSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float newX = 0;
        float touchXDelta = 0;
        // �imdi if else ile oyuncunun pc demi mobildemi oldu�unu taspit etmeliyiz. yani ekrana m� dokundu yoksa fareylemi dokundu
        //sonra ilk dokunduktan itibaren x y�r�ngesinde ne kadar kayd�rd���n� bulaca��z.
        if (Input.touchCount > 0 && Input.GetTouch(0).phase==TouchPhase.Moved)//mobil i�in belirliyoruz
        {
            touchXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;
            // burada ilk dokunu�tan itibaren x ekseninde parma�� ne kadar kayd�rd���n� buluyoruz ve bunu ekran boyutuna b�l�yoruz. ��nk� b�lmesek ne kadar ekran k���kse o kadar x ekseni hassasla��r.
        }
        else if (Input.GetMouseButton(0))//pc i�in ayn� �eyi yap�yoz
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + XSpeed * touchXDelta * Time.deltaTime;  //burada parmak ne kadar kaym��sa o kadar player sa�a sola kayar.
        newX = Mathf.Clamp(newX, -limitX, limitX);  // burada platformdan d��mesin diye sa�da ve solda limitlendiriyoruz.

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        // burada karakterin pozisyonunu bulunduran de�i�ken yazd�m ve h�z�n� ve kuvvetini ayarlamak i�in _curr*** de�i�kenini denkleme ekledim
        //b�ylelikle _curr** de�i�kenini artt�rd���mda player hareket edecektir
        transform.position = newPosition;
        // yapt���m de�i�keni playerin pozisyonuna e�itledim

        if (_spawningBridge)
        {
            _creatingBridgeTimer -= Time.deltaTime;
            if (_creatingBridgeTimer<0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderValume(-0.01f);
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position; //iki referas nokta aras�ndaki y�n vekt�r�n� elde ettik
                float distance = direction.magnitude;//iki referans noktas� aras�ndaki mesafeyi veriyo
                direction = direction.normalized; //bu iki kodla art�k verilen y�leri zemine g�re ayarlayabilirim.
                createdBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePiece.transform.position = newPiecePosition;
            }
        }

    }

    private void OnTriggerEnter(Collider other)//scritin ba�l� oldu�u prefab�n(yani playerin) ba�ka bir prefaba �arpmas�n� kontrol eden fonksiyondur.
    {
        if (other.tag=="addCylinder")//e�er �arpt��� prefab�n etiketinin ismi"addClinder" ise :
        {
            IncrementCylinderValume(0.1f);
            Destroy(other.gameObject);//�arp��t���m�z objeyi sil demek.yani silindiri silecek.
        }
        else if (other.tag=="SpawnBridge")
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag=="StopSpawnBridge")
        {
            StopSpawninBridge();
        }
    }

    private void OnTriggerStay(Collider other) //�steki fonksiyon ile ayn� �ekilde �al���r fakat engellerde kald��� s�rece silindirler silinecek.
    {
        if (other.tag=="Trap")
        {
            IncrementCylinderValume(-Time.fixedDeltaTime);
        }
    }

    public void IncrementCylinderValume(float value)
    {
        if (cylinders.Count==0)//e�er cylinders listesi eleman� 0 ra e�itse
        {
            if (value>0)//artt�rma miktar� 0 dan b�y�kse(yani cylinder artt�r�l�yorsa
            {
                CreateCylinder(value);//o zaman silindir yarat
            }
            else//e�er miktar azalt�lmaya �al���yorsa ve playerde silindir yoksa
            {
                //GameOver
            }
        }
        else//ko�ulun tesi ise: playerin alt�ndaki silindirin hacmini g�ncelleyece�iz.yani silindirin en son indeksteki silindiri g�ncelleyece�iz.
        {
            cylinders[cylinders.Count - 1].IncrementCylinderValume(value);//cylinders listesindeki son indeksi(cylinders[cylinders.Count - 1]) inc** fonk ile �ag�r�yoruz.
        }

    }
    
    public void CreateCylinder(float value)//yeni bir silindir yaratmam�z� sa�layacak.(playerin aya��n�n alt�nda)
    {
        RidingCylinder createdCylinder = Instantiate(RidingCylinderPrefab, transform).GetComponent<RidingCylinder>();//Cylinder Prefab� al�p playere ekliyoruz(yani oyunda bir sdsilindir yarat�yoruz)
        cylinders.Add(createdCylinder);//yaratt���m�z silindiri silindirler listesine ekliyoruz
        createdCylinder.IncrementCylinderValume(value);//yarat�lm�� olan silindirin boyutunu belirleme;

    }

    public void DestroyCylinder(RidingCylinder cylinder)//silindirleri yok etme metodu. eidingCylinder s�n�f�nda kullan�lacak.
    {
        cylinders.Remove(cylinder);//�nce silindiri silindirler listesinden siliyoruz
        Destroy(cylinder.gameObject);   //sonra silindir objesini siliyoruz.
    }

    public void StartSpawningBridge(BridgeSpawner spawner)//k�tr� yaratmaya ba�lamam�z� sa�layacak fonk
    {
        _bridgeSpawner = spawner;//referans noktalar�na eri�tik
        _spawningBridge = true;//k�pr� yap�p yapmayaca��m�z de�i�keni true yaplal�y�z ona g�re �al��t�raca�z ��nk�
    }

    public void StopSpawninBridge()//k�pr� ba�latmay� durduracak fonk
    {
        _spawningBridge = false; // sadece de�i�keni false yapaca��z.
    }

}
