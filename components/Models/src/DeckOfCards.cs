namespace Kadense.RPG.Models;

public class DeckOfCards
{
    public DeckOfCards()
    {
        Cards = new List<GameCard>();
        Name = "Custom Deck";
    }

    public DeckOfCards(KadenseRandomizer random, string name, bool includeJokers = false)
    {
        Cards = new List<GameCard>();
        Name = name;
        if (Name == "Standard Deck")
        {
            Cards = CreateStandardDeck(random, includeJokers);
        }
        else
        {
            var games = new GamesFactory().EndGames();
            var game = games.Where(g => g.Name!.ToLowerInvariant() == Name.ToLowerInvariant())
                .First();
            var customDeck = game.CustomDecks.First();
            var cardsArray = customDeck.Value.Invoke().ToArray();

            random.Shuffle(cardsArray);
            Cards = cardsArray!.ToList();
        }
    }

    public string Name { get; set; }

    public DeckOfCards(KadenseRandomizer random, bool includeJokers = false)
    {
        Name = "Standard";
        Cards = CreateStandardDeck(random, includeJokers);
    }

    public void AddCard(string cardName)
    {
        var card = new GameCard(cardName);
        Cards.Add(card);
    }

    public void AddCard(GameCard card)
    {
        Cards.Add(card);
    }

    public void AddCards(IEnumerable<string> cardNames)
    {
        foreach (var cardName in cardNames)
        {
            AddCard(cardName);
        }
    }

    public void AddCards(IEnumerable<GameCard> cards)
    {
        foreach (var card in cards)
        {
            AddCard(card);
        }
    }

    public List<GameCard> CreateStandardDeck(KadenseRandomizer random, bool includeJokers = false)
    {
        var cards = new List<string>();
        for (int i = 1; i <= 52; i++)
        {
            int suit = (i - 1) / 13 + 1; // 1 to 4
            int rank = (i - 1) % 13 + 1; // 1 to 13
            string card = $"{Rank(rank)} of {Suit(suit)}";
            cards.Add(card);
        }

        if (includeJokers)
        {
            cards.Add(":black_joker: Joker");
            cards.Add(":black_joker: Joker");
        }

        var cardsArray = cards.Select(c => new GameCard(c)).ToArray();
        random.Shuffle(cardsArray);
        return cardsArray.ToList();
    }



    public string Suit(int suit) => suit switch
    {
        1 => ":heart_suit: Hearts",
        2 => ":diamond_suit: Diamonds",
        3 => ":club_suit: Clubs",
        4 => ":spade_suit: Spades",
        _ => throw new ArgumentException($"Unknown suit type: {suit}"),
    };


    public string Rank(int rank) => rank switch
    {
        1 => "Ace",
        2 => "2",
        3 => "3",
        4 => "4",
        5 => "5",
        6 => "6",
        7 => "7",
        8 => "8",
        9 => "9",
        10 => "10",
        11 => "Jack",
        12 => "Queen",
        13 => "King",
        _ => throw new ArgumentException($"Unknown card rank: {rank}"),
    };
    public List<GameCard> Cards { get; set; }

    public List<GameCard> DrawnCards { get; set; } = new List<GameCard>();

    public GameCard[] DrawCards(int count = 1)
    {
        var cardsTaken = Cards.Take(count).ToArray();
        Cards.RemoveAll(card => cardsTaken.Contains(card));
        DrawnCards.AddRange(cardsTaken);
        return cardsTaken;
    }

    public void ResetDeck(KadenseRandomizer random)
    {
        Cards.AddRange(DrawnCards);
        DrawnCards.Clear();
        Shuffle(random);
    }

    public void Shuffle(KadenseRandomizer random)
    {
        var cardsArray = Cards.ToArray();
        random.Shuffle(cardsArray);
        Cards = cardsArray.ToList();
    }

    public int Count()
    {
        return Cards.Count;
    }
}