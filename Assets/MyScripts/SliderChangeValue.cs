using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SliderChangeValue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI number;
    //[SerializeField] private Slider slider;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setValue()
    {
        Slider myslider = GetComponent<Slider>();  
        number.text = myslider.value.ToString();

    }
}
