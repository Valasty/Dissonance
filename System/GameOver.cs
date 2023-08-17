using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOver : MonoBehaviour{

    GeneralFunctions general;
    public GameObject mainMenuCanvas;
    public Text[] gameOverTexts;

     IEnumerator Start(){

        general = FindObjectOfType<GeneralFunctions>();

        gameOverTexts[0].text = general.localization.gameOverText;
        gameOverTexts[1].text = general.localization.retryButtonText;
        gameOverTexts[2].text = general.localization.backToMenuButtonText;

        print("lero");

        yield return new WaitForSeconds(3);
        EventSystem.current.SetSelectedGameObject(mainMenuCanvas.transform.GetChild(1).gameObject);

        general.audioController.battleMusicSource.Stop();
        StartCoroutine(general.audioController.VolumeFade("Music", true, 1, 0.01f));

    }

    public void RetryButton(){

        mainMenuCanvas.SetActive(false);
        StartCoroutine(general.LoadBattle(general.sceneEnemies));
    }

    public void MainMenuButton(){

        mainMenuCanvas.SetActive(false);
        general.ResetCamera();
        Destroy(general.transform.parent.gameObject);
        SceneManager.LoadScene("Main Menu");
    }
}
