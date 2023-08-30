using Mirror;
using TMPro;
using UnityEngine;

public class G2C_PlayerScript : NetworkBehaviour
{

    public GameObject floatinfInfo;

    public TextMesh NameText;

    private Material PlayerMaterialClone;

    [SyncVar(hook = nameof(OnPlayerNameCHange))]
    private string playerName;

    [SyncVar(hook = nameof(OnPlayerColorChange))]
    private Color playerColor;

    private void OnPlayerNameCHange(string oldstr, string newStr) 
    {
        NameText.text = newStr;
    }

    private void OnPlayerColorChange(Color oldColor, Color newColor) 
    {
        NameText.color = newColor;

        PlayerMaterialClone = new Material(GetComponent<Renderer>().material);

        PlayerMaterialClone.SetColor("_EmissionColor", newColor);

        GetComponent<Renderer>().material = PlayerMaterialClone;
    }

    [Command]
    private void CmdSetupPlayer(string nameValue, Color colorValue) 
    {
        playerName = nameValue;
        playerColor = colorValue;
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition= Vector3.zero;

        floatinfInfo.transform.localPosition = new Vector3(0.0f, -.3f, .6f);
        floatinfInfo.transform.localScale = new Vector3(.1f, .1f, .1f);

        ChangePlayerNameAndColor();
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            floatinfInfo.transform.LookAt(Camera.main.transform);
            return;
        }

        var movex = Input.GetAxis("Horizontal") * Time.deltaTime * 110f;

        var movez = Input.GetAxis("Vertical") * Time.deltaTime * 4.0f;

        transform.Rotate(0, movex, 0);

        transform.Translate(0, 0, movez);

        if (Input.GetKeyDown(KeyCode.C)) 
        {
            ChangePlayerNameAndColor();
        }
    }

    private void ChangePlayerNameAndColor()
    {
        var tempName = $"Player {Random.Range(0, 999)}";

        var tempColor = new Color
        (
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            1
        );

        CmdSetupPlayer(tempName, tempColor);
    }
}
