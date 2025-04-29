using UnityEngine;

namespace Game.Tutorial
{
    public class PopUpGUI : MonoBehaviour
    {
        public GameObject popUpObj;
        public void OnClickButtonPopUp()
        {
            if (popUpObj != null)
                popUpObj.SetActive(!popUpObj.activeSelf);
        }
    }

}