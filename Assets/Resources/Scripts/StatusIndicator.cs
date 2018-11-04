using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour {

    [SerializeField]
    private RectTransform healthBarRect;

    [SerializeField]
    private Text healthText;

    private GradientColorKey[] gck;
    private GradientAlphaKey[] gak;
    private Gradient g;
    public Color startColor;
    public Color midColor;
    public Color endColor;


    private void Start()
    {
        g = new Gradient();
        healthBarRect.gameObject.GetComponent<Image>().color = startColor;

        if (healthBarRect == null)
        {
            Debug.Log("STATUS INDICATOR : No health bar object reference");
        }
        if (healthText == null)
        {
            Debug.Log("STATUS INDICATOR : No health text object reference");
        }
        gck = new GradientColorKey[3];
        gck[0].color = startColor;
        gck[0].time = 0.0F;
        gck[1].color = midColor;
        gck[1].time = 0.5F;
        gck[2].color = endColor;
        gck[2].time = 1F;
        gak = new GradientAlphaKey[2];
        gak[0].alpha = 1.0F;
        gak[0].time = 0.0F;
        gak[1].alpha = 1.0F;
        gak[1].time = 1.0F;
        g.SetKeys(gck, gak);

    }

    public void SetHealth(int _cur, int _max)
    {
        float _value =  ((float)_cur / (float)_max);

        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);
        healthText.text = _cur + "/" + _max + " HP";


        if(g != null)
        healthBarRect.gameObject.GetComponent<Image>().color = g.Evaluate(1 - _value);

    }
}


