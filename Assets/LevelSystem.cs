using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{
    public List<EnemyAction> enemyActions = new List<EnemyAction>();
    
    // Update is called once per frame
    void Update()
    {

        if (enemyActions.Count <= 0)
        {
            bool isPass = true;
            
            enemyActions.ForEach(delegate(EnemyAction action)
            {
                if (action != null)
                    isPass = false;

            });

            if (isPass)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                Destroy(this);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
