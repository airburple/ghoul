using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Steamworks;

public class spawnGlobal : MonoBehaviour {

    // Use this for initialization
    int totalPatrons;
    int killedPatrons;
    int waveCount;
    public int totalWaves;
    public int momRatio;
    public int dadRatio;
    int enemySpawnCount;
    int score;
    int momCount = 0;
    int dadCount = 0;
    int kidCount = 0;
    int momWait = 0;
    int dadWait = 0;
    public float failTime;
    public float wavePrepTime;
    float waveTimer;
    float timer;
    float deathTimer;
    bool dead = false;
    bool timing;

    //spawn timers for the different spawn points
    float kitchenTimer;
    float mainTimer;
    float windowTimer;
    float bathroomTimer;

    //enemy at each point will spawn every set number of seconds
    public float kitchenRate;
    public float mainRate;
    public float windowRate;
    public float bathroomRate;
    //wave for each spawn to start making enemies
    public int kitchenStart;
    public int mainStart;
    public int windowStart;
    public int bathroomStart;
    int currSpawn;

    //UI elements
    Text timerText;
    Text waveText;
    Text scoreText;
    Text kidText;
    Text momText;
    Text dadText;
    GameOverScreen GO;
    WinScreen winScreen;
    Image flameHealth;

    Image Ready;
    Image Wave;
    Image waveNumber;
    Image readyNumber;

    Sprite[] num;
    Sprite[] numDesat;

    //number of each enemy type to spawn on each wave
    int[] waveEnemyCountKid;
    int[] waveEnemyCountMom;
    int[] waveEnemyCountDad;

	public AudioClip collectfearsound;
	private AudioSource source; 

    spawnAI ms;
    spawnAI ks;
    spawnAI ws;
    spawnAI bs;

    //variables for powerup tracking
    int roomScareCurrent;
    public int roomScareMax;
    int stopTimeCurrent;
    public int stopTimeMax;
    int speedCurrent;
    public int speedMax;
    public float stopTimeLength;
    float stopTimeWindow;
    public float speedUpPower;
    public float speedLength;
    float speedWindow;
    bool fast;
    public float speedMod;
    Image speedUI;
    Image roomScareUI;
    Image stopTimeUI;

    //some global things for easy access
    player Ghoul;
    Cam mainCam;

    bool won;

    void Start () {
        timerText = GameObject.Find("WaveTimeUI").GetComponent<Text>();
        kidText = GameObject.Find("GirlCount").GetComponent<Text>();
        momText = GameObject.Find("MomCount").GetComponent<Text>();
        dadText = GameObject.Find("DadCount").GetComponent<Text>();
        GO = GameObject.Find("gameover").GetComponent<GameOverScreen>();
        winScreen = GameObject.Find("WinScreen").GetComponent<WinScreen>();
        flameHealth = GameObject.Find("SkullFlame").GetComponent<Image>();
        Ready = GameObject.Find("Ready").GetComponent<Image>();
        Wave = GameObject.Find("Wave").GetComponent<Image>();
        waveNumber = GameObject.Find("waveNumber").GetComponent<Image>();
        readyNumber = GameObject.Find("readyNumber").GetComponent<Image>();
        source = GetComponent<AudioSource>();
        ms = GameObject.Find("MainSpawn").GetComponentInChildren<spawnAI>();
        ks = GameObject.Find("KitchenSpawn").GetComponentInChildren<spawnAI>();
        ws = GameObject.Find("WindowSpawn").GetComponentInChildren<spawnAI>();
        bs = GameObject.Find("BathroomSpawn").GetComponentInChildren<spawnAI>();
        Ghoul = GameObject.Find("Player").GetComponent<player>();
        stopTimeUI = GameObject.Find("timePower").GetComponent<Image>();
        speedUI = GameObject.Find("speedPower").GetComponent<Image>();
        roomScareUI = GameObject.Find("scarePower").GetComponent<Image>();
        mainCam = GameObject.Find("Camera").GetComponent<Cam>();

        score = 0;
        waveCount = -1;
        totalWaves = 10;
        momWait = 0;
        dadWait = 0;
        timing = true;
        won = false;

        timer = -wavePrepTime;
        waveTimer = timer;
        kitchenTimer = timer;
        mainTimer = timer;
        windowTimer = timer;
        bathroomTimer = timer;
        currSpawn = 0;

        roomScareCurrent = 0;

        stopTimeCurrent = 0;

        speedCurrent = 0;

        speedUI.fillAmount = (float)speedCurrent / (float)speedMax;
        roomScareUI.fillAmount = (float)roomScareCurrent / (float)roomScareMax;
        stopTimeUI.fillAmount = (float)stopTimeCurrent / (float)stopTimeMax;

        fast = false;

        if (GameModeControl.mode == 2)
            Time.timeScale = speedMod;
        else
            Time.timeScale = 1;

        totalWaves = 10;

        populateEnemy(GameModeControl.mode);

        num = new Sprite[10];
        numDesat = new Sprite[10];

        for(int i = 0; i < 10; i++)
        {
            num[i] = Resources.Load<Sprite>("WaveUI/WaveUI/" + (i+1));
            numDesat[i] = Resources.Load<Sprite>("WaveUI/WaveUI/" + (i+1) + "desat");

            
        }

        //num[0] = Resources.Load<Sprite>("WaveUI/WaveUI/" + (1));
        //numDesat[0] = Resources.Load<Sprite>("WaveUI/WaveUI/" + (1) + "desat");



        spawnNextWave();

    }

