using System;
using System.Collections.Generic;
using GildedRose.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static GildedRose.Console.Program;

namespace GildedRose.Test
{
    [TestClass]
    public class InventoryProgramTests
    {
        private readonly string SulfurasItemName = "Sulfuras, Hand of Ragnaros";
        private readonly string BrieItemName = "Aged Brie";
        private readonly string BackStagePassItemName = "Backstage passes to a TAFKAL80ETC concert";
        private readonly string ConjuredManaCakeItemName = "Conjured Mana Cake";

        private Item CreateItem1()
        {
            return new Item { Name = "Item1", SellIn = 1, Quality = 1 };
        }
        private Item CreateItem2()
        {
            return new Item { Name = "Item2", SellIn = 1 };
        }
        private Item CreateSulfuras()
        {
            return new Item { Name = SulfurasItemName, SellIn = 1, Quality = 1 };
        }
        private Item CreateBrie()
        {
            return new Item { Name = BrieItemName, SellIn = 1, Quality = 1 };
        }
        private Item CreateBackStagePass()
        {
            return new Item { Name = BackStagePassItemName, Quality = 1, SellIn = 15 };
        }
        private Item CreateConjuredItem()
        {
            return new Item { Name = ConjuredManaCakeItemName, SellIn = 3, Quality = 6 };
        }

        [TestMethod]
        public void TestWeCanFindItemByNameReturnsNullWhenThereIsNoMatch()
        {
            var inventoryProgram = new Program
            {
                Items = new List<Item> { CreateItem1(), CreateItem2() }
            };
            var item = inventoryProgram.FindItemByName("NonExistentItem");
            Assert.IsNull(item);
        }

        [TestMethod]
        public void TestWeCanFindItemByNameReturnsNullWhenThereAreNoItems()
        {
            var inventoryProgram = new Program();
            var item = inventoryProgram.FindItemByName("NonExistentItem");
            Assert.IsNull(item);
        }

        private Item UpdateQualityReturnItem(IList<Item> items, string itemName)
        {
            var inventoryProgram = new Program
            {
                Items = items
            };
            inventoryProgram.UpdateQuality();
            return inventoryProgram.FindItemByName(itemName);
        }

        // - At the end of each day our system lowers both (Quality, SellIn) values for every item
        [TestMethod]
        public void TestThatSellInDaysIsLoweredWhenItemNameIsNotSulfuras()
        {
            var item = UpdateQualityReturnItem(new List<Item> { CreateItem1() }, "Item1");
            Assert.AreEqual(0, item.SellIn, "SellIn days was not lowered.");
        }

        // - "Sulfuras", being a legendary item, never has to be sold or decreases in Quality
        [TestMethod]
        public void TestThatSellInDaysIsNotLoweredForSulfuras()
        {
            var item = UpdateQualityReturnItem(new List<Item> { CreateSulfuras() }, SulfurasItemName);
            Assert.AreEqual(1, item.SellIn, string.Format("SellIn days was lowered for '{0}'.", SulfurasItemName));
        }

        // - At the end of each day our system lowers both (Quality, SellIn) values for every item
        [TestMethod]
        public void TestThatQualityIsLoweredForNormalCase()
        {
            var item = UpdateQualityReturnItem(new List<Item> { CreateItem1() }, "Item1");
            Assert.AreEqual(0, item.Quality, "Quality was not lowered for normal case.");
        }

        // - "Sulfuras", being a legendary item, never has to be sold or decreases in Quality
        [TestMethod]
        public void TestThatQualityIsNotLoweredForSulfuras()
        {
            var item = UpdateQualityReturnItem(new List<Item> { CreateSulfuras() }, SulfurasItemName);
            Assert.AreEqual(1, item.Quality, string.Format("Quality days was lowered for '{0}'.", SulfurasItemName));
        }

        // - The Quality of an item is never negative
        [TestMethod]
        public void TestThatQualityIsNeverNegative()
        {
            var item = UpdateQualityReturnItem(new List<Item> { CreateItem2() }, "Item2");
            Assert.AreEqual(0, item.Quality, "Quality is negative.");
        }

        // - Once the sell by date has passed, Quality degrades twice as fast
        [TestMethod]
        public void TestThatQualityIsLoweredAtTwiceTheRateWhenSellByDateIsPassed()
        {
            var item = CreateItem1();
            item.SellIn = 0;
            item.Quality = 2;
            item = UpdateQualityReturnItem(new List<Item> { item }, "Item1");
            Assert.AreEqual(0, item.Quality, "Quality did not reduce at twice rate for an item past its sell by date.");
        }

        // - At the end of each day our system lowers both (Quality, SellIn) values for every item
        // - "Conjured" items degrade in Quality twice as fast as normal items
        [TestMethod]
        public void TestThatQualityIsLoweredBy2ForConjuredItem()
        {
            var item = UpdateQualityReturnItem(new List<Item> { CreateConjuredItem() }, ConjuredManaCakeItemName);
            Assert.AreEqual(4, item.Quality, "Quality was not lowered by 2 for conjured item.");
        }

        // - "Aged Brie" actually increases in Quality the older it gets
        [TestMethod]
        public void TestThatAgedBrieIncreasesInQuality()
        {
            var item = UpdateQualityReturnItem(new List<Item> { CreateBrie() }, BrieItemName);
            Assert.AreEqual(2, item.Quality, "Quality was not increased for Brie.");
        }

