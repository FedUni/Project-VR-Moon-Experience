using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolfWarning : MonoBehaviour
{
if (!Physics.Raycast(myray, out myhit, 1000.0f))
  {
      if(myhit.gameObject.tag == "Object")
      {
          text.setActive(true);
          text.GetComponent<Text>().text = "text needed";
      }
      else
      {
          text.setActive(false);
          text.GetComponent<Text>().text = "";
      }
  }