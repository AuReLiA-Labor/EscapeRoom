using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Entrance : MonoBehaviour
{
    [SerializeField] private GameObject _entrance;
    [SerializeField] private GameObject _learningPoint;
    [SerializeField] private Button _buttonEnter;
    // Start is called before the first frame update
    void Start()
    {
        _buttonEnter.onClick.AddListener(OnClickEnter);
    }

    private void OnClickEnter()
    {
        _entrance.SetActive(false);
        _learningPoint.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
