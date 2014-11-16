using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignPattern
{
    class Trie
    {
        private static Trie instance;

        private Trie()
        {
        }

        public static Trie getTrieInstance()
        {
            if (instance == null)
            {
                instance = new Trie();
                return instance;
            }
            return instance;
        }

        private int root = -1;
        private int numPoint = 0;
        private string ret;
        class point
        {
            public int[] son;
            public point()
            {
                son = new int[128];
            }
        };
        List<point> Memory = new List<point>();
        private int CreateTrieNode()
        {
            int i;
            int p;
            point temp = new point();
            for (i = 20; i < 128; i++)
            {
                temp.son[i] = -1;
                temp.son[0] = 1;
            }
            Memory.Add(temp);
            p = numPoint++;
            return p;
        }

        private void GetSub(int p, string start)
        {
            string now = start;
            bool flag = false;
            for (int i = 20; i < 128; ++i)
            {
                if (Memory[p].son[i] != -1)
                {
                    flag = true;
                    now += Convert.ToChar(i).ToString();
                    GetSub(Memory[p].son[i], now);
                    now = now.Remove(now.Length - 1);

                }
            }
            if (!flag)
            {
                ret += now;
                ret += " ";
            }
        }

        private void Update(string word)
        {
            int i, k;
            int p = -1;
            if (root == -1) root = CreateTrieNode();
            p = root;
            i = 0;

            for (i = 0; i < word.Length; ++i)
            {
                k = word[i];
                if (k > 127) return;
                if (Memory[p].son[k] == -1) Memory[p].son[k] = CreateTrieNode();
                p = Memory[p].son[k];
            }
        }


        public void Bulid(string code)
        {
            string temp = "";
            for (int i = 0; i < code.Length; ++i)
            {
                if ((code[i] >= '0' && code[i] <= '9') || (code[i] >= 'a' && code[i] <= 'z') || (code[i] >= 'A' && code[i] <= 'Z'))
                    temp += code[i];
                else
                {
                    if (temp.Length != 0) Update(temp);
                    temp = "";
                }
            }
            if (temp.Length != 0) Update(temp);
        }

        public string Search(string word)
        {
            ret = "";
            if (word.Length == 0 || root == -1) return word;
            int p = root;
            int i, k;
            for (i = 0; i < word.Length; ++i)
            {
                k = word[i];
                if (k > 127) return "error";
                if (Memory[p].son[k] == -1) return word;
                p = Memory[p].son[k];
            }
            GetSub(p, word);
            return ret;
        }
    }
}
