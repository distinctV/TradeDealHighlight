using System;
using Engine.Impl.UI.Menu.Protagonist.Inventory;
using Engine.Source.Components;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using InputServices;
using UnityEngine;

public class StorableUITrade : Engine.Impl.UI.Menu.Protagonist.Inventory.StorableUI
{
  [SerializeField]
  private GameObject _selectedImage;
  private int selectedCount;
  private void Start() => _selectedImage.SetActive(false);

  protected override void Update()
  {
    if (internalStorable.Max > 1)
    {
      textCount.text = selectedCount == 0 ? internalStorable.Count.ToString() : selectedCount + "/" + internalStorable.Count;
      if (textCount.gameObject != null)
        textCount.gameObject.SetActive(true);
    }
    else
    {
      textCount.text = null;
      if (textCount.gameObject != null)
        textCount.gameObject.SetActive(false);
    }
    selectedImage.gameObject.SetActive(isSelected);
    Color color = enabledBackgroundColor;
    if (!isEnabled)
    {
      color = disabledBackgroundColor;
    }
    else
    {
      int currentDealStatus = 0;
      if (internalStorable != null && internalStorable.Storage != null && internalStorable.Storage.Owner != null)
      {
         StorableComponent storable = (StorableComponent)internalStorable;
         bool playerBuying = storable.Storage.Owner.GetComponent<Engine.Source.Components.PlayerControllerComponent>() == null;
         float basePrice = PLVirtualMachine.Common.EngineAPI.VMECS.VMGlobalMarketManager.Instance.GetCurrentItemTradeGlobalPrice(storable.Owner.Name, !playerBuying);
         float actualPrice = playerBuying ? storable.Invoice.SellPrice : storable.Invoice.BuyPrice;
         
         if (basePrice > 0.0001f && Mathf.Abs(actualPrice - basePrice) > 0.01f)
         {
            if (playerBuying)
                currentDealStatus = actualPrice < basePrice ? 1 : -1;
            else
                currentDealStatus = actualPrice > basePrice ? 1 : -1;
         }
      }

      if (currentDealStatus == 1)
          color = new Color(0.7f, 1f, 0.7f, 0.3f);
      else if (currentDealStatus == -1)
          color = new Color(1f, 0.7f, 0.7f, 0.3f);
      else
          color = new Color(1f, 1f, 0.7f, 0.3f);
    }
    ImageBackground.color = color;
  }
  
  protected override void CalculatePosition()
  {
    if (internalStorable == null || internalStorable.IsDisposed)
      return;
    base.CalculatePosition();
    Vector2 vector2 = !cellStyle.IsSlot ? InventoryUtility.CalculateInnerSize(((StorableComponent) internalStorable).Placeholder.Grid, cellStyle) : InventoryUtility.CalculateInnerSize(gridSlot, cellStyle);
    selectedImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vector2.x + cellStyle.BackgroundImageOffset.x * 2f);
    selectedImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vector2.y + cellStyle.BackgroundImageOffset.x * 2f);
  }

  public int GetSelectedCount() => selectedCount;

  public void SetSelectedCount(int count, bool isInit = false)
  {
    selectedCount = count;
    if (InputService.Instance.JoystickUsed)
    {
      _selectedImage.SetActive(count > 0);
      if (isInit)
        isSelected = false;
    }
    else
    {
      isSelected = count > 0;
      _selectedImage.SetActive(false);
    }
    Update();
  }
}
