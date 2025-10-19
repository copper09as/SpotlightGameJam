using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, 0); 
    private TextMeshProUGUI tmp;

    void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    public void SetText(string text)
    {
        if (tmp != null)
            tmp.text = text;
    }
}
