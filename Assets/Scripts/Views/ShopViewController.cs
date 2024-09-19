using JsonFx.Json;
using UnityEngine;

public class ShopViewController : MonoBehaviour
{
	private const int PremiumPackId = 333;
	private const int SuperPremiumPackId = 733;

	public static ShopViewController I;

	[SerializeField] GameObject _mainMenu;
	[SerializeField] tk2dTextMesh _nameLabel;
	[SerializeField] ModelClothingController _clothingController;
	[SerializeField] GameObject _characterView;
	[SerializeField] tk2dSprite _packIcon;
	[SerializeField] GameObject _descriptionObj;
	[SerializeField] ShadowText _buyText;
	[SerializeField] ShadowText _buyTenText;
	[SerializeField] GameObject _simplePackDescription;
	[SerializeField] tk2dTextMesh _purchasedCount;
	[SerializeField] ShadowText _price;
	[SerializeField] ShadowText _packDescription;
	[SerializeField] ShadowText _buyTenPrice;
	[SerializeField] GameObject _friendsMenu;
	[SerializeField] tk2dSprite _packIconSuperVip;
	[SerializeField] tk2dSprite _carIconTitanZ1;
	[SerializeField] tk2dSprite _carIconDodge;
	[SerializeField] tk2dSprite _carIconComaro;
	[SerializeField] tk2dSprite _carIconHummer;
	[SerializeField] tk2dSprite _carIconPorshe;
	[SerializeField] tk2dSprite _carIconBugatti;
	[SerializeField] tk2dSprite _carIconCitroenGt;
	[SerializeField] tk2dSprite _carIconMonsterPolice;
	[SerializeField] tk2dSprite _carIconRatRot;
	[SerializeField] tk2dSprite _carIconZeus;
	[SerializeField] GameObject _btnBuy;
	[SerializeField] GameObject _btnGift;

	[SerializeField] GameObject _btnBuy10;
	[SerializeField] GameObject _btnGiftBuy10;
	[SerializeField] GameObject _tenPriceLbl;
	[SerializeField] GameObject _shareBtn;

	private ContentInfo _contentInfo;
	private int _currentPackIndex = 3;

	public int callWindow = -1;

	public int GetCurrentPackIndex()
	{
		return _currentPackIndex;
	}

	private void Awake()
	{
		I = this;
	}

	private void OnEnable()
	{
		_currentPackIndex = MainMenuController.I.PackIndex;
		if (_contentInfo == null)
		{
			TextAsset textAsset = (TextAsset)Resources.Load("content");
			if (!string.IsNullOrEmpty(textAsset.text))
			{
				_contentInfo = JsonReader.Deserialize<ContentInfo>(textAsset.text);
				_contentInfo.Initialize();
			}
		}
		UpdateCurrentView();
	}

	private void DisableAllIcon()
	{
		_packIconSuperVip.gameObject.SetActive(false);
		_packIcon.gameObject.SetActive(false);
		_carIconTitanZ1.gameObject.SetActive(false);
		_carIconZeus.gameObject.SetActive(false);
		_carIconDodge.gameObject.SetActive(false);
		_carIconComaro.gameObject.SetActive(false);
		_carIconHummer.gameObject.SetActive(false);
		_carIconPorshe.gameObject.SetActive(false);
		_carIconBugatti.gameObject.SetActive(false);
		_carIconCitroenGt.gameObject.SetActive(false);
		_carIconMonsterPolice.gameObject.SetActive(false);
		_carIconRatRot.gameObject.SetActive(false);
	}

	private void ToggleTenBtn(bool flag)
	{
		_tenPriceLbl.SetActive(flag);
		_btnBuy10.SetActive(flag);
		_btnGiftBuy10.SetActive(flag);
		if (!flag)
		{
			_btnGift.transform.localPosition = new Vector3(0.85f, 0.4f, 0f);
		}
		else
		{
			_btnGift.transform.localPosition = new Vector3(-0.9f, 2.169f, 0f);
		}
	}

	private void ToggleMobileBtn(bool flag)
	{
		_btnBuy.SetActive(flag);
		_price.transform.gameObject.SetActive(flag);
	}

