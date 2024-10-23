using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI TapToPlay;
   [SerializeField] private GameObject Hand_Ico;
   [SerializeField] private GameObject Tap_line;
  [SerializeField] private RectTransform tap_handRectTransform;

   private bool gameStarted = false;

   void Start()
   {
       // Scale the TapToPlay text
       LeanTween.scale(TapToPlay.gameObject, new Vector3(0.2f, 0.6f, 0.6f), 1f).setLoopPingPong().setEaseInOutQuad();

       // Move the Hand_Ico
       LeanTween.move(Hand_Ico.GetComponent<RectTransform>(), new Vector2(-17f, -27f), 1f).setLoopPingPong().setEaseInOutQuad();
   }

   void Update()
   {
       if (!gameStarted && Input.GetMouseButtonDown(0))
       {
           Vector2 mousePos = Input.mousePosition;

           if (RectTransformUtility.RectangleContainsScreenPoint(tap_handRectTransform, mousePos))
           {
               StartGame();
           }
       }
   }

   void StartGame()
   {
       gameStarted = true;

       // Stop animations
       LeanTween.cancel(TapToPlay.gameObject);
       LeanTween.cancel(Hand_Ico.GetComponent<RectTransform>());

      // TapToPlay.gameObject.SetActive(false);
       Hand_Ico.SetActive(false);
       Tap_line.SetActive(false);

       Debug.Log("Game Started!");
   }
}
