using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassingValue : MonoBehaviour
{
    public static int score
    {
        get => _score;
        set
        {
            _score = value;
        }
    }
    private static int _score;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


}