	public void UpdateCurrentView()
	{
		ToggleMobileBtn(true);
		_price.transform.localPosition = new Vector3(-0.9f, -2.5f, -0.9f);
		ToggleTenBtn(false);
		_btnGift.SetActive(true);
		if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType != SpecialPackType.Car)
		{
			ToggleTenBtn(true);
		}
		else
		{
			int id = SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id;
			if (id == 102 || id == 103 || id == 108)
			{
				_btnGift.SetActive(false);
			}
		}
		if (DataKeeper.Language == Language.Russian)
		{
			_nameLabel.text = SpecialPacksRegistry.SpecialPacks[_currentPackIndex].RussianName;
			_packDescription.SetText(SpecialPacksRegistry.SpecialPacks[_currentPackIndex].RussianDescription);
		}
		else
		{
			_nameLabel.text = SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Name;
			_packDescription.SetText(SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Description);
		}
		DisableAllIcon();
		_descriptionObj.SetActive(SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 333 || SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 733 || SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType == SpecialPackType.Car);
		if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 333 || SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 733 || SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType == SpecialPackType.Car)
		{
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 333)
			{
				string text = "Делает вашу игру более комфортной";
				if (DataKeeper.Language == Language.English)
				{
					text = "Makes your game more comfortable";
				}
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text;
				_packIcon.gameObject.SetActive(true);
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 733)
			{
				string text2 = "Cамый крутой статус в игре";
				if (DataKeeper.Language == Language.English)
				{
					text2 = "The coolest status in the game!";
				}
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text2;
				_packIconSuperVip.gameObject.SetActive(true);
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 101)
			{
				_carIconTitanZ1.gameObject.SetActive(true);
				string text3 = "Titan Z1 - крутой полицейский джип. Наводите порядок на серверах вместе с друзьями!";
				if (DataKeeper.Language == Language.English)
				{
					text3 = "Titan Z1 is a cool police SUV. Establish order on servers together with your friends!";
				}
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text3;
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 102)
			{
				string text4 = "Zeus - крутой спорткар для серьезных игроков! Дави врагов со стилем!";
				if (DataKeeper.Language == Language.English)
				{
					text4 = "Zeus is a fabulous sport car for serious players! Crush enemies in style!";
				}
				_carIconZeus.gameObject.SetActive(true);
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text4;
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 103)
			{
				string text5 = "Doge ReCharger — культовый автомобиль. Настоящая классика.";
				if (DataKeeper.Language == Language.English)
				{
					text5 = "Doge ReCharger is an iconic vehicle. Real classics.";
				}
				_carIconDodge.gameObject.SetActive(true);
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text5;
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 104)
			{
				string text6 = "Kamaro - легендарный масл кар. А у тебя уже есть свой Bumblebee?";
				if (DataKeeper.Language == Language.English)
				{
					text6 = "Kamaro is a legendary muscle car. Do you have a Bumblebee of your own already?";
				}
				_carIconComaro.gameObject.SetActive(true);
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text6;
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 105)
			{
				string text7 = "Hammer - знаменитый внедорожник для серьезных спецопераций.";
				if (DataKeeper.Language == Language.English)
				{
					text7 = "Hammer is a famous offroadster for no-nonsense special ops.";
				}
				_carIconHummer.gameObject.SetActive(true);
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text7;
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 106)
			{
				string text8 = "Rorshe 911 - классика жанра. Машина для тех, кто понимает толк в автомобилях.";
				if (DataKeeper.Language == Language.English)
				{
					text8 = "Rorshe 911 is a classics of the genre. A car intended for those who has good car sense.";
				}
				_carIconPorshe.gameObject.SetActive(true);
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text8;
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 107)
			{
				string text9 = "Budgetti - король среди суперкаров. Можете ли вы себе позволить ездить на таком автомобиле?";
				if (DataKeeper.Language == Language.English)
				{
					text9 = "Budgetti is a king among super cars. Are you able to afford driving such a car?";
				}
				_carIconBugatti.gameObject.SetActive(true);
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text9;
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 108)
			{
				string text10 = "Supercar GT - новейший  суперкар. Удивленные глаза игроков вам обеспечены!";
				if (DataKeeper.Language == Language.English)
				{
					text10 = "Supercar GT is a top-of-the-line super car. You are sure to make other players pie-eyed!";
				}
				_carIconCitroenGt.gameObject.SetActive(true);
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text10;
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 109)
			{
				string text11 = "Monster - джип-монстр с повышенной проходимостью и уникальным внешним видом.";
				if (DataKeeper.Language == Language.English)
				{
					text11 = "Monster is a monster jeep enjoying cross-country capacity and unique appearance.";
				}
				_carIconMonsterPolice.gameObject.SetActive(true);
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text11;
			}
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id == 110)
			{
				string text12 = "Rat Rod - заряженный полицейский автомобиль ручной сборки.";
				if (DataKeeper.Language == Language.English)
				{
					text12 = "Rat Rod is a loaded hand-assembled police car.";
				}
				_carIconRatRot.gameObject.SetActive(true);
				_descriptionObj.transform.GetChild(0).GetComponent<tk2dTextMesh>().text = text12;
			}
		}
		EquipCharacter();
		if (!string.IsNullOrEmpty(SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Icon))
		{
			_simplePackDescription.SetActive(false);
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType != SpecialPackType.Car)
			{
				if (DataKeeper.Language == Language.Russian)
				{
					_price.SetText("Цена: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPrice() + " " + GetCurrencyName_RU() + "\n(на неделю)");
					_buyTenPrice.SetText("Цена: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPriceForTen() + " " + GetCurrencyName_RU() + "\n(на месяц)");
				}
				else
				{
					_price.SetText("Price: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPrice() + " " + GetCurrencyName_EN() + "\n(per week)");
					_buyTenPrice.SetText("Price: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPriceForTen() + " " + GetCurrencyName_EN() + "\n(per month)");
				}
			}
			else if (DataKeeper.Language == Language.Russian)
			{
				_price.SetText("Цена: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPrice() + " " + GetCurrencyName_RU() + "\n(навсегда)");
			}
			else
			{
				_price.SetText("Price: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPrice() + " " + GetCurrencyName_EN() + "\n(forever)");
			}
		}
		else
		{
			_simplePackDescription.SetActive(true);
			_packIcon.gameObject.SetActive(false);
			_packIconSuperVip.gameObject.SetActive(false);
			if (!DataKeeper.IsUserDummy)
			{
				PurchasedItemsBackensInfo purchasedItemsBackensInfo = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id);
				if (purchasedItemsBackensInfo != null)
				{
					if (DataKeeper.Language == Language.Russian)
					{
						_purchasedCount.text = "Куплено: " + purchasedItemsBackensInfo.count;
					}
					else
					{
						_purchasedCount.text = "Purchased: " + purchasedItemsBackensInfo.count;
					}
				}
				else if (DataKeeper.Language == Language.Russian)
				{
					_purchasedCount.text = "Куплено: " + 0;
				}
				else
				{
					_purchasedCount.text = "Purchased: " + 0;
				}
			}
			else
			{
				_purchasedCount.text = string.Empty;
			}
			if (DataKeeper.Language == Language.Russian)
			{
				_price.SetText("Цена: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPrice() + " " + GetCurrencyName_RU());
				_buyTenPrice.SetText("Цена: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPriceForTen() + " " + GetCurrencyName_RU());
			}
			else
			{
				_price.SetText("Price: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPrice() + " " + GetCurrencyName_EN());
				_buyTenPrice.SetText("Price: " + SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPriceForTen() + " " + GetCurrencyName_EN());
			}
		}
		if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType != SpecialPackType.Car)
		{
			_btnBuy.SetActive(true);
		}
		else
		{
			PurchasedItemsBackensInfo purchasedItemsBackensInfo2 = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id && item.count > 0);
			if (purchasedItemsBackensInfo2 == null)
			{
				_btnBuy.SetActive(true);
				_shareBtn.SetActive(false);
			}
			else
			{
				_price.transform.localPosition = new Vector3(-0.9f, -2.91f, -0.9f);
				_price.SetText("Уже куплено.");
				_btnBuy.SetActive(false);
				_shareBtn.SetActive(true);
			}
		}
		SetBuyButton(SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPriceFloat() <= 0f, SpecialPacksRegistry.SpecialPacks[_currentPackIndex].GetPriceForTenFloat() <= 0f);
		if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType == SpecialPackType.ItemsPack)
		{
			ToggleMobileBtn(false);
		}
	}

	public string GetLocalPrice()
	{
		Debug.LogError("not implemented yet");
		return ":)";
	}

	public string GetCurrencyName_RU()
	{
		return "$";
	}

	public string GetCurrencyName_EN()
	{
		return "$";
	}

	private void SetBuyButton(bool isFree, bool tenIsFree)
	{
		if (DataKeeper.Language == Language.Russian)
		{
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType == SpecialPackType.Subscription)
			{
				_buyTenText.SetText((!tenIsFree) ? "Купить" : "Взять");
			}
			else
			{
				_buyTenText.SetText((!tenIsFree) ? "Купить 10" : "Взять");
			}
			_buyText.SetText((!isFree) ? "Купить" : "Взять");
		}
		else
		{
			if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType == SpecialPackType.Subscription)
			{
				_buyTenText.SetText((!tenIsFree) ? "Buy" : "Get");
			}
			else
			{
				_buyTenText.SetText((!tenIsFree) ? "Buy 10" : "Get");
			}
			_buyText.SetText((!isFree) ? "Buy" : "Get");
		}
	}

	private void TakeOffAll()
	{
		_clothingController.TakeOff(ClothingBodyPart.Headwear);
		_clothingController.TakeOff(ClothingBodyPart.Vest);
		_clothingController.TakeOff(ClothingBodyPart.Backpack);
		_clothingController.TakeOff(ClothingBodyPart.Bodywear);
		_clothingController.TakeOff(ClothingBodyPart.Legwear);
		_clothingController.RemoveFromHand();
	}

	private void EquipCharacter()
	{
		if (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id != 333 && SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id != 733 && SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType != SpecialPackType.Car)
		{
			_characterView.SetActive(true);
			TakeOffAll();
			{
				foreach (KeyValueInt item in SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Items)
				{
					Clothing clothingInfo = _contentInfo.GetClothingInfo(item.Key);
					if (clothingInfo != null)
					{
						switch (clothingInfo.BodyPart)
						{
						case ClothingBodyPart.Backpack:
						case ClothingBodyPart.Headwear:
						case ClothingBodyPart.Vest:
							_clothingController.Equip(clothingInfo.BodyPart, clothingInfo.Prefab);
							break;
						case ClothingBodyPart.Bodywear:
						case ClothingBodyPart.Legwear:
							_clothingController.Equip(clothingInfo.BodyPart, clothingInfo.Id);
							break;
						}
					}
					else
					{
						Weapon weaponInfo = _contentInfo.GetWeaponInfo(item.Key);
						if (weaponInfo != null)
						{
							_clothingController.AddInHand(weaponInfo);
						}
					}
				}
				return;
			}
		}
		_characterView.SetActive(false);
	}

	private void OnClickLeft()
	{
		_currentPackIndex = (SpecialPacksRegistry.SpecialPacks.Count + (_currentPackIndex - 1)) % SpecialPacksRegistry.SpecialPacks.Count;
		UpdateCurrentView();
	}

	private void OnClickRight()
	{
		_currentPackIndex = (_currentPackIndex + 1) % SpecialPacksRegistry.SpecialPacks.Count;
		UpdateCurrentView();
	}

	private void OnClickBuy()
	{
		switch (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType)
		{
		case SpecialPackType.ItemsPack:
			JsSpeeker.I.BuySomething(SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id.ToString());
			break;
		case SpecialPackType.Subscription:
		{
			int num = SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id + 200;
			JsSpeeker.I.BuySomething(num.ToString());
			break;
		}
		case SpecialPackType.Car:
			JsSpeeker.I.BuySomething(SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id.ToString());
			break;
		}
	}

	private void OnClickBuyTen()
	{
		switch (SpecialPacksRegistry.SpecialPacks[_currentPackIndex].PackType)
		{
		case SpecialPackType.ItemsPack:
		{
			int num = SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id + 600;
			JsSpeeker.I.BuySomething(num.ToString());
			break;
		}
		case SpecialPackType.Subscription:
			JsSpeeker.I.BuySomething(SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id.ToString());
			break;
		}
	}

	public void OnClickMenu()
	{
		base.gameObject.SetActive(false);
		_mainMenu.SetActive(true);
	}

	private void OnClickFriendsBuy()
	{
		callWindow = 1;
		EnableFriendsList();
	}

	private void OnClickFriendsBuyTen()
	{
		callWindow = 2;
		EnableFriendsList();
	}

	private void EnableFriendsList()
	{
		base.gameObject.SetActive(false);
		_friendsMenu.SetActive(true);
	}

	private void OnClickShareCar()
	{
		JsSpeeker.I.ShareCar(SpecialPacksRegistry.SpecialPacks[_currentPackIndex].Id);
	}
}
