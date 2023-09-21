using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //1.1
    public static PlayerController Current;//diðer silindirlerin playere eriþebilmesi için bir deðiþken yaratýyoruz.


    public GameObject RidingCylinderPrefab;//silindir prefabumu tutacak bir deiþken yaratýyoruz.
    public List<RidingCylinder> cylinders; //playerin altýndaki silindirleri tutmak için bir lis oluþturduk

    public float limitX; // player ekrandan çýkmamasý için sýnýrlama.platformdan dýþarý çýkmamalý. Bunlarý unityde ayarlayacaðz
    public float runningSpeed;//koþma hýzý
    public float XSpeed;//Saða sola ne kadar hýzla gidecek
    private float _currentRunningSpeed;//koþma hýzý

    private bool _spawningBridge;
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;

    private float _creatingBridgeTimer;

    // Start is called before the first frame update
    void Start()
    {
        Current = this;//deðiþkeni þu anda baðlý olduðumuz playere eþitledik.aktardýk.
        _currentRunningSpeed = runningSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float newX = 0;
        float touchXDelta = 0;
        // þimdi if else ile oyuncunun pc demi mobildemi olduðunu taspit etmeliyiz. yani ekrana mý dokundu yoksa fareylemi dokundu
        //sonra ilk dokunduktan itibaren x yörüngesinde ne kadar kaydýrdýðýný bulacaðýz.
        if (Input.touchCount > 0 && Input.GetTouch(0).phase==TouchPhase.Moved)//mobil için belirliyoruz
        {
            touchXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;
            // burada ilk dokunuþtan itibaren x ekseninde parmaðý ne kadar kaydýrdýðýný buluyoruz ve bunu ekran boyutuna bölüyoruz. çünkü bölmesek ne kadar ekran küçükse o kadar x ekseni hassaslaþýr.
        }
        else if (Input.GetMouseButton(0))//pc için ayný þeyi yapýyoz
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + XSpeed * touchXDelta * Time.deltaTime;  //burada parmak ne kadar kaymýþsa o kadar player saða sola kayar.
        newX = Mathf.Clamp(newX, -limitX, limitX);  // burada platformdan düþmesin diye saðda ve solda limitlendiriyoruz.

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        // burada karakterin pozisyonunu bulunduran deðiþken yazdým ve hýzýný ve kuvvetini ayarlamak için _curr*** deðiþkenini denkleme ekledim
        //böylelikle _curr** deðiþkenini arttýrdýðýmda player hareket edecektir
        transform.position = newPosition;
        // yaptýðým deðiþkeni playerin pozisyonuna eþitledim

        if (_spawningBridge)
        {
            _creatingBridgeTimer -= Time.deltaTime;
            if (_creatingBridgeTimer<0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderValume(-0.01f);
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position; //iki referas nokta arasýndaki yön vektörünü elde ettik
                float distance = direction.magnitude;//iki referans noktasý arasýndaki mesafeyi veriyo
                direction = direction.normalized; //bu iki kodla artýk verilen yöleri zemine göre ayarlayabilirim.
                createdBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePiece.transform.position = newPiecePosition;
            }
        }

    }

    private void OnTriggerEnter(Collider other)//scritin baðlý olduðu prefabýn(yani playerin) baþka bir prefaba çarpmasýný kontrol eden fonksiyondur.
    {
        if (other.tag=="addCylinder")//eðer çarptýðý prefabýn etiketinin ismi"addClinder" ise :
        {
            IncrementCylinderValume(0.1f);
            Destroy(other.gameObject);//çarpýþtýðýmýz objeyi sil demek.yani silindiri silecek.
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

    private void OnTriggerStay(Collider other) //üsteki fonksiyon ile ayný þekilde çalýþýr fakat engellerde kaldýðý sürece silindirler silinecek.
    {
        if (other.tag=="Trap")
        {
            IncrementCylinderValume(-Time.fixedDeltaTime);
        }
    }

    public void IncrementCylinderValume(float value)
    {
        if (cylinders.Count==0)//eðer cylinders listesi elemaný 0 ra eþitse
        {
            if (value>0)//arttýrma miktarý 0 dan büyükse(yani cylinder arttýrýlýyorsa
            {
                CreateCylinder(value);//o zaman silindir yarat
            }
            else//eðer miktar azaltýlmaya çalýþýyorsa ve playerde silindir yoksa
            {
                //GameOver
            }
        }
        else//koþulun tesi ise: playerin altýndaki silindirin hacmini güncelleyeceðiz.yani silindirin en son indeksteki silindiri güncelleyeceðiz.
        {
            cylinders[cylinders.Count - 1].IncrementCylinderValume(value);//cylinders listesindeki son indeksi(cylinders[cylinders.Count - 1]) inc** fonk ile çagýrýyoruz.
        }

    }
    
    public void CreateCylinder(float value)//yeni bir silindir yaratmamýzý saðlayacak.(playerin ayaðýnýn altýnda)
    {
        RidingCylinder createdCylinder = Instantiate(RidingCylinderPrefab, transform).GetComponent<RidingCylinder>();//Cylinder Prefabý alýp playere ekliyoruz(yani oyunda bir sdsilindir yaratýyoruz)
        cylinders.Add(createdCylinder);//yarattýðýmýz silindiri silindirler listesine ekliyoruz
        createdCylinder.IncrementCylinderValume(value);//yaratýlmýþ olan silindirin boyutunu belirleme;

    }

    public void DestroyCylinder(RidingCylinder cylinder)//silindirleri yok etme metodu. eidingCylinder sýnýfýnda kullanýlacak.
    {
        cylinders.Remove(cylinder);//önce silindiri silindirler listesinden siliyoruz
        Destroy(cylinder.gameObject);   //sonra silindir objesini siliyoruz.
    }

    public void StartSpawningBridge(BridgeSpawner spawner)//kötrü yaratmaya baþlamamýzý saðlayacak fonk
    {
        _bridgeSpawner = spawner;//referans noktalarýna eriþtik
        _spawningBridge = true;//köprü yapýp yapmayacaðýmýz deðiþkeni true yaplalýyýz ona göre çalýþtýracaðz çünkü
    }

    public void StopSpawninBridge()//köprü baþlatmayý durduracak fonk
    {
        _spawningBridge = false; // sadece deðiþkeni false yapacaðýz.
    }

}
