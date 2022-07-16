using UnityEngine;
using UnityEngine.UI;

public class SceneFadeTransition : MonoBehaviour , ISceneTransitionHandler
{
    public Image BlackImage;
    public Canvas Canvas;

    public void SetPercent(float percent)
    {
        if(percent==0)
        {
            if(Canvas.enabled==true)
            {                
                Canvas.enabled=false;
                ChangeImageAlphaValue(ref BlackImage,percent);
            }
            return;
        }
        else
        {
            if(Canvas.enabled==false)
                Canvas.enabled=true;
        }
        
        ChangeImageAlphaValue(ref BlackImage,percent);
    }


    private void ChangeImageAlphaValue(ref Image image,float percentAlpha)
    {
        if(image==null)
            return;
        if (percentAlpha < 0)
        {
            percentAlpha = 0;
        }
        Color StaminaColor = image.color;
        StaminaColor.a = percentAlpha;
        image.color = StaminaColor;
    }
}
