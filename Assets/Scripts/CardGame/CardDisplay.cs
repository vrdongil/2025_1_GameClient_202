using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public int cardIndex;

    public MeshRenderer cardRenderer;
    public TextMeshPro nameText;
    public TextMeshPro costText;
    public TextMeshPro attackText;
    public TextMeshPro descriptionText;


    public bool isDragging = false;
    private Vector3 originalPosition;

    public LayerMask enemyLater;
    public LayerMask playerLayer;

    private CardManager cardManager;
    // Start is called before the first frame update
    void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        enemyLater = LayerMask.GetMask("Enemy");

        cardManager = FindObjectOfType<CardManager>();

        SetupCard(cardData);
    }

   public void SetupCard(CardData data)
    {
        cardData = data;

        if (nameText != null) nameText.text = data.cardName;
        if (costText != null) costText.text = data.manaCost.ToString();
        if (attackText != null) attackText.text = data.effectAmount.ToString();
        if (descriptionText != null) descriptionText.text = data.description;

        if(cardRenderer != null && data.artwork != null)
        {
            Material cardMaterial = cardRenderer.material;
            cardMaterial.mainTexture = data.artwork.texture;
        }
    }

    private void OnMouseDown()
    {
        originalPosition = transform.position;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if(isDragging)
        {

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        }
    }

   



    private void OnMouseUp()
    {

        CharacterStats playerStats = FindObjectOfType<CharacterStats>();
        if(playerStats == null || playerStats.currentMana < cardData.manaCost)
        {
            Debug.Log($"������ �����մϴ�! (�ʿ� : {cardData.manaCost}, ���� : {playerStats?.currentMana ?? 0}");
            transform.position = originalPosition;
            return;
        }
        
        isDragging = false;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool cardUsed = false;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLater))
        {
            CharacterStats enemyStats = hit.collider.GetComponent<CharacterStats>();

            if (enemyStats != null)
            {
                if (cardData.cardtype == CardData.CardType.Attack)
                {
                    enemyStats.TakeDamage(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} ī��� ������ {cardData.effectAmount} �������� �������ϴ�.");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("�� ī��� ������ ��� �� �� �����ϴ�.");
                }
            }
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayer))
        {
           // CharacterStats playerStats = hit.collider.GetComponent<CharacterStats>();

            if (playerStats != null)
            {
                if (cardData.cardtype == CardData.CardType.Heal)
                {
                    playerStats.Heal(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} ī��� �÷��̾��� ü���� {cardData.effectAmount} ȸ���߽��ϴ�!");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("�� ī��� �÷��̾�� ��� �� �� �����ϴ�.");
                }
            }
        }
        else if (cardManager != null)
        {

            float distToDiscard = Vector3.Distance(transform.position, cardManager.discardPosition.position);
            if (distToDiscard < 2.0f)
            {
                cardManager.DiscardCard(cardIndex);
                return;
            }
        }
        
        
        if (!cardUsed)
        {
            transform.position = originalPosition;
            cardManager.ArrangeHand();
        }
        else
        {
            if (cardManager != null)
                cardManager.DiscardCard(cardIndex);

            playerStats.UseMana(cardData.manaCost);
            Debug.Log($"������ {cardData.manaCost} ��� �߽��ϴ�. (���� ���� : {playerStats.currentMana})");
        }
        
       
    }

    private void ProcessAdditionalEffectsAndDiscard()
    {

        CardData cardDatacopy = cardData;
        int cardIndexCopy = cardIndex;

        foreach(var effect in cardDatacopy.additionalEffects)
        {
            switch (effect.effectType)
            {
                case CardData.AdditionalEffectType.DrawCard:
                    
                    for (int i = 0; i < effect.effectAmount; i++)
                    {
                        if(cardManager != null)
                        {
                            cardManager.DrawCard();
                        }
                    }
                    Debug.Log($"{effect.effectAmount} ���� ī�带 ��ο� �߽��ϴ�.");
                    break;
            }
        }
    }
}