    // Update is called once per frame
    void Update()
    {
        if(!won)
        { 
            if (!dead)
            {
                timer += Time.deltaTime;
                if (timer > stopTimeWindow && !timing)
                    startTimer();
                if (timer > speedWindow && fast)
                    slowPlayer();
                if (timing)
                {
                    waveTimer += Time.deltaTime;
                    kitchenTimer += Time.deltaTime;
                    mainTimer += Time.deltaTime;
                    windowTimer += Time.deltaTime;
                    bathroomTimer += Time.deltaTime;
                }
            }
            //prep timer



            if (waveTimer < 0)
            {
                timerText.text = "00";
                Sprite us = num[(int)(-waveTimer)];
                //Debug.Log(us.name);
                readyNumber.sprite = us;

            }
            else//timer on the wave
            {
                if (Wave.IsActive())
                {
                    Wave.CrossFadeAlpha(0f, 0.2f, true);
                    Ready.CrossFadeAlpha(0f, 0.2f, true);
                    readyNumber.CrossFadeAlpha(0f, 0.2f, true);
                    waveNumber.CrossFadeAlpha(0f, 0.2f, true);
                }


                int curr = (int)(failTime - waveTimer);
                if (curr > 9)
                    timerText.text = "" + curr;
                else
                {
                    if (curr > 0)
                        timerText.text = "0" + curr;
                    else
                        timerText.text = "00";
                }
                flameHealth.fillAmount = 1 - (waveTimer / failTime);

            }

            if (waveCount > totalWaves && !dead)//set win condition wave
            {
                dead = true;
                winFunction();//replace with win function later for final scene
            }
            if (killedPatrons >= enemySpawnCount && waveTimer > 0 && !dead)
            {
                source.PlayOneShot(collectfearsound, .5f);
                spawnNextWave();
            }

            if (waveTimer >= failTime)
            {
                dead = true;
                if (deathTimer == 0)
                {
                    GameObject player = GameObject.Find("Player");
                    player.GetComponent<player>().killPlayer();
                    //re-enable player
                    SkinnedMeshRenderer[] skins = player.GetComponentsInChildren<SkinnedMeshRenderer>();//turn on mesh renderer
                    foreach (SkinnedMeshRenderer s in skins)
                    {
                        s.enabled = true;
                    }
                    //player.GetComponentInChildren<MeshRenderer>().enabled = true;
                    player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().enabled = true;//turn on control
                    player.GetComponent<Rigidbody>().isKinematic = false;//unfix
                    player.GetComponent<player>().control = true;
                    player.GetComponent<CapsuleCollider>().enabled = true;//turn on collider
                    player.GetComponent<posess>().one = false;//enable posession of another object

                }
                deathTimer += Time.deltaTime;
                AnimatorStateInfo state = GameObject.Find("Player").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
                if (deathTimer > 4 && dead)
                {
                    failFunction();
                    waveTimer = 0;

                }
            }
            checkSpawns();
        }

    }

