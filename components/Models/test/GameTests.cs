namespace Kadense.RPG.Models.Tests {
    public class UnitTest1
    {
        [Fact]
        public void TestCharacterLinking()
        {
            var game = new GamesFactory().EndGames().Where(g => g.Name == "Desert Bluffs").First();

            Assert.NotNull(game);

            Assert.NotNull(game.CharacterLocations);
            Assert.NotEmpty(game.CharacterLocations);
            var link = game.CharacterLocations.First();
            Assert.NotNull(link);
            Assert.NotNull(link.CharacterId);
            Assert.NotEmpty(link.CharacterId);
            Assert.NotNull(link.LocationId);
            Assert.NotEmpty(link.LocationId);
        }

        [Fact]
        public void TestItemLinking()
        {
            var game = new GamesFactory().EndGames().Where(g => g.Name == "Desert Bluffs").First();

            Assert.NotNull(game);

            Assert.NotNull(game.ItemLinks);
            Assert.NotEmpty(game.ItemLinks);
            var link = game.ItemLinks.First();
            Assert.NotNull(link);
            Assert.NotNull(link.ItemId);
            Assert.NotEmpty(link.ItemId);
            Assert.NotNull(link.LinkedToId);
            Assert.NotEmpty(link.LinkedToId);
        }

        [Fact]
        public void TestItemLocationLinking()
        {
            var game = new GamesFactory().EndGames().Where(g => g.Name == "Desert Bluffs").First();

            Assert.NotNull(game);
            var items = game.GetItemsForLocation("american-spirit");
            Assert.NotNull(items);
            Assert.NotEmpty(items);
        }
    }
}