using System;
using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory;
using Engine.Source.Components;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using InputServices;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class StorableUITrade : StorableUI
{
	// Token: 0x06000694 RID: 1684
	private void Start()
	{
		this._selectedImage.SetActive(false);
	}

	// Token: 0x06000695 RID: 1685
	protected override void Update()
	{
		if (this.internalStorable.Max > 1)
		{
			this.textCount.text = ((this.selectedCount == 0) ? this.internalStorable.Count.ToString() : (this.selectedCount.ToString() + "/" + this.internalStorable.Count.ToString()));
			if (this.textCount.gameObject != null)
			{
				this.textCount.gameObject.SetActive(true);
			}
		}
		else
		{
			this.textCount.text = null;
			if (this.textCount.gameObject != null)
			{
				this.textCount.gameObject.SetActive(false);
			}
		}
		this.selectedImage.gameObject.SetActive(this.isSelected);
		Color color = this.enabledBackgroundColor;
		if (!this.isEnabled)
		{
			color = this.disabledBackgroundColor;
		}
		else
		{
			int debugState = 0;
			IStorableComponent internalStorable = this.internalStorable;
			bool flag;
			if (internalStorable == null)
			{
				flag = null != null;
			}
			else
			{
				IStorageComponent storage = internalStorable.Storage;
				flag = ((storage != null) ? storage.Owner : null) != null;
			}
			if (flag)
			{
				bool playerBuying = this.internalStorable.Storage.Owner.GetComponent<PlayerControllerComponent>() == null;
				float basePrice = 0f;
				if (VMGlobalMarketManager.Instance != null)
				{
					basePrice = VMGlobalMarketManager.Instance.GetCurrentItemTradeGlobalPrice(this.internalStorable.Owner.Name, !playerBuying);
				}
				else
				{
					debugState = 7;
				}
				if (debugState == 0 && basePrice < 0.0001f)
				{
					debugState = 6;
				}
				float actualPrice = (playerBuying ? this.internalStorable.Invoice.SellPrice : this.internalStorable.Invoice.BuyPrice);
				if (debugState == 0 && Mathf.Abs(actualPrice - basePrice) > 0.01f)
				{
					if (playerBuying)
					{
						if (actualPrice < basePrice)
						{
							debugState = 1;
						}
						else
						{
							debugState = 2;
						}
					}
					else if (actualPrice > basePrice)
					{
						debugState = 3;
					}
					else
					{
						debugState = 4;
					}
				}
			}
			else
			{
				debugState = 5;
			}
			switch (debugState)
			{
			case 1:
				color = new Color(0.3f, 1f, 0.3f, 0.3f);
				goto IL_02D9;
			case 2:
				color = new Color(1f, 0.3f, 0.3f, 0.3f);
				goto IL_02D9;
			case 3:
				color = new Color(0.7f, 1f, 0.3f, 0.3f);
				goto IL_02D9;
			case 4:
				color = new Color(1f, 0.5f, 0.3f, 0.3f);
				goto IL_02D9;
			case 5:
				color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
				goto IL_02D9;
			case 6:
				color = new Color(0f, 1f, 1f, 0.3f);
				goto IL_02D9;
			case 7:
				color = new Color(1f, 0f, 1f, 0.3f);
				goto IL_02D9;
			}
			color = new Color(1f, 1f, 0.7f, 0.3f);
		}
		IL_02D9:
		base.ImageBackground.color = color;
	}

	// Token: 0x06000696 RID: 1686
	protected override void CalculatePosition()
	{
		if (this.internalStorable == null || this.internalStorable.IsDisposed)
		{
			return;
		}
		base.CalculatePosition();
		Vector2 vector2 = ((!this.cellStyle.IsSlot) ? InventoryUtility.CalculateInnerSize(((StorableComponent)this.internalStorable).Placeholder.Grid, this.cellStyle) : InventoryUtility.CalculateInnerSize(StorableUI.gridSlot, this.cellStyle));
		this.selectedImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vector2.x + this.cellStyle.BackgroundImageOffset.x * 2f);
		this.selectedImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vector2.y + this.cellStyle.BackgroundImageOffset.x * 2f);
	}

	// Token: 0x06000697 RID: 1687
	public int GetSelectedCount()
	{
		return this.selectedCount;
	}

	// Token: 0x06000698 RID: 1688
	public void SetSelectedCount(int count, bool isInit = false)
	{
		this.selectedCount = count;
		if (InputService.Instance.JoystickUsed)
		{
			this._selectedImage.SetActive(count > 0);
			if (isInit)
			{
				this.isSelected = false;
			}
		}
		else
		{
			this.isSelected = count > 0;
			this._selectedImage.SetActive(false);
		}
		this.Update();
	}

	// Token: 0x04000602 RID: 1538
	[SerializeField]
	private GameObject _selectedImage;

	// Token: 0x04000603 RID: 1539
	private int selectedCount;
}
