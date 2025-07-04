namespace Kadense.RPG.CardDeck;

public class DeckOfCards
{
    public DeckOfCards()
    {
        Cards = new List<string>();
    }
    public DeckOfCards(Random random, bool includeJokers = false)
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

        var cardsArray = cards.ToArray();
        random.Shuffle(cardsArray);
        Cards = cardsArray.ToList();
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
    public List<string> Cards { get; set; }

    public string[] DrawCards(int count = 1)
    {
        var cardsTaken = Cards.Take(count).ToArray();
        Cards.RemoveAll(card => cardsTaken.Contains(card));
        return cardsTaken;
    }

    public void Shuffle(Random random)
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