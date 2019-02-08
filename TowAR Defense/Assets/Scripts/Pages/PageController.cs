using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Add all pages here;
// All pages are instantiated from the unity editor.
// When the GameLobbyScene loads, the DefaultPage is displayed.
// DefaultPage can be replaced with the OverrideDefault method.
public class PageController : MonoBehaviour
{
    public static PageController instance = null;

    // All pages are defined here.
    public GameObject MainPage;
    public GameObject CreateRoomPage;
    public GameObject JoinRoomPage;
    public GameObject GamePendingPage;
    public GameObject PauseGamePage;
    public GameObject EndGamePage;

    // Hash containing pages for easy lookup in the OverrideDefault Method
    private Dictionary<string,GameObject> Pages = new Dictionary<string, GameObject>();

    private static GameObject DisplayedPage;
    private static string DisplayedPageName;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Destroy(this.gameObject);
        }

        // Instantiate the Pages Hash and DefaultPage
        Pages.Add("MainPage", MainPage);
        Pages.Add("CreateRoomPage", CreateRoomPage);
        Pages.Add("JoinRoomPage", JoinRoomPage);
        Pages.Add("GamePendingPage", GamePendingPage);
        Pages.Add("PauseGamePage", PauseGamePage);
        Pages.Add("EndGamePage", EndGamePage);

        DisplayedPage = MainPage;
        DisplayedPageName = "MainPage";
        DisplayedPage.SetActive(true);

        foreach(var Page in Pages)
        {
            if (DisplayedPageName != Page.Key)
            {
                Page.Value.SetActive(false);
            }
        }
    }


    public void OverrideDefault(string Page)
    {
        DisplayedPage = Pages[Page];
        if (DisplayedPage == null)
        {
            throw new System.Exception("Default Page Cannot Be Null");
        }
    }

    public void SwitchPage(string Page)
    {
        GameObject newPage = Pages[Page];

        if (newPage == null)
        {
            throw new System.Exception("Give Page Name " + Page + " Does not Exist");
        } else {
            GetComponent<AudioSource>().Play();
            DisplayedPage.SetActive(false);
            DisplayedPage = newPage;
            DisplayedPage.SetActive(true);
        }
    }

    // Switch handles
    public void HandleSwitchToMainPage()    
    {
        SwitchPage("MainPage");
    }
    public void HandleSwitchToCreatePage()
    {
        SwitchPage("CreateRoomPage");
    }

    public void HandleSwitchToJoinPage()
    {
        SwitchPage("JoinRoomPage");
    }

    public void HandleSwitchToGamePendingPage()
    {
        SwitchPage("GamePendingPage");
    }

    public void HandleSwitchToPauseGamePagee()
    {
        SwitchPage("PauseGamePage");
    }

    public void HandleSwitchToEndGamePage()
    {
        SwitchPage("EndGamePage");
    }
}
