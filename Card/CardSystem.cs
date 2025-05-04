using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakingBot.Card
{

    public enum CardNumbers
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    public enum CardSuits
    {
        Clubs,
        Spades,
        Diamonds,
        Hearts
    }

    internal class CardSystem
    {

        public CardNumbers SelectedNumber { get; set; }

        public CardSuits SelectedSuit { get; set; }

        public DiscordColor EmbedColor { get; set; }

        public CardSystem()
        {
            Random rand = new();

            SelectedSuit = (CardSuits)rand.Next(0, 4);

            SelectedNumber = (CardNumbers)rand.Next(0,13);

            switch (SelectedSuit)
            {
                case CardSuits.Clubs:
                    EmbedColor = DiscordColor.DarkGray;
                    break;
                case CardSuits.Spades:
                    EmbedColor = DiscordColor.NotQuiteBlack;
                    break;
                case CardSuits.Diamonds:
                    EmbedColor = DiscordColor.Aquamarine;
                    break;
                case CardSuits.Hearts:
                    EmbedColor = DiscordColor.HotPink;
                    break;
            }
        }
    }
}
