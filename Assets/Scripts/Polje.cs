using UnityEngine;

public class Polje : MonoBehaviour
{
    public int Red;
    public int Stupac;
    private bool _zauzeto = false;
    public bool Zauzeto => _zauzeto;

    public void PostaviZnak(string znak, GameObject prefab)
    {
        _zauzeto = true;
        Vector3 pozicija = new Vector3(transform.position.x, 0.03f, transform.position.z);
        Instantiate(prefab, pozicija, Quaternion.identity);
        Debug.Log("Polje " + Red + "," + Stupac + " = " + znak);
    }

    public void Resetiraj()
    {
        _zauzeto = false;
    }
}