using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageSwitcher : MonoBehaviour
{
    #region PUBLIC MEMBERS

    public GameObject to = null;
    public GameObject from = null;
    #endregion

    #region PRIVATE MEMBERS
    private GameObject m_from;
    #endregion

    #region MONOBEHAVIOUR METHODS
    void Start()
    {
        if (this.from != null) this.m_from = this.from;
        else this.m_from = this.transform.parent.gameObject;
    }
    #endregion

    #region PUBLIC METHODS
    public void SwitchPage()
    {
        this.m_from.SetActive(false);
        this.to.SetActive(true);
    }
    #endregion
}
