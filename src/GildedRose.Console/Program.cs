using System.Collections.Generic;
using System.Linq;

namespace GildedRose.Console
{
    public class Program
    {
        public IList<Item> Items;
        static void Main(string[] args)
        {
            System.Console.WriteLine("OMGHAI!");

            var app = new Program()
                          {
                              Items = new List<Item>
                                          {
                                              new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                                              new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                                              new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                                              new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                                              new Item
                                                  {
                                                      Name = "Backstage passes to a TAFKAL80ETC concert",
                                                      SellIn = 15,
                                                      Quality = 20
                                                  },
                                              new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
                                          }

                          };

            app.UpdateQuality();

            System.Console.ReadKey();

        }

        public class Item
        {
            public string Name { get; set; }

            public int SellIn { get; set; }

            public int Quality { get; set; }
        }

        public void UpdateQuality()
        {
            foreach (var item in Items)
            {
                IUpdate inventoryUpdateStrategy = new DecrementingUpdate(); // Set to default

                if (InventoryUpdateStrategyMap.ContainsKey(item.Name))
                    inventoryUpdateStrategy = InventoryUpdateStrategyMap[item.Name];

                inventoryUpdateStrategy.Update(item);
            }
        }

        // Used for unit tests
        public Item FindItemByName(string name)
        {
            if (Items != null)
            {
                return Items.FirstOrDefault(i => i.Name.Equals(name));
            }
            return null;
        }

        private static IDictionary<string, IUpdate> InventoryUpdateStrategyMap = new Dictionary<string, IUpdate>
        {
            { "Aged Brie", new IncrementingUpdate() },
            { "Sulfuras, Hand of Ragnaros", new LegendaryUpdate() },
            { "Backstage passes to a TAFKAL80ETC concert", new BackStagePassUpdate() },
            { "Conjured Mana Cake", new ConjuredUpdate() }
        };

        // Base interface that every item inherits from. Every item updates something
        public interface IUpdate
        {
            void Update(Item item);
        }

        // Abstract update interface which includes adjusting the sell and the quality. Some functions are virtual
        public abstract class DefaultUpdate : IUpdate
        {
            public virtual void Update(Item item)
            {
                AdjustSellInDays(item);
                AdjustQuality(item);
                if (item.SellIn < 0)
                    AdjustQuality(item);
            }

            protected virtual void AdjustSellInDays(Item item)
            {
                item.SellIn--;
            }

            protected virtual void AdjustQuality(Item item) { }
        }

        // Legendary items are never sold or decrease in value so make Update function redundent
        public class LegendaryUpdate : DefaultUpdate
        {
            public override void Update(Item item)
            {

            }
        }

        // Decrementing items simply lose 1 quality every update. Quality must stay above 0.
        public class DecrementingUpdate : DefaultUpdate
        {
            protected override void AdjustQuality(Item item)
            {
                DecrementQuality(item);
            }

            protected static void DecrementQuality(Item item)
            {
                if (item.Quality > 0)
                    item.Quality--;
            }
        }

        // Conjured items are decrementing items except they lose 2 quality every update
        public class ConjuredUpdate : DecrementingUpdate
        {
            protected override void AdjustQuality(Item item)
            {
                DecrementQuality(item);
                DecrementQuality(item);
            }
        }

        // Incrementing items 
        public class IncrementingUpdate : DefaultUpdate
        {
            protected override void AdjustQuality(Item item)
            {
                IncrementQuality(item);
            }

            private static void IncrementQuality(Item item)
            {
                if (item.Quality < 50)
                    item.Quality++;
            }
        }

        public class BackStagePassUpdate : IncrementingUpdate
        {
            public override void Update(Item item)
            {
                base.Update(item);
                if (item.SellIn < 11)
                {
                    AdjustQuality(item);
                }
                if (item.SellIn < 6)
                {
                    AdjustQuality(item);
                }
                if (item.SellIn < 0)
                {
                    item.Quality = 0;
                }
            }
        }
    }

}
