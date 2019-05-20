using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digitoyGamesProject
{
    static class MyExtensions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    };

    public class Program
    {
        const int NUM_UNIQUE_TOKEN_INDICES = 53;
        const int NUM_UNIQUE_TOKEN_VALUES = 13;
        const int NUM_UNIQUE_TOKEN_COLORS = 4;
        const int NUM_TOTAL_TOKENS = 106;
        const int NUM_PLAYERS = 4;
        const int FAKE_OKEY_INDEX = 52;
        const int SIZE_OF_HAND_NON_STARTERS_DEALT = 14;
        const int SIZE_OF_EXTRA_TOKENS_DEALT_TO_STARTER = 1;

        private static readonly Random getrandom = new Random();

        private static void GenerateTokens(ref List<Token> tokens)
        {
            for (int i = 0; i <= 52; ++i)
            {
                for (int k = 0; k < 2; ++k)
                {
                    tokens.Add(new Token(i));
                }
            }
        }
        

        private static Token PickGosterge(ref List<Token> tokens)
        {
            int gosterge;
            do
            {
                gosterge = getrandom.Next(tokens.Count - 1);
            } while (tokens[gosterge].GetColor() == TokenColor.NONE);

            return tokens[gosterge];
        }


        public static void Main()
        {
            List<Token> tokens = new List<Token>();

            // Create deck
            GenerateTokens(ref tokens);
            tokens.Shuffle();

            // Pick gosterge and okey
            Token gosterge = PickGosterge(ref tokens);

            for (int i = tokens.Count - 1; i >= 0; --i)
            {
                if (gosterge.GetValue() == tokens[i].GetValue() && gosterge.GetColor() == tokens[i].GetColor())
                {
                    tokens.RemoveAt(i);
                    break;
                }
            }


            Token okey = new Token(gosterge.GetValue() < 13 ? gosterge.GetIndex() + 1 : gosterge.GetIndex() / 13 * 13);

            List<Player> players = new List<Player>(NUM_PLAYERS);

            int handStart;
            int handEnd = 0;

            for (int i = 0; i < 4; ++i)
            {
                players.Add(new Player(i));
                handStart = i * SIZE_OF_HAND_NON_STARTERS_DEALT;
                handEnd = handStart + SIZE_OF_HAND_NON_STARTERS_DEALT;
                List<Token> hand = new List<Token>();
                for (int j = handStart; j < handEnd; j++)
                {
                    hand.Add(tokens[j]);
                }

                players[i].ReceiveTokens(hand);
            }

            int startingPlayerIndex = getrandom.Next(0, players.Count - 1);

            List<Token> extraTokens = new List<Token>();
            for (int i = handEnd; i < handEnd + SIZE_OF_EXTRA_TOKENS_DEALT_TO_STARTER; i++)
            {
                extraTokens.Add(tokens[i]);
            }

            players[startingPlayerIndex].ReceiveTokens(extraTokens);

            int minDistanceToFinish = players[0].FindDistanceToFinish(gosterge, okey, true);
            int probableWinnerIndex = 0;

            for (int i = 1; i < 4; ++i)
            {
                int distanceToFinish = players[i].FindDistanceToFinish(gosterge, okey, true);
                if (minDistanceToFinish > distanceToFinish)
                {
                    minDistanceToFinish = distanceToFinish;
                    probableWinnerIndex = i;
                }
            }

            Console.WriteLine("Probable winner is the player with index: " + probableWinnerIndex);
            int a = 5;
        }
    }
}
