﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace BannerKings.Managers.Items
{
    public class BKItemCategories : DefaultTypeInitializer<BKItemCategories, ItemCategory>
    {
        public ItemCategory Book { get; private set; }
        public ItemCategory Fruit { get; private set; }
        public ItemCategory Bread { get; private set; }
        public ItemCategory Pie { get; private set; }
        public ItemCategory Honey { get; private set; }
        public ItemCategory Gold { get; private set; }
        public ItemCategory Limestone { get; private set; }
        public ItemCategory Marble { get; private set; }
        public ItemCategory Gems { get; private set; }
        public ItemCategory Mead { get; private set; }
        public ItemCategory Garum { get; private set; }
        public ItemCategory Spice { get; private set; }
        public ItemCategory Papyrus { get; private set; }
        public ItemCategory Ink { get; private set; }
        public ItemCategory Dyes { get; private set; }
        public ItemCategory Eggs { get; private set; }
        public ItemCategory Oxen { get; private set; }

        public override IEnumerable<ItemCategory> All => throw new NotImplementedException();

        public override void Initialize()
        {
            Oxen = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("Oxen"));
            Oxen.InitializeObject(true, 7, 0, ItemCategory.Property.None, DefaultItemCategories.Cow, 0.1f, true);

            Eggs = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("Eggs"));
            Eggs.InitializeObject(true, 25, 0, ItemCategory.Property.BonusToFoodStores, DefaultItemCategories.Meat, 0.1f);

            Dyes = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("Dyes"));
            Dyes.InitializeObject(true, 5, 10, ItemCategory.Property.BonusToProsperity);

            Papyrus = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("Papyrus"));
            Papyrus.InitializeObject(true, 5, 10, ItemCategory.Property.BonusToTax);

            Ink = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("Ink"));
            Ink.InitializeObject(true, 5, 10, ItemCategory.Property.BonusToTax);

            Spice = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("Spice"));
            Spice.InitializeObject(true, 20, 60, ItemCategory.Property.BonusToProsperity);

            Book = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("book"));
            Book.InitializeObject(false, 0, 0);

            Fruit = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("apple"));
            Fruit.InitializeObject(true, 20, 0, ItemCategory.Property.BonusToFoodStores, DefaultItemCategories.DateFruit, 0.2f);

            Bread = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("bread"));
            Bread.InitializeObject(true, 100, 5, ItemCategory.Property.BonusToFoodStores, DefaultItemCategories.Grain, 0.3f);

            Pie = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("pie"));
            Pie.InitializeObject(true, 20, 30, ItemCategory.Property.BonusToFoodStores, DefaultItemCategories.Grain, 0.1f);

            Honey = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("honey"));
            Honey.InitializeObject(true, 15, 30, ItemCategory.Property.BonusToFoodStores, DefaultItemCategories.Cheese, 0.1f);

            Mead = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("mead"));
            Mead.InitializeObject(true, 10, 5, ItemCategory.Property.BonusToFoodStores, DefaultItemCategories.Beer, 0.2f);

            Garum = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("garum"));
            Garum.InitializeObject(true, 10, 5, ItemCategory.Property.BonusToFoodStores);

            Gold = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("gold"));
            Gold.InitializeObject(true, 0, 0, ItemCategory.Property.BonusToTax);

            Limestone = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("limestone"));
            Limestone.InitializeObject(true, 0, 0, ItemCategory.Property.BonusToProduction);

            Marble = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("marble"));
            Marble.InitializeObject(true, 0, 0, ItemCategory.Property.BonusToProsperity);

            Gems = Game.Current.ObjectManager.RegisterPresumedObject(new ItemCategory("gems"));
            Gems.InitializeObject(true, 0, 1, ItemCategory.Property.None);

            DefaultItemCategories.Hides.InitializeObject(true, 10, 5, ItemCategory.Property.None, null, 0f, false, true);
            DefaultItemCategories.Arrows.InitializeObject(true, 10, 10, ItemCategory.Property.None, null, 0f, false, true);
        }
    }
}