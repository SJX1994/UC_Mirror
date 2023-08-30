using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuppetMouseState : MonoBehaviour
{
    public int puppetId; // 配对ID
    PuppetUnit puppetUnit;
    public UnityAction<int> OnMouseEnterPuppet;
    public UnityAction<int> OnMouseExitPuppet;
    public UnityAction<int,GameObject> OnMouseUpPuppet;
    
    // Start is called before the first frame update
    void Start()
    {
        puppetUnit = transform.GetComponent<PuppetUnit>();
    }
    private void OnMouseUp()
    {
        if(OnMouseUpPuppet!=null)
        {
            GameObject whatHit = puppetUnit.puppetLine.transform.gameObject;
            OnMouseUpPuppet(puppetId,whatHit);
        }
    }
    private void OnMouseExit()
    {
        OnMouseExitPuppet?.Invoke(puppetId);
    }
    private void OnMouseEnter()
    {
        OnMouseEnterPuppet?.Invoke(puppetId);
    }
}
