using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManagerTest : MonoBehaviour
{
   //canvas
   [SerializeField] private GameObject _welcomeCanvas;
   [SerializeField] private GameObject _howToScanCanvas;
   [SerializeField] private GameObject _scanningCanvas;
   [SerializeField] private GameObject _inGameCanvas;
   //panels
   [SerializeField] private GameObject _learnigPointWelcomePanel;
   [SerializeField] private GameObject _failPanel;
   [SerializeField] private GameObject _successPanel;
   [SerializeField] private GameObject _timeScorePanel;
   
   //buttons
   [SerializeField] private Button _startButton;
   [SerializeField] private Button _scanButton;
   [SerializeField] private Button _closeLearningWelcome;
   [SerializeField] private Button _closeFailPanel;
   [SerializeField] private Button _closeSuccessPanel;
   [SerializeField] private Button _restartButton;
   [SerializeField] private Button _playButton;
   private void Start()
   {
      _startButton.onClick.AddListener(OnStartButtonClicked);
      _scanButton.onClick.AddListener(OnScanButtonClicked);
      _closeLearningWelcome.onClick.AddListener(OnCloseLearningWelcomeClicked);
      _closeFailPanel.onClick.AddListener(OnCloseFailPanelClicked);
      _closeSuccessPanel.onClick.AddListener(OnCloseSuccessPanelClicked);
      _restartButton.onClick.AddListener(OnRestartButtonClicked);
      _playButton.onClick.AddListener(OnPlayButtonClicked);
      
      
   }



   private void OnStartButtonClicked()
   {
      _welcomeCanvas.SetActive(false);
      _howToScanCanvas.SetActive(true);
   }
   private void OnScanButtonClicked()
   {
      _howToScanCanvas.SetActive(false);
      _scanningCanvas.SetActive(true);
      _inGameCanvas.SetActive(true);
   }
   private void OnPlayButtonClicked()
   {
      _learnigPointWelcomePanel.SetActive(false);
      _timeScorePanel.SetActive(true);
   }
   private void OnCloseLearningWelcomeClicked()
   {
      _learnigPointWelcomePanel.SetActive(false);
      _welcomeCanvas.SetActive(true);
   }
   private void OnRestartButtonClicked()
   {
      _successPanel.SetActive(false);
   }

   private void OnCloseSuccessPanelClicked()
   {
      _successPanel.SetActive(false);
   }

   private void OnCloseFailPanelClicked()
   {
      _failPanel.SetActive(false);
   }
  

   

  
}