        // - Once the sell by date has passed, Quality degrades twice as fast
        // - "Aged Brie" actually increases in Quality the older it gets
        // - Combining both "Aged Brie" past the sell by date increases in value twice as fast.
        [TestMethod]
        public void TestThatAgedBrieIncreasesInQualityAtTwiceTheRateWhenSellByDateIsPassed()
        {
            var item = CreateBrie();
            item.SellIn = 0;
            item = UpdateQualityReturnItem(new List<Item> { item }, BrieItemName);
            Assert.AreEqual(3, item.Quality, "Quality did not increase at twice rate for an 'Aged Brie' past its sell by date.");
        }

        // - The Quality of an item is never more than 50
        [TestMethod]
        public void TestThatQualityOfAnItemIsNeverMoreThan50()
        {
            var item = CreateBrie();
            item.Quality = 50;
            item = UpdateQualityReturnItem(new List<Item> { item }, BrieItemName);
            Assert.AreEqual(50, item.Quality, "Quality of an item increased beyond 50.");
        }

        // "Backstage passes", like aged brie, increases in Quality as it's SellIn value approaches; 
        [TestMethod]
        public void TestThatQualityOfABackStagePassItemIncreasesBy1WhenSellInDaysIsMoreThan10()
        {
            var item = CreateBackStagePass();
            item = UpdateQualityReturnItem(new List<Item> { item }, BackStagePassItemName);
            Assert.AreEqual(2, item.Quality, "Quality of a backstage pass did not increase by 1 when sell in days is more than 10.");
        }

        // "Backstage passes", like aged brie, increases in Quality as it's SellIn value approaches; 
        // Quality increases by 2 when there are *10 days* or less
        [TestMethod]
        public void TestThatQualityOfABackStagePassItemIncreasesBy2WhenSellInDays10()
        {
            var item = CreateBackStagePass();
            item.SellIn = 10;
            item = UpdateQualityReturnItem(new List<Item> { item }, BackStagePassItemName);
            Assert.AreEqual(3, item.Quality, "Quality of a backstage pass did not increase by 2 when sell in days is 10.");
        }

        // "Backstage passes", like aged brie, increases in Quality as it's SellIn value approaches; 
        // Quality increases by 2 when there are 10 days or *less*
        [TestMethod]
        public void TestThatQualityOfABackStagePassItemIncreasesBy2WhenSellInDaysIsLessThan10()
        {
            var item = CreateBackStagePass();
            item.SellIn = 9;
            item = UpdateQualityReturnItem(new List<Item> { item }, BackStagePassItemName);
            Assert.AreEqual(3, item.Quality, "Quality of a backstage pass did not increase by 2 when sell in days is less than 10.");
        }

        // "Backstage passes", like aged brie, increases in Quality as it's SellIn value approaches; 
        // and by 3 when there are *5 days* or less 
        [TestMethod]
        public void TestThatQualityOfABackStagePassItemIncreasesBy3WhenSellInDaysIs5()
        {
            var item = CreateBackStagePass();
            item.SellIn = 5;
            item = UpdateQualityReturnItem(new List<Item> { item }, BackStagePassItemName);
            Assert.AreEqual(4, item.Quality, "Quality of a backstage pass did not increase by 3 when sell in days is 5.");
        }

        // "Backstage passes", like aged brie, increases in Quality as it's SellIn value approaches; 
        // and by 3 when there are 5 days or *less*
        [TestMethod]
        public void TestThatQualityOfABackStagePassItemIncreasesBy3WhenSellInDaysIsLessThan5()
        {
            var item = CreateBackStagePass();
            item.SellIn = 4;
            item = UpdateQualityReturnItem(new List<Item> { item }, BackStagePassItemName);
            Assert.AreEqual(4, item.Quality, "Quality of a backstage pass did not increase by 3 when sell in days is less than 5.");
        }

        // "Backstage passes", like aged brie, increases in Quality as it's SellIn value approaches; 
        // but Quality drops to 0 after the concert
        [TestMethod]
        public void TestThatQualityOfABackStagePassItemDropsTo0AfterTheConcert()
        {
            var item = CreateBackStagePass();
            item.SellIn = 0;
            item = UpdateQualityReturnItem(new List<Item> { item }, BackStagePassItemName);
            Assert.AreEqual(0, item.Quality, "Quality of a backstage pass did not drop to 0 after the concert.");
        }

        // "Backstage passes", like aged brie, increases in Quality as it's SellIn value approaches; 
        // Corner case where quality is 47 all the 3 if conditions with asterisks are required even though the if condition is same
        //if (Items[i].Quality < 50) ************************************
        //{
        //    Items[i].Quality = Items[i].Quality + 1;

        //    if (Items[i].Name == "Backstage passes to a TAFKAL80ETC concert")
        //    {
        //        if (Items[i].SellIn < 11)
        //        {
        //            if (Items[i].Quality < 50)************************************
        //            {
        //                Items[i].Quality = Items[i].Quality + 1;
        //            }
        //        }

        //        if (Items[i].SellIn < 6)
        //        {
        //            if (Items[i].Quality < 50)************************************
        //            {
        //                Items[i].Quality = Items[i].Quality + 1;
        //            }
        //        }
        //    }
        //}
        [TestMethod]
        public void TestThatQualityOfABackStagePassItemIncreasesBy3WhenSellInDaysIsLessThan5CornerCasesWhenQualityIs47()
        {
            var item = CreateBackStagePass();
            item.SellIn = 4;
            item.Quality = 47;
            item = UpdateQualityReturnItem(new List<Item> { item }, BackStagePassItemName);
            Assert.AreEqual(50, item.Quality, "Quality of a backstage pass did not increase by 3 when sell in days is less than 5.");
        }
    }
}
