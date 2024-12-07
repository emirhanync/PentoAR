using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour
{
    public GameObject MarketPlace;
    public void openMarket() 
    {
        MarketPlace.SetActive(true);
    }
    public void closeMarket() { MarketPlace.SetActive(false); }

    public void OpenLink()
    {
        Application.OpenURL("www.google.com");
    }
}
