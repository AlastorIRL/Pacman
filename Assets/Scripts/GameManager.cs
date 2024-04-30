using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;

    public Pacman pacman;
    public Transform pellets;


    //intro audio
    public AudioSource source;
    public AudioClip intro;
    //end


    //pacman audio
    public AudioClip[] waka;
    private int wakaOrder = 1;
    //public AudioSource wakaSource;
    //end


    //death sfx
    public AudioClip death;
    //source usa msm da intro

    //UI vidas - vou fazer com icones do pacman na lateral q sumam de acordo com a quantidade de vidas restante
    public Transform life3;
    public Transform life2;
    public Transform life1;
    //end

    //score ui
    public Text scoreText;
    

    public Transform getready;


    public int ghostMultiplier { get; private set; } = 1;

    public int score { get; private set; }
    public int lives { get; private set; }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (this.lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        getready.gameObject.SetActive(true);
        Time.timeScale = 0f;
        source.PlayOneShot(intro);
        
        SetScore(0);
        SetLives(3);
        lifeInterface();

        StartCoroutine(WaitForIntroToEnd(intro.length));
    }

    

    private IEnumerator WaitForIntroToEnd(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        NewRound();
    }  

    private void NewRound()
    {
        getready.gameObject.SetActive(false);
        foreach(Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        ResetGhostMultiplier();

        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }

        this.pacman.ResetState();
    }

    private void GameOver()
    {
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);       
    }

    private void SetScore(int score)
    {
        this.score = score;
        this.scoreText.text = score.ToString();
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);
        this.ghostMultiplier++;
        this.scoreText.text = score.ToString();
    }

    public void PacmanEaten()
    {
        source.PlayOneShot(death);
        this.pacman.gameObject.SetActive(false);

        SetLives(this.lives -1);
        lifeInterface();

        if(this.lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f);
        } else 
        {
            GameOver();
        }
    }

    private void lifeInterface()
    {
        if(this.lives == 3)
        {
            life3.gameObject.SetActive(true);
            life2.gameObject.SetActive(true);
            life1.gameObject.SetActive(true);     
        } else if (this.lives == 2)
        {
            life3.gameObject.SetActive(false);
            life2.gameObject.SetActive(true);
            life1.gameObject.SetActive(true);
        } else if (this.lives == 1)
        {
            life3.gameObject.SetActive(false);
            life2.gameObject.SetActive(false);
            life1.gameObject.SetActive(true);            
        } else
        {
            life3.gameObject.SetActive(false);
            life2.gameObject.SetActive(false);
            life1.gameObject.SetActive(false);
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        WakaSound();
        source.PlayOneShot(waka[wakaOrder]); 

        pellet.gameObject.SetActive(false);
        SetScore(this.score + pellet.points);
        this.scoreText.text = score.ToString();

        if (!HasRemainingPellets())
        {
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3.0f);
        }
    }

    public void WakaSound()
    {
        if (wakaOrder == 1)
        {
            wakaOrder = 0;
        } else if (wakaOrder == 0)
        {
            wakaOrder = 1;
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i <this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
        this.scoreText.text = score.ToString();
    }

    private bool HasRemainingPellets()
    {
        foreach(Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }        

        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }
}
