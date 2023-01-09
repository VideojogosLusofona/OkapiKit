using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Variable[] variablesToResetOnStart;
    [SerializeField] private Variable[] variablesToResetOnSpawn;
    [SerializeField] private GameObject[] prefabsToSpawn;

    Coroutine spawnCR = null;

    void Start()
    {
        foreach (var variable in variablesToResetOnStart)
        {
            variable.ResetValue();
        }

        Respawn(0.0f);
    }

    public void Respawn(float time)
    {
        if (time > 0.0f)
        {
            if (spawnCR != null) return;
            spawnCR = StartCoroutine(RespawnCR(time));
            return;
        }

        foreach (var variable in variablesToResetOnSpawn)
        {
            variable.ResetValue();
        }

        foreach (var obj in prefabsToSpawn)
        {
            Instantiate(obj);
        }
    }

    IEnumerator RespawnCR(float time)
    {
        yield return new WaitForSeconds(time);

        Respawn(0.0f);

        spawnCR = null;
    }
}
