using UnityEngine;
using UnityEngine.Assertions;

public class Magnetizable : MonoBehaviour
{
    public bool magnetActive = true;

    MagnetData magnetData;
    public Rigidbody2D rigidBody;

    public Magnetizable nextMagnet;
    public Magnetizable magnetizedTo;

    public Transform posMag;
    public Transform negMag;

    // Grabs MagnetData (ScriptableObject) and parents RigidBody2D (Required)
    private void Awake()
    {
        magnetData = Resources.Load<MagnetData>("ScriptableObjects/MagnetData");
        magnetData.nonMagnetizedList.Add(this);
        rigidBody = transform.parent.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rigidBody, "No RigidBody2D component was found on parent object.");

        Assert.IsTrue(gameObject.transform.childCount > 0);
        posMag = gameObject.transform.GetChild(0);
        if(gameObject.transform.childCount > 1) // Player has no negative magnet
            negMag = gameObject.transform.GetChild(1);
    }

    // Gets last magnetized object in the chain and demagnetizes each object until it reaches this magnetizable
    public void DeMagnetize()
    {
        if (rigidBody == null) // Sometimes tries to demagnetize objects undergoing Destroy
            return;
        if(magnetData.magnetizedList.Count == 0)
        {
            return;
        }
        Magnetizable _last = magnetData.magnetizedList.GetLast();
        while(_last != this)
        {
            _last.rigidBody.velocity = Vector2.zero;
            _last.magnetActive = false;
            magnetData.RemoveMagnetizable(_last);
            _last.magnetizedTo = null;
            _last.nextMagnet = null;

            if (magnetData.magnetizedList.Count == 0)
                break;
            _last = magnetData.magnetizedList.GetLast();
        }
        rigidBody.velocity = Vector2.zero;
        magnetActive = false;
        magnetData.RemoveMagnetizable(this);
        if (magnetizedTo != null)
            magnetizedTo.nextMagnet = null;
        magnetizedTo = null;
        nextMagnet = null;
    }

    // Attach object to last object in chain
    public void Magnetize()
    {
        AudioManager.instance.Play("Pickcargo");
        magnetActive = true;
        Magnetizable _mag = magnetData.magnetizedList.GetLast();
        magnetizedTo = _mag;
        _mag.nextMagnet = this;
        magnetData.AddMagnetizable(this);
    }
    
    // Attach object to target object
    public void Magnetize(Magnetizable toMagnet)
    {
        AudioManager.instance.Play("Pickcargo");
        magnetActive = true;
        magnetizedTo = toMagnet;
        toMagnet.nextMagnet = this;
        magnetData.AddMagnetizable(this);
    }


}
