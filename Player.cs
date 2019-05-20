using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digitoyGamesProject
{
    public class Player
    {
        const int FAKE_OKEY_INDEX = 52;
        const int NUM_UNIQUE_TOKEN_INDICES = 53;
        const int NUM_UNIQUE_TOKEN_VALUES = 13;

        private List<Token> hand;



        public Player(int idx)
        {
            
        }


        public void ReceiveTokens(List<Token> tokensGiven)
        {
            List<Token> tokens = new List<Token>(tokensGiven);
            this.hand = tokens;
        }

        public int FindDistanceToFinish(Token gosterge, Token okey, bool verbose = false)
        {
            int numOkeys = 0;
            for (int i = 0; i < hand.Count; ++i)
            {
                if (okey == hand[i])
                {
                    numOkeys++;
                }
            }

            int numFakeOkeys = 0;
            for (int i = 0; i < hand.Count; ++i)
            {
                if (hand[i].GetColor() == TokenColor.NONE)
                {
                    numFakeOkeys++;
                }
            }
            // Degeri ayni renkleri farkli taslari oncelikli gruplama
            int bestDistanceByValue = FindDistanceByPrioritizingValueGroups(numOkeys, numFakeOkeys, okey);
            // Cifte gitmek
            //int bestDistanceByPairing = FindDistanceByPairing(okey);
            // Rengi ayni sirali taslari oncelikli gruplama
            int bestDistanceByColor = FindDistanceByPrioritizingColorGroups(numOkeys, numFakeOkeys, okey);

            return Math.Min(bestDistanceByValue, bestDistanceByColor);
        }


        private int FindDistanceByGroupingSameValues(List<Token> tokens, Token okey)
        {
            int distance = tokens.Count;
            int numOkeys = 0;
            int numFakeOkeys = 0;
            int okeyIndex = okey.GetIndex();
            int okeyValue = okey.GetValue();

            for (int i = 0; i < tokens.Count; ++i)
            {
                if (okey.GetIndex() == tokens[i].GetIndex())
                {
                    numOkeys++;
                }
                else if (FAKE_OKEY_INDEX == tokens[i].GetIndex())
                {
                    numFakeOkeys++;
                }
            }
            List<HashSet<TokenColor>> valueFrequencies = null;

            int num3Removed = 0, num4Removed = 0, num2Removed = 0, num1Removed = 0;
            for (int k = 0; k < 2; ++k)
            {
                // Loops below done twice because a player may have 6-8 of the same value at hand

                FindValueFrequenciesByColor(tokens, okey, out valueFrequencies, ref numFakeOkeys);
                num4Removed += RemoveValueGroupFromTokens(tokens, valueFrequencies, 4);
            }
            distance -= 4 * num4Removed;

            for (int k = 0; k < 2; ++k)
            {
                // Loops below done twice because a player may have 6-8 of the same value at hand
                valueFrequencies = null;
                FindValueFrequenciesByColor(tokens, okey, out valueFrequencies, ref numFakeOkeys);
                num3Removed += RemoveValueGroupFromTokens(tokens, valueFrequencies, 3);
            }
            distance -= 3 * num3Removed;


            if (numOkeys > 0)
            {
                for (int t = 1; t <= numOkeys; ++t)
                {
                    for (int k = 0; k < 2; ++k)
                    {
                        // Loops below done twice because a player may have 6-8 of the same value at hand
                        valueFrequencies = null;
                        FindValueFrequenciesByColor(tokens, okey, out valueFrequencies, ref numFakeOkeys);
                        /*
						 * TODO: The line below is causing the algorithm to be incorrect. Now, the first
						 * found group of 2 colors is deleted. However, it may differ which group of 2
						 * colors we delete (when we consequently try to group them as increasing series)
						 * Thus, this code should be refactored so that the groups of 2 colors to be deleted
						 * would be given as a parameter.
						 */
                        num2Removed += RemoveValueGroupFromTokens(tokens, valueFrequencies, 2, true);
                        if (num2Removed > 0)
                        {
                            break;
                        }
                    }
                }
                distance -= 3 * num2Removed; // 3, since we delete an okey with each of them
                numOkeys -= num2Removed;

                valueFrequencies = null;
                FindValueFrequenciesByColor(tokens, okey, out valueFrequencies, ref numFakeOkeys);
                if (numOkeys == 2)
                {
                    num1Removed += RemoveValueGroupFromTokens(tokens, valueFrequencies, 1, true);
                    if (num1Removed > 0)
                    {
                        distance -= 3;
                        numOkeys = 0;
                    }
                    else if (num3Removed > 0)
                    {
                        int okeysToUse = Math.Min(num3Removed, numOkeys);
                        distance -= okeysToUse;
                        numOkeys -= okeysToUse;
                    }
                }
                else if (numOkeys == 1)
                {
                    if (num3Removed > 0)
                    {
                        distance--;
                        numOkeys = 0;
                    }
                }
            }

            removefromTokensList(tokens, TokenColor.NONE, -1); // removes all fake okeys
            removefromTokensList(tokens, okey.GetColor(), okey.GetValue()); // removes all okeys
            for (int k = 0; k < numOkeys; ++k)
            {
                // Add unused okeys
                tokens.Add(new Token(okey.GetIndex()));
            }

            for (int k = 0; k < numFakeOkeys; ++k)
            {
                // Add unused fake okeys
                tokens.Add(new Token(FAKE_OKEY_INDEX));
            }


            return distance;
        }

        private int FindDistanceByGroupingSameColors(List<Token> tokens, Token okey)
        {
            /*
		 * TODO: Try to group all the given elements with same color into consecutive
		 * series. Utilize as many as the okey and fake okey tokens at hand as you can use.
		 * Return the number of elements that could not be integrated into a group of 3 or more.
		 */
            return -1;
        }

        private int FindDistanceByPrioritizingValueGroups(int numOkeys, int numFakeOkeys, Token okey)
        {
            int minDistance = -1;
            for (int i = 0; i <= numOkeys; ++i)
            {

                for (int j = 0; j <= numFakeOkeys; ++j)
                {
                    List<Token> tokens = new List<Token>(hand);
                    removefromTokensList(tokens, TokenColor.NONE, -1); // removes all fake okeys
                    removefromTokensList(tokens, okey.GetColor(), okey.GetValue()); // removes all okeys


                    for (int k = 0; k < i; ++k)
                    {
                        tokens.Add(new Token(okey.GetIndex()));
                    }

                    for (int k = 0; k < j; ++k)
                    {
                        tokens.Add(new Token(FAKE_OKEY_INDEX));
                    }

                    minDistance = FindDistanceByGroupingSameValues(tokens, okey); //first, try to group by same values

                    for (int k = 0; k < numOkeys - i; ++k)
                    {
                        // Add the rest of the okeys at hand
                        tokens.Add(new Token(okey.GetIndex()));
                    }

                    for (int k = 0; k < numFakeOkeys - j; ++k)
                    {
                        // Add the rest of the fake okeys at hand
                        tokens.Add(new Token(FAKE_OKEY_INDEX));
                    }

                    int distance = FindDistanceByGroupingSameColors(tokens, okey); // group the rest by color

                    if (minDistance == -1)
                    {
                        minDistance = distance;
                    }
                    else
                    {
                        minDistance = Math.Min(minDistance, distance);
                    }
                }
            }

            return minDistance;
        }


        private int FindDistanceByPrioritizingColorGroups(int numOkeys, int numFakeOkeys, Token okey)
        {
            int minDistance = -1;
            for (int i = 0; i <= numOkeys; ++i)
            {
                for (int j = 0; j <= numFakeOkeys; ++j)
                {
                    List<Token> tokens = new List<Token>(hand);
                    removefromTokensList(tokens, TokenColor.NONE, -1); // removes all fake okeys
                    removefromTokensList(tokens, okey.GetColor(), okey.GetValue()); // removes all okeys

                    for (int k = 0; k < i; ++k)
                    {
                        tokens.Add(new Token(okey.GetIndex()));
                    }

                    for (int k = 0; k < j; ++k)
                    {
                        tokens.Add(new Token(FAKE_OKEY_INDEX));
                    }

                    FindDistanceByGroupingSameColors(tokens, okey); //first, try to group by same colored

                    for (int k = 0; k < numOkeys - i; ++k)
                    {
                        // Add the rest of the okeys at hand
                        tokens.Add(new Token(okey.GetIndex()));
                    }

                    for (int k = 0; k < numFakeOkeys - j; ++k)
                    {
                        // Add the rest of the fake okeys at hand
                        tokens.Add(new Token(FAKE_OKEY_INDEX));
                    }

                    int distance = FindDistanceByGroupingSameValues(tokens, okey); //group the res by values

                    if (minDistance == -1)
                    {
                        minDistance = distance;
                    }
                    else
                    {
                        minDistance = Math.Min(minDistance, distance);
                    }
                }
            }

            return minDistance;
        }

        private int FindDistanceByPairing(Token okey)
        {
            List<int> tokenFrequencies = new List<int>();
            for (int i = 0; i < NUM_UNIQUE_TOKEN_INDICES; ++i)
            {
                tokenFrequencies[i] = 0;
            }

            for (int i = 0; i < hand.Count; ++i)
            {
                tokenFrequencies[hand[i].GetIndex()]++;
            }

            int numPairs = 0;
            int okeyIndex = okey.GetIndex();
            for (int i = 0; i < tokenFrequencies.Count; ++i)
            {
                if (tokenFrequencies[i] == 2 && i != okeyIndex)
                {
                    numPairs++;
                }
            }

            if (tokenFrequencies[okeyIndex] == 1)
            {
                int numUnpairedCards = hand.Count - numPairs * 2 - 1; // -1 is for the okey token at hand
                if (numUnpairedCards > 0)
                {
                    numPairs++;
                }
            }
            else if (tokenFrequencies[okeyIndex] == 2)
            {
                int numUnpairedCards = hand.Count - numPairs * 2 - 2; // -2 is for the okey tokens at hand
                numPairs += (numUnpairedCards > 0 ? 2 : 1);
            }

            return hand.Count - numPairs * 2;
        }

        private static void removefromTokensList(List<Token> tokens, TokenColor color, int value)
        {
            for (int i = tokens.Count - 1; i >= 0; --i)
            {
                if (tokens[i].GetColor() == color && tokens[i].GetValue() == value)
                {
                    tokens.RemoveAt(i);
                }
            }
        }

        private static void FindValueFrequenciesByColor(List<Token> tokens, Token okey,
                                 out List<HashSet<TokenColor>> valueFrequencies,
                                 ref int numFakeOkeys)
        {
            valueFrequencies = new List<HashSet<TokenColor>>();

            for (int i = 0; i < NUM_UNIQUE_TOKEN_VALUES + 1; ++i)
            {
                valueFrequencies.Add(new HashSet<TokenColor>());
            }

            for (int i = 0; i < tokens.Count; ++i)
            {
                if (okey.GetIndex() != tokens[i].GetIndex())
                {
                    if (FAKE_OKEY_INDEX == tokens[i].GetIndex())
                    {
                        if (numFakeOkeys > 0)
                        {
                            valueFrequencies[okey.GetValue()].Add(okey.GetColor());
                            numFakeOkeys--;
                        }
                    }
                    else
                    {
                        valueFrequencies[tokens[i].GetValue()].Add(tokens[i].GetColor());
                    }
                }
            }
        }

        private static int RemoveValueGroupFromTokens(List<Token> tokens,
                               List<HashSet<TokenColor>> valueFrequencies,
                               int groupSizeToRemove,
                               bool removeJustOne = false)
        {
            int numRemoved = 0;
            for (int i = 1; i < valueFrequencies.Count; ++i)
            {
                if (valueFrequencies[i].Count == groupSizeToRemove)
                {
                    valueFrequencies[i].Clear();

                    numRemoved++;
                    if (removeJustOne)
                    {
                        break;
                    }
                }
            }

            return numRemoved;
        }


    };


}