    void checkSpawns()
    {
        if (totalPatrons < enemySpawnCount)
        {
            if (kitchenTimer >= kitchenRate )
            {
                if (waveCount >= kitchenStart)
                {
                    ks.genAI();
                    kidCount++;
                    kidText.text = "" + kidCount;
                    kitchenTimer = 0;
                    totalPatrons++;
                    checkMom(ks);
                    checkDad(ks);
                    currSpawn++;
                    
                }

            }

        }
        if (totalPatrons < enemySpawnCount)
        {
            if (mainTimer >= mainRate )
            {
                if (waveCount >= mainStart)
                {
                    ms.genAI();
                    kidCount++;
                    kidText.text = "" + kidCount;
                    mainTimer = 0;
                    totalPatrons++;
                    checkMom(ms);
                    checkDad(ms);
                    currSpawn++;
                }


            }
        }
        if (totalPatrons < enemySpawnCount)
        {
            if (windowTimer >= windowRate )
            {
                if (waveCount >= windowStart)
                {
                    ws.genAI();
                    kidCount++;
                    kidText.text = "" + kidCount;
                    windowTimer = 0;
                    totalPatrons++;
                    checkMom(ws);
                    checkDad(ws);
                    currSpawn++;
                }

            }
        }

        if (totalPatrons < enemySpawnCount)
        {
            if (bathroomTimer >= bathroomRate )
            {
                if (waveCount >= bathroomStart)
                {
                    bs.genAI();
                    kidCount++;
                    kidText.text = "" + kidCount;
                    bathroomTimer = 0;
                    totalPatrons++;
                    checkMom(bs);
                    checkDad(bs);
                    currSpawn = 0;
                }

            }
        }
    }

    void checkMom(spawnAI s)
    {
        momWait++;//might be able to get rid of this wait and ratio stuff
        if (waveEnemyCountMom[waveCount] > momCount)
        {
            s.genMom();
            momWait -= momRatio;
            momCount++;
            momText.text = "" + momCount;
            totalPatrons++;
        }
    }

    void checkDad(spawnAI s)
    {
        dadWait++;
        if ( waveEnemyCountDad[waveCount] > dadCount)
        {
            s.genDad();
            dadWait -= dadRatio;
            dadCount++;
            dadText.text = "" + dadCount;
            totalPatrons++;
        }
    }

