using System;
using System.Collections.Generic;

[Serializable]
public class PremiumClothingPack
{
	public string Id;
	public string BodyClothingId;
	public string LegsClothingId;
}

[Serializable]
public class SpecialPack
{
	public int Id;
	public SpecialPackType PackType;
	public string Name;
	public string RussianName;
	public string Description;
	public string RussianDescription;
	public string Icon;
	public int Price;
	public int PriceUSD;
	public int PriceForTen;
	public int PriceForTenUSD;

	public List<KeyValueInt> Items;

	public string GetPriceForTen()
	{
		string result = Convert.ToString(PriceForTen);
		return result;
	}

	public float GetPriceForTenFloat()
	{
		float result = PriceForTen;
		return result;
	}

	public string GetPrice()
	{
		string result = Convert.ToString(Price);
		return result;
	}

	public float GetPriceFloat()
	{
		float result = Price;
		return result;
	}
}

public enum SpecialPackType
{
	ItemsPack = 0,
	Subscription = 1,
	Car = 2
}

public static class SpecialPacksRegistry
{
	public static List<SpecialPack> SpecialPacks = new List<SpecialPack>
	{
		new SpecialPack
		{
			Id = 1,
			Name = "Builder",
			RussianName = "Строитель",
			Price = 1,
			PriceForTenUSD = 1,
			PriceForTen = 9,
			Items = new List<KeyValueInt>
			{
				new KeyValueInt
				{
					Key = "travelpack",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "roadworkerShirt",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "blueJeans",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "minerHelmet",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "Wooden Wall",
					Value = 20
				},
				new KeyValueInt
				{
					Key = "Wooden Door",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Wooden Doorway",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Wooden Foundation",
					Value = 20
				},
				new KeyValueInt
				{
					Key = "Wooden Window",
					Value = 10
				},
				new KeyValueInt
				{
					Key = "Wooden Shutter",
					Value = 10
				},
				new KeyValueInt
				{
					Key = "Wooden Support",
					Value = 20
				},
				new KeyValueInt
				{
					Key = "Brazier",
					Value = 10
				},
				new KeyValueInt
				{
					Key = "Nails",
					Value = 50
				},
				new KeyValueInt
				{
					Key = "sledgehammer",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "Wire",
					Value = 20
				}
			},

			RussianDescription = "1. Кувалда\n2. Одежда рабочего\n3. Деревянные стены\n4. Деревянные двери\n5. Деревянные дверные проёмы\n6. Деревянные фундаменты\n7. Деревянные окна\n8. Деревянные ставни\n9. Факел для стен\n10. Гвозди\n11. Проволока",
			Description = "1. Sledgehammer\n2. Roadworker clothing\n3. Wooden walls\n4. Wooden doors\n5. Wooden doorways\n6. Wooden foundations\n7. Wooden windows\n8. Wooden shutters\n9. Braziers\n10. Nails\n11. Wire"
		},
		new SpecialPack
		{
			Id = 2,
			Name = "Special Forces",
			RussianName = "Спецназовец",
			Price = 2,
			PriceForTenUSD = 2,
			PriceForTen = 18,

			Items = new List<KeyValueInt>
			{
				new KeyValueInt
				{
					Key = "travelpack",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "mm5_56",
					Value = 300
				},
				new KeyValueInt
				{
					Key = "beret",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "specialForcesShirt",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "specialForcesPants",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "policeVest",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "binoculars",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "MRE",
					Value = 3
				},
				new KeyValueInt
				{
					Key = "Adrenaline",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Bottled Water",
					Value = 3
				},
				new KeyValueInt
				{
					Key = "Landmine",
					Value = 3
				},
				new KeyValueInt
				{
					Key = "m16",
					Value = 1
				}
			},

			RussianDescription = "1.М16 (+ 300 патронов)\n2. Одежда спецназа\n3. Полицейская броня\n4. Бинокль\n5. Сухой паёк\n6. Адреналин\n7. Вода\n8. Мины\n",
			Description = "1.M16(+ 300 ammo)\n2. SAWT clothing\n3. Binoculars\n4. MRE\n5. MRE\n6. Adrenaline\n7. Bottled Water\n8. Landmine"
		},
		new SpecialPack
		{
			Id = 3,
			Name = "Doctor",
			RussianName = "Доктор",
			Price = 1,
			PriceForTenUSD = 1,
			PriceForTen = 9,

			Items = new List<KeyValueInt>
			{
				new KeyValueInt
				{
					Key = "travelpack",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "doctorShirt",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "doctorPants",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "Purification Tablets",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Vaccine",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Medkit",
					Value = 3
				},
				new KeyValueInt
				{
					Key = "Antibiotics",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Blood Bag",
					Value = 3
				},
				new KeyValueInt
				{
					Key = "Dressing",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Painkillers",
					Value = 10
				},
				new KeyValueInt
				{
					Key = "Morphine",
					Value = 10
				},
				new KeyValueInt
				{
					Key = "Vitamins",
					Value = 15
				},
				new KeyValueInt
				{
					Key = "Splint",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "butcherKnife",
					Value = 1
				}
			},

			RussianDescription = "1. Нож мясника\n2. Одежда доктора\n3. Очищающие таблетки\n4. Вакцина\n5. Аптечка\n6. Антибиотики\n7. Пакеты крови\n8. Перевязочные наборы\n9. Обезболивающее\n10. Морфин\n11. Витамины\n12. Шины",
			Description = "1. Butcher knife\n2. Doctor clothing\n3. Purification Tablets\n4. Vaccines\n5. Medkits\n6. Antibiotics\n7. Blood bags\n8. Dressings\n9. Painkillers\n10. Morphine\n11. Vitamins\n12. Splints"
		},
		new SpecialPack
		{
			Id = 4,
			Name = "Farmer",
			RussianName = "Фермер",
			Price = 1,
			PriceForTenUSD = 1,
			PriceForTen = 9,

			Items = new List<KeyValueInt>
			{
				new KeyValueInt
				{
					Key = "travelpack",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "farmerHat",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "brownSuspenders",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "grayBlueJeans",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "Greenhouse Foundation",
					Value = 3
				},
				new KeyValueInt
				{
					Key = "Corn Seed",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Pumkin Seed",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Turnip Seed",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Tomato Seed",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Carrot Seed",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Fertilizer",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Large Bottled Water",
					Value = 3
				},
				new KeyValueInt
				{
					Key = "shovel",
					Value = 1
				}
			},

			RussianDescription = "1. Лопата\n2. Одежда фермера\n3. Ферма\n4. Различные семена\n5. Удобрения\n6. Большие бутылки воды",
			Description = "1. Shovel\n2. Farmer clothing\n3. Greenhouse foundation\n4. Different seeds\n5. Large bottled water"
		},
		new SpecialPack
		{
			Id = 5,
			Name = "Sniper",
			RussianName = "Снайпер",
			Price = 2,
			PriceForTenUSD = 2,
			PriceForTen = 18,

			Items = new List<KeyValueInt>
			{
				new KeyValueInt
				{
					Key = "travelpack",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "dragunovSniperRifle",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "ushanka",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "mercenaryShirt",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "mercenaryPants",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "MRE",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Bottled Water",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "mm7_62",
					Value = 50
				}
			},

			RussianDescription = "1. СВД (+50 патронов)\n2. Одежда наёмника\n3. Ушанка\n4. Сухой паёк\n5. Вода",
			Description = "1. Dragunov sniper rifle (+50 ammo)\n2. Mercenary clothing\n3. Ushanka\n4. MRE\n5. Water"
		},
		new SpecialPack
		{
			Id = 6,
			Name = "Bomberman",
			RussianName = "Подрывник",
			Price = 2,
			PriceForTenUSD = 2,
			PriceForTen = 18,

			Items = new List<KeyValueInt>
			{
				new KeyValueInt
				{
					Key = "travelpack",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "magnum357",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "bandana",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "Raw Explosives",
					Value = 20
				},
				new KeyValueInt
				{
					Key = "Binoculars",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "Trip Mine",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Landmine",
					Value = 10
				},
				new KeyValueInt
				{
					Key = "minesweeperShirt",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "minesweeperPants",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "mm9",
					Value = 50
				}
			},

			RussianDescription = "1. Magnum 357 (+50 патронов)\n2. Одежда сапёра\n3. Бандана\n4. Сырьё для взрывчатки\n5. Бинокль\n6. Шахтёрские мины\n7. Мины",
			Description = "1. Magnum 357 (+50 ammo)\n2. Minesweeper clothing\n3. Bandana\n4. Raw Explosives\n5. Binoculars\n6. Trip mine\n7. Mine"
		},
		new SpecialPack
		{
			Id = 7,
			Name = "Stalker",
			RussianName = "Сталкер",
			Price = 3,
			PriceForTenUSD = 3,
			PriceForTen = 27,

			Items = new List<KeyValueInt>
			{
				new KeyValueInt
				{
					Key = "travelpack",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "m4a1",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "mercenaryShirt",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "mercenaryPants",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "militaryNVG",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "MRE",
					Value = 3
				},
				new KeyValueInt
				{
					Key = "Bottled Energy",
					Value = 2
				},
				new KeyValueInt
				{
					Key = "Bottled Water",
					Value = 3
				},
				new KeyValueInt
				{
					Key = "Campfire",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "Vaccine",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "mm5_56",
					Value = 150
				}
			},

			RussianDescription = "1. M4A1 (+150 патронов)\n2. Одежда наёмника\n3. ПНВ\n4. Сухой паёк\n5. Энергетик\n6. Вода\n7. Костёр\n8. Вакцины",
			Description = "1. M4A1 (+150 ammo)\n2. Mercenary clothing\n3. Military NVG\n4. MRE\n5. Bottled Energy\n6. Bottled Water\n7. Campfire\n8. Vaccine"
		},
		new SpecialPack
		{
			Id = 8,
			Name = "Trapper",
			RussianName = "Спец по ловушкам",
			Price = 1,
			PriceForTenUSD = 1,
			PriceForTen = 9,

			Items = new List<KeyValueInt>
			{
				new KeyValueInt
				{
					Key = "travelpack",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "magnum357",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "minesweeperShirt",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "minesweeperPants",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "Caltrop",
					Value = 10
				},
				new KeyValueInt
				{
					Key = "Electric Trap",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Snare",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Barbed Wire",
					Value = 10
				},
				new KeyValueInt
				{
					Key = "Binoculars",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "mm9",
					Value = 50
				}
			},

			RussianDescription = "1. Magnum 357 (+50 патронов)\n2. Одежда сапёра\n3. Шипы\n4. Электрические ловушки\n5. Западни\n6. Колючая проволока\n7. Бинокль",
			Description = "1. Magnum 357 (+50 ammo)\n2. Minesweeper clothing\n3. Caltrop\n4. Electric Trap\n5. Snare\n6. Barbed Wire\n7. Binoculars"
		},
		new SpecialPack
		{
			Id = 9,
			Name = "Rambo",
			RussianName = "Рэмбо",
			Price = 5,
			PriceForTenUSD = 5,
			PriceForTen = 45,

			Items = new List<KeyValueInt>
			{
				new KeyValueInt
				{
					Key = "travelpack",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "m249",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "commandoPants",
					Value = 1
				},
				new KeyValueInt
				{
					Key = "Adrenaline",
					Value = 10
				},
				new KeyValueInt
				{
					Key = "Rope",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Bottled Energy",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Cooked Bacon",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Painkillers",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "Duct Tape",
					Value = 5
				},
				new KeyValueInt
				{
					Key = "mm5_56",
					Value = 400
				}
			},

			RussianDescription = "1. M249 (+400 патронов)\n2. Коммандос (штаны)\n3. Адреналин\n4. Энергетик\n5. Верёвка\n6. Скотч\n7. Обезболивающее\n8. Жаренная свинина",
			Description = "1. M249 (+400 ammo)\n2. Commando pants\n3. Adrenaline\n4. Bottled Energy\n5. Rope\n6. Duct Tape\n7. Painkillers\n8. Cooked Bacon"
		},
		new SpecialPack
		{
			Id = 333,
			PackType = SpecialPackType.Subscription,
			Name = "VIP subscription",
			RussianName = "VIP подписка",
			Price = 6,
			PriceForTen = 25,
			PriceUSD = 1,
			PriceForTenUSD = 3,
			Items = new List<KeyValueInt>(),
			Icon = "premium",
			RussianDescription = "1) Бесплатные скины\n2) Бесплатный стартовый набор новичка\n3) Дополнительные возможности изменения внешнего вида\n4) Возможность создавать приватные комнаты\n5) Шанс выпадения сетевых предметов при смерти от монстров 5-10%\n\nСписок бонусов от подписки будет значительно расширяться в обновлениях!",
			Description = "1) Free starter kit\n2) Extended customization options\n3) Ability to create private rooms\n4) Loot will drop after you were killed by zombie with only 5-10% chance instead of 100%.\n\nMore VIP bonuses will be added in the updates!"
		},
		new SpecialPack
		{
			Id = 733,
			PackType = SpecialPackType.Subscription,
			Name = "SUPERVIP subscription",
			RussianName = "SUPERVIP подписка",
			Price = 10,
			PriceForTen = 40,
			PriceUSD = 2,
			PriceForTenUSD = 4,
			Items = new List<KeyValueInt>(),
			Icon = "premium",
			RussianDescription = "Престижный статус в игре\n +все бонусы обычного VIP\n + возможность носить плащи\n +отображение надписи VIP в имени\n ...и многое другое!",
			Description = "All bonuses of simple VIP + more!"
		},
		new SpecialPack
		{
			Id = 102,
			PackType = SpecialPackType.Car,
			Name = "Zeus",
			RussianName = "Zeus",
			Price = 25,
			PriceUSD = 2,
			PriceForTen = 250,
			Items = new List<KeyValueInt>(),
			Icon = "Zeus",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		},
		new SpecialPack
		{
			Id = 101,
			PackType = SpecialPackType.Car,
			Name = "Titan Z1",
			RussianName = "Titan Z1",
			Price = 50,
			PriceUSD = 5,
			PriceForTen = 500,
			Items = new List<KeyValueInt>(),
			Icon = "TitanZ1",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		},
		new SpecialPack
		{
			Id = 103,
			PackType = SpecialPackType.Car,
			Name = "Doge ReCharger",
			RussianName = "Doge ReCharger",
			Price = 35,
			PriceUSD = 3,
			PriceForTen = 350,
			Items = new List<KeyValueInt>(),
			Icon = "Charger",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		},
		new SpecialPack
		{
			Id = 104,
			PackType = SpecialPackType.Car,
			Name = "Kamaro",
			RussianName = "Kamaro",
			Price = 75,
			PriceUSD = 7,
			PriceForTen = 750,
			Items = new List<KeyValueInt>(),
			Icon = "Camaro",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		},
		new SpecialPack
		{
			Id = 105,
			PackType = SpecialPackType.Car,
			Name = "Hammer",
			RussianName = "Hammer",
			Price = 200,
			PriceUSD = 20,
			PriceForTen = 2000,
			Items = new List<KeyValueInt>(),
			Icon = "Hummer",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		},
		new SpecialPack
		{
			Id = 106,
			PackType = SpecialPackType.Car,
			Name = "Rorshe 911",
			RussianName = "Rorshe 911",
			Price = 100,
			PriceUSD = 10,
			PriceForTen = 1000,
			Items = new List<KeyValueInt>(),
			Icon = "Porshe 911",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		},
		new SpecialPack
		{
			Id = 107,
			PackType = SpecialPackType.Car,
			Name = "Budgetti",
			RussianName = "Budgetti",
			Price = 499,
			PriceUSD = 50,
			PriceForTen = 4990,
			Items = new List<KeyValueInt>(),
			Icon = "Bugatti",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		},
		new SpecialPack
		{
			Id = 108,
			PackType = SpecialPackType.Car,
			Name = "Supercar GT",
			RussianName = "Supercar GT",
			Price = 120,
			PriceUSD = 12,
			PriceForTen = 1200,
			Items = new List<KeyValueInt>(),
			Icon = "Citroen GT",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		},
		new SpecialPack
		{
			Id = 109,
			PackType = SpecialPackType.Car,
			Name = "Monster",
			RussianName = "Monster",
			Price = 250,
			PriceUSD = 25,
			PriceForTen = 2500,
			Items = new List<KeyValueInt>(),
			Icon = "Monster",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		},
		new SpecialPack
		{
			Id = 110,
			PackType = SpecialPackType.Car,
			Name = "Rat Rod",
			RussianName = "Rat Rod",
			Price = 150,
			PriceUSD = 15,
			PriceForTen = 1500,
			Items = new List<KeyValueInt>(),
			Icon = "Rat Rod",
			RussianDescription = "Для выбора автомобиля, который вы купили, необходимо найти любую машину, сесть на водительское место и сменить авто, нажав на 'B'\n",
			Description = "To select a car you've bought, it is necessary to find any car, take the driver's seat and replace the car by pressing 'B'\n"
		}
	};

	public static string GetShopID_IOS(string shop_id)
	{
		string text = "com.cubedz.shop_id" + shop_id;
		if (shop_id == "333")
		{
			text += "_";
		}
		return text;
	}
}
