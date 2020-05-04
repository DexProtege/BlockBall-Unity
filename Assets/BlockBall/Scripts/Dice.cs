using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Dice : MonoBehaviour {
    //Random direction to move on
    Vector2 direction;
    //Speed multiplier for better control of difficulty
    public float speedMultiplier = 1.0f;
    //currentSpeed
    float maxCurrentSpeed = 1.0f;
    //Our Rigidbody
    Rigidbody2D myRigidbody2D;
    //Animator
    Animator myAnimator;
    //Base to block
    public Animator baseController;
    //Start and End UI
    public Animator UI;
    //Have we started
    bool started = false;
    //Visual Score
    public TextMesh currentScoreShower;
    //Visual Score Animator
    Animator currentScoreShowerAnimator;
    //Speed Incremental values
    public float[] speedIncrementals;
    //Background color
    public Color[] BGColors;
    //End Game Background color
    public Color endGameBGColor = Color.white;
    //Current Incremental Value
    int currentIncremental = 0;
    //Counter to cuntrol intervals
    float counter = 0.0f;
    //Score
    int score = 0;
    //Area to play
    public float gamePlayArea = 10.0f;
    //Is the game over
    bool over = false;
    //End UI Text
    public Text endScoreText;
    //End Ui Best Text
    public Text endBestScoreText;
    //All audios stuff
    public AudioClip startSound;
    public AudioClip touchWallSound;
    public AudioClip touchBaseSound;
    public AudioClip endGameSound;
    AudioSource myAudioPlayer;
    // Use this for initialization
    void Start () {
        //Assigning all the variables
        myRigidbody2D = transform.GetComponent<Rigidbody2D>();
        myAnimator = transform.GetComponent<Animator>();
        myAudioPlayer = transform.GetComponent<AudioSource>();

        if (speedIncrementals.Length > 0)
        {
            maxCurrentSpeed = speedIncrementals[0] * speedMultiplier;
        }
        else {
            maxCurrentSpeed = 5.0f * speedMultiplier ;
        }

        if (!myRigidbody2D)
        {
            Debug.LogError("No Rigidbody Found");
        }

        if (!myAnimator) {
            Debug.LogError("No Animator Found");
        }

        if (!UI) {
            Debug.LogError("No UI Animator Found");
        }

        if (!myAudioPlayer) {
            Debug.LogError("No Audio Source Found");
        }

        if (!endGameSound || !startSound || !touchBaseSound || !touchWallSound) {
            Debug.LogWarning("Please Assign all the audio clips");
        }

        if (currentScoreShower)
        {
            currentScoreShowerAnimator = currentScoreShower.transform.root.GetComponent<Animator>();
        }
        //Random Color for Start
        if (BGColors.Length > 0)
            Camera.main.backgroundColor = BGColors[Random.Range(0, BGColors.Length)];

    }

    //Random direction to move
    void AddRandomForce() {
        direction = new Vector2(Random.Range(-1.0f, 1.0f), 1.0f);
        myRigidbody2D.velocity = (direction.normalized * maxCurrentSpeed);
    }

    //Did we hit a wall
    void OnCollisionEnter2D() {
        if (myAnimator)
        {
            myAnimator.SetTrigger("Splash");
        }

        if (myAudioPlayer && touchWallSound )
            myAudioPlayer.PlayOneShot(touchWallSound,.35f);
    }

    //Did we hit Base Controller 
    void OnTriggerEnter2D() {
        score++;

        if (currentScoreShower)
        {
            currentScoreShower.text = score.ToString();
        }
        else {
            Debug.LogWarning("No TextMesh assigned");
        }

        SetNewIncremental();

        AddRandomForce();

        if (myAnimator)
        {
            myAnimator.SetTrigger("Splash");
        }

        if (myAudioPlayer && touchBaseSound)
        {
            myAudioPlayer.pitch = maxCurrentSpeed / 8.0f;
            myAudioPlayer.PlayOneShot(touchBaseSound);

        }

    }

    //Increse score and play blink animation
    void SetNewIncremental() {
        if (currentIncremental < speedIncrementals.Length-1)
        {
            currentIncremental++;
            maxCurrentSpeed = speedIncrementals[currentIncremental] * speedMultiplier;
            if (currentScoreShower) {

                currentScoreShower.transform.localScale = Vector3.one * (0.25f/((score + 10) / 10));
                
                if (currentScoreShowerAnimator)
                {
                    currentScoreShowerAnimator.SetTrigger("Splash");
                }
            }


        }
    }

    // Update is called once per frame
    void Update () {
        //This will start the game
        if (Input.GetButtonDown("Fire1") && UI && !started) {
            started = true;
            UI.enabled = true;
            StartCoroutine(MakeItVisible());
            AddRandomForce();

            if (myAudioPlayer && startSound)
                myAudioPlayer.PlayOneShot(startSound);
        }
        //Enable blocker
        if (Input.GetButtonDown("Fire1") && started && baseController && counter <= 0.0f && !over)
        {
            baseController.SetTrigger("Blink");
            counter = 1.0f;
        }
        else if (counter > 0.0f) {
            counter -= Time.deltaTime * maxCurrentSpeed * 0.25f;
        }
        //Are out of game play area 
        if (started && Vector3.Distance(transform.position, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f) ) >= gamePlayArea && !over) {
            over = true;
            OverIt();
        }

	}
    //Called when you loose
    void OverIt() {
        Debug.Log("Over");
        StopCoroutine(MakeItVisible());
        StartCoroutine(MakeItInvisible());
        StartCoroutine(MakeItWhite());

        if (PlayerPrefs.GetInt("Best", 0) < score) {
            PlayerPrefs.SetInt("Best", score);
        }

        if (endScoreText) {
            endScoreText.text = "SCORE : " + score.ToString();
        }
        if (endBestScoreText) {
            endBestScoreText.text = "BEST : " + PlayerPrefs.GetInt("Best", 0);
        }

        if (UI) {
            UI.SetTrigger("End");
        }

        if (myAudioPlayer && endGameSound)
            myAudioPlayer.PlayOneShot(endGameSound);
    }

    //Enable visual score graphic
    IEnumerator MakeItVisible() {
        if (!currentScoreShower) {
            yield return null;
        }

        while (currentScoreShower.color.a <= 0.75f) {
            currentScoreShower.color = Color.Lerp(currentScoreShower.color, Color.white, Time.deltaTime * 5.0f);
            yield return null;
        }

        yield return null;
    }

    //Disable visual score graphic 
    IEnumerator MakeItInvisible() {
        if (!currentScoreShower)
        {
            yield return null;
        }

        while (currentScoreShower.color.a >= 0.1f)
        {
            currentScoreShower.color = Color.Lerp(currentScoreShower.color, new Color(255,255,255,0.0f), Time.deltaTime * 5.0f);
            yield return null;
        }
        currentScoreShower.color = new Color(255, 255, 255, 0.0f);
        yield return null;
    }

    //Make background black and white
    IEnumerator MakeItWhite ()
    {
        while (Camera.main.backgroundColor != endGameBGColor)
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, endGameBGColor, Time.deltaTime * 5.0f);
            yield return null;
        }

        yield return null;
    }

    //Shows gameplay area in editor
    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f), gamePlayArea);
    }

    //Restart the game
    public void Restart() {
        Initiate.Fade(Application.loadedLevelName, Color.white, 3.0f);
    }
}
