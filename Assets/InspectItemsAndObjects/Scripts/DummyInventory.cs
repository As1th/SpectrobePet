using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyInventory : MonoBehaviour {

    public event EventHandler<ItemSO> OnItemSelected;



    [SerializeField] private List<ItemSO> itemList;

    private Transform itemTemplate;
    private Dictionary<ItemSO, Transform> itemSOTransformDic;

    private void Awake() {
        itemTemplate = transform.Find("InventoryItemTemplate");
        itemTemplate.gameObject.SetActive(false);

        itemSOTransformDic = new Dictionary<ItemSO, Transform>();

        foreach (ItemSO itemSO in itemList) {
            Transform itemTransform = Instantiate(itemTemplate, transform);
            itemTransform.gameObject.SetActive(true);
            itemTransform.Find("Image").GetComponent<Image>().sprite = itemSO.sprite;

            itemSOTransformDic[itemSO] = itemTransform;

            itemTransform.GetComponent<Button>().onClick.AddListener(() => {
                SelectItem(itemSO);
            });
        }
    }

    private void SelectItem(ItemSO selectedItemSO) {
        foreach (ItemSO itemSO in itemSOTransformDic.Keys) {
            itemSOTransformDic[itemSO].Find("Selected").gameObject.SetActive(false);
        }

        itemSOTransformDic[selectedItemSO].Find("Selected").gameObject.SetActive(true);

        OnItemSelected?.Invoke(this, selectedItemSO);
    }

}
