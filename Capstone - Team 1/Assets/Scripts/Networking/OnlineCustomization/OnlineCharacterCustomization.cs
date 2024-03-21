using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class OnlineCharacterCustomization : MonoBehaviour
{
    [SerializeField] private Penguin p1;
    [SerializeField] private Penguin p2;
    [SerializeField] private Hat[] hatArray;
    [SerializeField] private PhotonView photonView;
    public Hat p1Hat
    {
        get
        {
            Hat hat = getHat(p1);
            int index = getIndex(hat);
            return hatArray[index];
        }
    }
    public Hat p2Hat
    {
        get
        {
            Hat hat = getHat(p2);
            int index = getIndex(hat);
            return hatArray[index];
        }
    }

    public void seeP1()
    {
        p1.gameObject.SetActive(true);
    }
    public void seeP2()
    {
        p2.gameObject.SetActive(true);
    }


    //To be used to compare cloned elements to the prefab
    private string CLONE = "(Clone)";

    private void Awake()
    {
        //Assign different random hats to each penguin
        if(OnlineCharacterCustomizationUI.XHAT != null && OnlineCharacterCustomizationUI.OHAT != null)
        {
            p1.setHat(OnlineCharacterCustomizationUI.XHAT);
            p2.setHat(OnlineCharacterCustomizationUI.OHAT);
        }
        else
        {
            p1.setHat(hatArray[0]);
            p2.setHat(hatArray[1]);
        }

    }

    public void setRandomHat(bool isP1)
    {
        //generates a random hat not used by other player
        System.Random random = new System.Random();
        int randIndex = random.Next(0, hatArray.Length);
        //gets the index of the other players hat
        int otherIndex = getIndex(!isP1);
        //get the current hat so random hat is new
        int curIndex = getIndex(isP1);
        do
        {
            randIndex = random.Next(0, hatArray.Length);
        } while (randIndex == otherIndex || randIndex == curIndex);

        photonView.RPC("onlineSetRandomHat", RpcTarget.All, randIndex, isP1);
    }

    [PunRPC]
    public void onlineSetRandomHat(int randIndex, bool isP1)
    {
        if (isP1)
        {
            Destroy(getHat(p1).gameObject);
            p1.setHat(hatArray[randIndex]);

        }
        else
        {
            Destroy(getHat(p2).gameObject);
            p2.setHat(hatArray[randIndex]);
        }
    }


    private void getObjectNames(Hat hat, ref string arrayHat, ref string curHat, Penguin player)
    {
        //Puts name of hat object into each string and removes "(Clone)" from cloned hat for easy comparison
        //Used in get index by penguin
        arrayHat = hat.gameObject.GetComponent<MeshFilter>().name;
        curHat = getHat(player).gameObject.GetComponent<MeshFilter>().name;
        curHat = curHat.Replace(CLONE, "");
    }

    private void getObjectNames(Hat hat, Hat checkHat, ref string arrayHat, ref string curHat)
    {
        //Puts name of hat object into each string and removes "(Clone)" from cloned hat for easy comparison
        //Used in get index by hat
        arrayHat = hat.gameObject.GetComponent<MeshFilter>().name;
        curHat = checkHat.gameObject.GetComponent<MeshFilter>().name;
        curHat = curHat.Replace(CLONE, "");
    }

    private int getIndex(bool isP1)
    {
        //Gets the index of a penguins hat
        int index = 0;
        foreach (Hat hat in hatArray)
        {
            //initilaize strings to store object names
            string arrayHat = "", curHat = "";
            getObjectNames(hat, ref arrayHat, ref curHat, isP1 ? p1 : p2);
            if (arrayHat == curHat)
            {
                return index;
            }
            else
            {
                ++index;
            }
        }
        return -1;
    }

    private int getIndex(Hat inputHat)
    {
        //gets index of a hat
        int index = 0;
        foreach (Hat hat in hatArray)
        {
            //initialize strings to store object names
            string arrayHat = "", curHat = "";
            getObjectNames(hat, inputHat, ref arrayHat, ref curHat);
            if (arrayHat == curHat)
            {
                return index;
            }
            else
            {
                ++index;
            }
        }
        return -1;

    }
    public void ForwardHat(bool isP1)
    {
        //moves forward through array of hats
        int index = getIndex(isP1);
        Hat hat;
        //checking to see if hat is sused by other player and skipped if so
        //Uses mod operator to loop around array as necessary
        if (((index + 1) % hatArray.Length) == getIndex(!isP1))
        {
            hat = hatArray[(index + 2) % hatArray.Length];

        }
        else
        {
            hat = hatArray[(index + 1) % hatArray.Length];
        }
        if (isP1)
            //destroys the old hat on the penguin and sets new hat
        {
            Destroy(getHat(p1).gameObject);
            p1.setHat(hat);
        }
        else
        {
            Destroy(getHat(p2).gameObject);
            p2.setHat(hat);
        }
    }

    public void BackwardHat(bool isP1)
    {
        //moves backwards through array
        int index = getIndex(isP1);
        if(index <= 0)
        {
            //goes to last element in array
            index = hatArray.Length + index;
        }
        Hat hat;
        //checks to make sure new hat is not used by other team
        //extra if added to handle cases where the other team has the first hat assigned so we have to go from index 1 to the last index
        if(((index - 1) % hatArray.Length) == getIndex(!isP1))
        {
            int newIndex = -1;
            if((index - 2) < 0) {
                newIndex = hatArray.Length + newIndex;
            }
            else
            {
                newIndex = index - 2;
            }
            hat = hatArray[(newIndex) % hatArray.Length];

        }
        else
        {
            hat = hatArray[(index - 1) % hatArray.Length];
        }
        if (isP1)
        //destroys the old hat on the penguin and sets new hat
        {
            Destroy(getHat(p1).gameObject);
            p1.setHat(hat);
        }
        else
        {
            Destroy(getHat(p2).gameObject);
            p2.setHat(hat);
        }
    }

    public void switchSides()
    {
        //grabs the hats assigned to each penguin, gets their index,
        //destroys the hats off the penguins, assigns the other team's hat to penguin based on index
        Hat p1Hat = getHat(p1);
        int indexP1 = getIndex(p1Hat);
        Destroy(p1Hat.gameObject);

        Hat p2Hat = getHat(p2);
        int indexP2 = getIndex(p2Hat);
        Destroy(p2Hat.gameObject);

        p1.setHat(hatArray[indexP2]);
        p2.setHat(hatArray[indexP1]);
    }

    private Hat getHat(Penguin player)
    {
        //returns the hat object off the penguin
        return player.head.GetComponentInChildren<Hat>();
    }
}
