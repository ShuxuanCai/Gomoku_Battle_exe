using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public enum GameState
{
    Ready = 1,
    GameOver = 2
}

public class GomokuNetworkManager : MonoBehaviourPunCallbacks
{
    public Text ReadyButtonText;
    public Text ourSideText;
    public Text ourSideReadyText;
    public Text otherSideText;
    public Text otherSideReadyText;
    public Text turnText;
    public Text GameOverText;
    public Text WinText;

    public GameObject player;
    public PieceColor playerTurn = PieceColor.Black;
    public GameState gameState = GameState.Ready;
    public AudioSource audioSource;
    public AudioSource BGMaudioSource;

    public Button soundButton;
    public Sprite SoundImage;
    public Sprite CloseSoundImage;

    // Start is called before the first frame update
    void Start()
    {
        SetUIState();

        // Connect Server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // Create or join room
        base.OnConnectedToMaster();
        print("OnConnectedToMaster Successfully!");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("GomokuRoom", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        // Create online player
        base.OnJoinedRoom();
        print("OnJoinedRoom Successfully!");

        if (player == null) return;
        GameObject newPlayer = PhotonNetwork.Instantiate(player.name, Vector3.zero, player.transform.rotation);

        // Instantiate attributes of player
        if(PhotonNetwork.IsMasterClient)
        {
            newPlayer.GetComponent<PhotonView>().RPC("SetPieceColor", RpcTarget.All, PieceColor.Black);
        }
        else
        {
            newPlayer.GetComponent<PhotonView>().RPC("SetPieceColor", RpcTarget.All, PieceColor.White);
        }
    }

    [PunRPC]
    public void ChangeTurn()
    {
        if (playerTurn == PieceColor.Black)
        {
            playerTurn = PieceColor.White;
            turnText.text = "White";
        }
        else
        {
            playerTurn = PieceColor.Black;
            turnText.text = "Black";
        }


    }

    [PunRPC]
    public void GameOver(PieceColor pieceColor)
    {
        if(GameOverText && WinText)
        {
            GameOverText.gameObject.SetActive(true);
            if (pieceColor == PieceColor.Black)
                WinText.text = "Black Win";
            else
                WinText.text = "White Win";
            WinText.gameObject.SetActive(true);
        }
        gameState = GameState.GameOver;
    }

    public void PlaySoundEffect()
    {
        if (audioSource == null) return;
        audioSource.Play();
    }

    public void CloseAndPlaySoundEffect()
    {
        if (BGMaudioSource.isPlaying)
        {
            BGMaudioSource.Stop();
            soundButton.image.sprite = CloseSoundImage;
        }
        else
        {
            BGMaudioSource.Play();
            soundButton.image.sprite = SoundImage;
        }
    }

    public void OnClickReadyButton()
    {
        ReadyButtonText.text = "Start";
        var players = GameObject.FindObjectsOfType<GomokuPlayerController>();
        foreach(var item in players)
        {
            if(item.GetComponent<PhotonView>().IsMine)
                item.GetComponent<PhotonView>().RPC("SetReadyAndStart", RpcTarget.All);
        }
    }

    void SetUIState()
    {
        ReadyButtonText.text = "Ready";
        ourSideText.text = "";
        ourSideReadyText.text = "";
        otherSideText.text = "";
        otherSideReadyText.text = "";
        turnText.text = "";
        GameOverText.gameObject.SetActive(false);
        WinText.gameObject.SetActive(false);
    }

    public void SetOurSideText(PieceColor pieceColor)
    {
        if(pieceColor == PieceColor.Black)
        {
            ourSideText.text = "Black";
            ourSideReadyText.text = "Ready";
        }
        else
        {
            ourSideText.text = "White";
            ourSideReadyText.text = "Ready";
        }
    }

    public void SetOtherSideText(PieceColor pieceColor)
    {
        if (pieceColor == PieceColor.Black)
        {
            otherSideText.text = "Black";
            otherSideReadyText.text = "Ready";
        }
        else
        {
            otherSideText.text = "White";
            otherSideReadyText.text = "Ready";
        }
    }
}
