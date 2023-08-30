using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;
using UnityEngine.UI;

public class UIZoomImage : MonoBehaviour, IScrollHandler
{
    private Vector3 initialScale;
    public Button RotateBtn;
    [SerializeField]
    private float zoomSpeed = 0.1f;
    [SerializeField]
    private float maxZoom = 10f;
    public RectTransform rectTransform;
    int clikcCount = 0;
    private void Awake()
    {
        initialScale = transform.localScale;
        if(!rectTransform)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        clikcCount = 0;
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        var delta = Vector3.one * (eventData.scrollDelta.y * zoomSpeed);
        var desiredScale = transform.localScale + delta;

        desiredScale = ClampDesiredScale(desiredScale);

        transform.localScale = desiredScale;
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale)
    {
        desiredScale = Vector3.Max(initialScale, desiredScale);
        desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale);
        return desiredScale;
    }
    Tween Rot; 
    public void Roate()
    {
        clikcCount ++ ;
        if(Rot!=null)Rot.Kill();
        
        // Rotate 到另一边
        if(clikcCount==2)
        {
            // 将RectTransform旋转180度
            Rot = rectTransform.DORotate(new Vector3(0f, 0f, 360f), 1.5f).SetEase(Ease.OutCirc).OnComplete(() =>{
                rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            });
            //RotateBtn.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
            //RotateBtn.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            clikcCount = 0;
        }
        if(clikcCount==1)
        {
            // 将RectTransform旋转180度
            Rot = rectTransform.DORotate(new Vector3(0f, 0f, 180f), 1.5f).SetEase(Ease.OutCirc).OnComplete(() =>{});
            //RotateBtn.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
            //RotateBtn.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, -0.5f);
        }
        
    }
}