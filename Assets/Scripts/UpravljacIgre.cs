using IksOks.GameLogic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpravljacIgre : MonoBehaviour
{
    public static UpravljacIgre Instanca;

    private Igra _igra;
    private bool _xNaPotezu = true;
    private bool _krajanigre = false;
    private bool _xPocinje = true;
    public GameObject XPrefab;
    public GameObject OPrefab;
    private bool _prvaPokreta = true;
    public TMP_Text StatusTekst;
    public TMP_Text RezultatTekst;
    public Button NovaIgraGumb;
    private int _scoreX = 0;
    private int _scoreO = 0;
    private int _scoreDraw = 0;

    private void Awake()
    {
        Instanca = this;
    }

    void Start()
    {
        _prvaPokreta = true;
        NovaIgraGumb.onClick.AddListener(NovaIgra);
        NovaIgra();
    }

    public void NovaIgra()
    {
        // Obriši sve X i O objekte iz scene
        GameObject[] xObjekti = GameObject.FindGameObjectsWithTag("X");
        GameObject[] oObjekti = GameObject.FindGameObjectsWithTag("O");
        foreach (var obj in xObjekti) Destroy(obj);
        foreach (var obj in oObjekti) Destroy(obj);

        _igra = new Igra();
        _igra.PokreniPartiju();
        _krajanigre = false;

        if (_prvaPokreta)
        {
            _xNaPotezu = true;
            _xPocinje = true;
            _prvaPokreta = false;
        }
        else
        {
            _xPocinje = !_xPocinje;
            _xNaPotezu = _xPocinje;
        }

        // Resetiraj sva polja
        Polje[] svaPolja = FindObjectsOfType<Polje>();
        foreach (var polje in svaPolja) polje.Resetiraj();

        AzurirajUI();
    }
    private void Update()
    {
        // Mouse (PC)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 pozicija = Mouse.current.position.ReadValue();
            ObradiDodirIliKlik(pozicija);
        }

        // Touch (mobitel)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 pozicija = Touchscreen.current.primaryTouch.position.ReadValue();
            ObradiDodirIliKlik(pozicija);
        }
    }

    private void ObradiDodirIliKlik(Vector2 pozicija)
    {
        Ray zraka = Camera.main.ScreenPointToRay(pozicija);
        RaycastHit pogodak;

        if (Physics.Raycast(zraka, out pogodak))
        {
            Polje polje = pogodak.transform.GetComponent<Polje>();
            if (polje != null)
            {
                ObradiKlik(polje.Red, polje.Stupac, pogodak.transform.gameObject);
            }
        }
    }

    public void ObradiKlik(int red, int stupac, GameObject poljeObjekt)
    {
        if (_krajanigre) return;

        string[] stupci = { "A", "B", "C" };
        string koordinate = stupci[stupac] + (red + 1).ToString();

        Znak znak;
        if (_xNaPotezu) { znak = new Iks(); }
        else { znak = new Oks(); }
        znak.Upisi(koordinate);

        bool uspjeh = _igra.UnosZnaka(znak);
        if (!uspjeh) return;

        Polje poljeSkripte = poljeObjekt.GetComponent<Polje>();
        poljeSkripte.PostaviZnak(_xNaPotezu ? "X" : "O", _xNaPotezu ? XPrefab : OPrefab);

        // Provjeri pobjednika
        string rezultat = _igra.ProvjeraTablice();

        if (rezultat == "X")
        {
            _scoreX++;
            _krajanigre = true;
            StatusTekst.text = "POBIJEDIO X!";
            RezultatTekst.text = $"X: {_scoreX}  |  REMI: {_scoreDraw}  |  O: {_scoreO}";
            return;
        }
        else if (rezultat == "O")
        {
            _scoreO++;
            _krajanigre = true;
            StatusTekst.text = "POBIJEDIO O!";
            RezultatTekst.text = $"X: {_scoreX}  |  REMI: {_scoreDraw}  |  O: {_scoreO}";
            return;
        }
        else if (FindObjectsOfType<Polje>().All(p => p.Zauzeto))
        {
            _scoreDraw++;
            _krajanigre = true;
            StatusTekst.text = "REMI!";
            RezultatTekst.text = $"X: {_scoreX}  |  REMI: {_scoreDraw}  |  O: {_scoreO}";
            return;
        }

        _xNaPotezu = !_xNaPotezu;
        AzurirajUI();
    }
    private void AzurirajUI()
    {
        if (_xNaPotezu)
            StatusTekst.text = "NA POTEZU: X";
        else
            StatusTekst.text = "NA POTEZU: O";

        RezultatTekst.text = $"X: {_scoreX}  |  REMI: {_scoreDraw}  |  O: {_scoreO}";
    }
}