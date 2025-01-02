using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IsCard : MonoBehaviour
{
    public Card card;

    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] TextMeshProUGUI Tier;
    [SerializeField] TextMeshProUGUI Cost;
    [SerializeField] TextMeshProUGUI Description;

    public string type;

    // Start is called before the first frame update
    void Start()
    {
        if (type == "Resource") gameObject.AddComponent<HasResourceEffect>();
    }

    private void OnValidate()
    {
        if (card != null) UpdateFields();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateFields()
    {
        Title.text = card.title;
        Tier.text = card.tier.ToString();
        Cost.text = card.cost.ToString();
        Description.text = card.description;
    }

    public virtual void Play()
    {
        Debug.Log("Played " + card.title);
        GetComponent<HasEffect>().Activate();
    }
}
