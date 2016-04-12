using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour {

    public Canvas GameOverCanvas;
    public Button exitText;
    public Button restartText;

    // Use this for initialization
    void Start()
    {

        GameOverCanvas = GameOverCanvas.GetComponent<Canvas>();

        exitText = exitText.GetComponent<Button>();
        restartText = restartText.GetComponent<Button>();

        GameOverCanvas.GetComponent<Image>().enabled = false;
        exitText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);


    }
    public void Won()

    {
        //GameObject.Find("pause").gameObject.SetActive(false);
        GameOverCanvas.GetComponent<Image>().enabled = true;
        exitText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        //GameObject.Find("Player").GetComponent<player>().canFly = false;
        Time.timeScale = 0.0f;
        restartText.Select();
    }

    public void StartDance()
	{

        Time.timeScale = 1.0f;
        GameObject[] g;//ghosts
        GameObject[] pl;//party lights
        GameObject[] l;//normal lights

        g = GameObject.FindGameObjectsWithTag("Ghosts");
        pl = GameObject.FindGameObjectsWithTag("PartyLights");
        l = GameObject.FindGameObjectsWithTag("Lights");

        GameObject.Find("PartyMusic").GetComponent<AudioSource>().Play();
        GameObject.Find("Background Music").GetComponent<AudioSource>().Stop();

        foreach (GameObject go in g)
        {
            //Debug.Log(go.name);
            if(go.GetComponent<GhostDance>() != null)
                go.GetComponent<GhostDance>().ghosrDanceProtocoll();
        }

        foreach (GameObject go in pl)
        {
            go.GetComponent<PartyLights>().turnOnLights();
        }

        foreach (GameObject go in l)
        {
            if(!go.name.Equals("TopLight"))
                go.GetComponent<Light>().enabled = false;
        }

        GameObject.Find("Player").GetComponent<Animator>().SetBool("party", true);

        GameOverCanvas.enabled = false;
        
    }

	public void ExitLevel()
	{
        SceneManager.LoadScene(0);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