    public void patronWasKilled(int type)
    {
        score++;
        killedPatrons++;

        SteamUserStats.SetAchievement("First Kill");
        SteamUserStats.StoreStats();

        if (type == 0)
        {
            kidCount--;
            if (speedCurrent < speedMax)
            {
                
                //flood mode power ups
                if(GameModeControl.mode == 1)
                {
                    stopTimeCurrent++;
                    //image fill
                    stopTimeUI.fillAmount = (float)stopTimeCurrent / (float)stopTimeMax;

                    roomScareCurrent++;
                    //image fill
                    roomScareUI.fillAmount = (float)roomScareCurrent / (float)roomScareMax;

                    speedCurrent ++;
                    //image fill
                    speedUI.fillAmount = (float)speedCurrent / (float)speedMax;
                }
                //double speed power ups
                else if (GameModeControl.mode == 2)
                {
                    speedCurrent += 2;
                    //image fill
                    speedUI.fillAmount = (float)speedCurrent / (float)speedMax;
                }
                //normal mode power up
                else
                {
                    speedCurrent++;
                    //image fill
                    speedUI.fillAmount = (float)speedCurrent / (float)speedMax;
                }

            }



        }
        else if (type == 1)
        {
            momCount--;
            if (roomScareCurrent < roomScareMax)
            {
                //double speed mode
                if (GameModeControl.mode == 2)
                {
                    roomScareCurrent += 2;
                    //image fill
                    roomScareUI.fillAmount = (float)roomScareCurrent / (float)roomScareMax;
                }
                //normal mode
                else
                {
                    roomScareCurrent++;
                    //image fill
                    roomScareUI.fillAmount = (float)roomScareCurrent / (float)roomScareMax;
                }
            }
        }
        else if (type == 2)
        {
            dadCount--;
         
            if (stopTimeCurrent < stopTimeMax)
            {
                //double speed mode
                if (GameModeControl.mode == 2)
                {
                    stopTimeCurrent += 2;
                    //image fill
                    stopTimeUI.fillAmount = (float)stopTimeCurrent / (float)stopTimeMax;
                }
                //normal mode
                else
                {
                    stopTimeCurrent++;
                    //image fill
                    stopTimeUI.fillAmount = (float)stopTimeCurrent / (float)stopTimeMax;
                }
            }
        }
        kidText.text = "" + kidCount;
        momText.text = "" + momCount;
        dadText.text = "" + dadCount;
        //scoreText.text = "" + score;
    }
    void populateEnemy(int mode)
    {
        waveEnemyCountKid = new int[totalWaves];
        waveEnemyCountMom = new int[totalWaves];
        waveEnemyCountDad = new int[totalWaves];
        //set spawn for little girl flood mode
        if (mode == 1)
        {
            waveEnemyCountKid[0] = 6;
            waveEnemyCountKid[1] = 8;
            waveEnemyCountKid[2] = 10;
            waveEnemyCountKid[3] = 12;
            waveEnemyCountKid[4] = 13;
            waveEnemyCountKid[5] = 15;
            waveEnemyCountKid[6] = 17;
            waveEnemyCountKid[7] = 19;
            waveEnemyCountKid[8] = 20;
            waveEnemyCountKid[9] = 22;
            momRatio = 0;
            dadRatio = 0;
            kitchenStart = 0;
            windowStart = 0;
            bathroomStart = 0;
        }
        else
        {
            //set number of kids on each wave
            waveEnemyCountKid[0] = 1;
            waveEnemyCountKid[1] = 3;
            waveEnemyCountKid[2] = 3;
            waveEnemyCountKid[3] = 4;
            waveEnemyCountKid[4] = 5;
            waveEnemyCountKid[5] = 6;
            waveEnemyCountKid[6] = 4;
            waveEnemyCountKid[7] = 7;
            waveEnemyCountKid[8] = 7;
            waveEnemyCountKid[9] = 6;

            //set number of moms on each wave
            waveEnemyCountMom[0] = 0;
            waveEnemyCountMom[1] = 0;
            waveEnemyCountMom[2] = 1;
            waveEnemyCountMom[3] = 1;
            waveEnemyCountMom[4] = 1;
            waveEnemyCountMom[5] = 2;
            waveEnemyCountMom[6] = 3;
            waveEnemyCountMom[7] = 2;
            waveEnemyCountMom[8] = 3;
            waveEnemyCountMom[9] = 4;

            //set dad count on each wave
            waveEnemyCountDad[0] = 0;
            waveEnemyCountDad[1] = 0;
            waveEnemyCountDad[2] = 1;
            waveEnemyCountDad[3] = 2;
            waveEnemyCountDad[4] = 2;
            waveEnemyCountDad[5] = 2;
            waveEnemyCountDad[6] = 3;
            waveEnemyCountDad[7] = 3;
            waveEnemyCountDad[8] = 3;
            waveEnemyCountDad[9] = 4;

        }

       
    }
    void failFunction()
    {
        //spawnNextWave();
        GameObject.Find("UI").gameObject.SetActive(false);
        GO.Died();
    }

    public void winFunction()
    {
        won = true;
        SteamUserStats.SetAchievement("Beat The Game");
        SteamUserStats.StoreStats();
        GameObject ui = GameObject.Find("UI");
        if(ui != null)
            ui.gameObject.SetActive(false);
        winScreen.Won();

    }

