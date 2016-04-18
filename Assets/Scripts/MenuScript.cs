
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class MenuScript : MonoBehaviour {
	public Button startText;
	//public Button exitText;
	public Button exitMenuText;
	public Button ControlsText;
    public Button floodButton;
    public Button doubleButton;
	public Canvas ControlMenu;
	public Canvas MainCanvas;
    public Image loading;
	float timer = 0f;
	// Use this for initialization
	void Start () {
		ControlMenu.enabled = false;
		startText = startText.GetComponent<Button> ();
		startText.image.enabled = false;
		ControlsText.image.enabled = false;
		exitMenuText = exitMenuText.GetComponent<Button> ();
		startText.image.enabled = true;
		ControlsText.image.enabled = true;
        loading.enabled = false;
		startText.Select ();

	}
	public void ExitPress()
	{
		startText.Select (); // exits controls screen and reselects startText
		ControlMenu.enabled = false;

    }

	public void StartLevel()
	{
        SceneManager.LoadScene(1);
        GameModeControl.mode = 0;
        loading.enabled = true;

    }

    public void StartTutorial()
    {
        SceneManager.LoadScene(2);
        GameModeControl.mode = 0;
    }

    public void StartFlood()
    {
        SceneManager.LoadScene(1);
        GameModeControl.mode = 1;
        loading.enabled = true;
    }
    public void StartDouble()
    {
        SceneManager.LoadScene(1);
        GameModeControl.mode = 2;
        loading.enabled = true;
    }
    public void ShowControl()
	{
		ControlMenu.enabled = true;
        MainCanvas.enabled = false;
		exitMenuText.Select ();

	}
	public void HideControl()
	{
		ControlMenu.enabled = false;
        MainCanvas.enabled = true;
		startText.Select ();

    }
	public void ExitGame()
	{
		Application.Quit();
	}
	
	// Update is called once per frame
	void Update () {
		/*timer += Time.deltaTime; 
		if (timer > 5f) {

			SplashCanvas.enabled=false;
			startText.image.enabled = true;
			ControlsText.image.enabled = true;

		}*/
	}
}
