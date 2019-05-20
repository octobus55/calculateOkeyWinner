using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digitoyGamesProject
{
    public enum TokenColor
    {
        YELLOW,
        BLUE,
        BLACK,
        RED,
        NONE
    };

    public class Token
    {
        private int index;
        private TokenColor color;
        private int val;

        public Token(int idx)
        {
            this.index = idx;
            if (idx < 13)
            {
                this.color = TokenColor.YELLOW;
                this.val = idx + 1;
            }
            else if (idx < 26)
            {
                this.color = TokenColor.BLUE;
                this.val = (idx % 12) + 1;
            }
            else if (idx < 39)
            {
                this.color = TokenColor.BLACK;
                this.val = (idx % 12) + 1;
            }
            else if (idx < 52)
            {
                this.color = TokenColor.RED;
                this.val = (idx % 12) + 1;
            }
            else
            {
                this.color = TokenColor.NONE;
            }
        }

        public TokenColor GetColor()
        {
            return color;
        }

        public int GetValue()
        {
            return val;
        }

        public int GetIndex()
        {
            return index;
        }

        //todo override equals
        /*bool operator==(const Token& rhs) const
         {
             return index == rhs.index;
         }*/
    };
}