    public void spawnNextWave()
    {
        
        waveCount++;
        enemySpawnCount = waveEnemyCountKid[waveCount];
        enemySpawnCount += waveEnemyCountMom[waveCount];
        enemySpawnCount += waveEnemyCountDad[waveCount];
        killedPatrons = 0;
        totalPatrons = 0;
        momCount = 0;
        dadCount = 0;
        kidCount = 0;
        timer = -wavePrepTime;
        waveTimer = -wavePrepTime;
        kitchenTimer = timer;
        mainTimer = timer;
        windowTimer = timer;
        bathroomTimer = timer;
        //waveText.text = "Wave: " + (waveCount + 1) + "";
        flameHealth.fillAmount = 1;
        currSpawn = 0;
       
        Wave.CrossFadeAlpha(1f, 0.2f, true);
        Ready.CrossFadeAlpha(1f, 0.2f, true);
        readyNumber.CrossFadeAlpha(1f, 0.2f, true);
        waveNumber.CrossFadeAlpha(1f, 0.2f, true);

        waveNumber.sprite = numDesat[waveCount];
        readyNumber.sprite = num[5];
    }

    public int getWaveCount()
    {
        return waveCount;
    }

    public int getTotalWaves()
    {
        return totalWaves;
    }

    public void stopTimer()
    {
        timing = false;
        stopTimeWindow = timer + stopTimeLength;
        stopTimeCurrent = 0;
        stopTimeUI.fillAmount = (float)stopTimeCurrent / (float)stopTimeMax;
    }
    public void startTimer()
    {
        timing = true;
    }

    public bool canFullRoomScare()
    {
        return roomScareCurrent >= roomScareMax;
    }
    public bool canStopTime()
    {
        return stopTimeCurrent >= stopTimeMax;
    }
    public bool canSpeedUp()
    {
        return speedCurrent >= speedMax;
    }

    //find all scare objects in the room ghoul kid is in and activate them
    public void fullRoomScare(string room)
    {
        GameObject[] roomScares = GameObject.FindGameObjectsWithTag(room);

        foreach(GameObject g in roomScares)
        {
            if (g.GetComponent<Scare>() != null)
            {
                //stupid fix for the piano and lamp being different than all other objects
                if (g.transform.parent.name == "lamp")
                {
                    g.transform.parent.GetComponent<lamp>().stupidLamp();
                }
                Debug.Log(g.gameObject.transform.parent.gameObject.name);
                g.GetComponent<Scare>().startScare();
                g.GetComponent<Scare>().startAnimation();
            }
            
        }
        roomScareCurrent = 0;
        roomScareUI.fillAmount = (float)roomScareCurrent / (float)roomScareMax;
    }
    //increase the players speed.
    public void speedPlayer(string room)
    {
        GameObject[] roomScares = GameObject.FindGameObjectsWithTag(room);

        foreach (GameObject g in roomScares)
        {
            if (g.GetComponent<Scare>() != null)
                g.GetComponent<Scare>().resetCooldown();
        }
        speedCurrent = 0;
        speedUI.fillAmount = (float)speedCurrent / (float)speedMax;
    }
    //put players speed back to normal
    public void slowPlayer()
    {
        Ghoul.speed = Ghoul.speed / speedUpPower;
        Ghoul.flySpeed = Ghoul.flySpeed / speedUpPower;
        mainCam.scrollSpeed = mainCam.scrollSpeed / speedUpPower;
        fast = false;
        
    }

    public void refillPowers()
    {
        speedCurrent = speedMax;
        roomScareCurrent = roomScareMax;
        stopTimeCurrent = stopTimeMax;
        speedUI.fillAmount = (float)speedCurrent / (float)speedMax;
        roomScareUI.fillAmount = (float)roomScareCurrent / (float)roomScareMax;
        stopTimeUI.fillAmount = (float)stopTimeCurrent / (float)stopTimeMax;
    }
}
