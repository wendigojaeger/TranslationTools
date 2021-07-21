using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WendigoJaeger.TranslationTool
{
    public class TrieNode<TKey, TValue> where TKey : IComparable
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public List<TrieNode<TKey, TValue>> Children { get; } = new List<TrieNode<TKey, TValue>>();

        public bool IsTerminator { get; set; }

        public bool IsLeaf
        {
            get
            {
                return Children.Count == 0;
            }
        }

        public bool IsValid
        {
            get
            {
                return Children.Count == 0 && Value != null;
            }
        }

        public TrieNode<TKey, TValue> Find(TKey key)
        {
            foreach(var child in Children)
            {
                if (child.Key.CompareTo(key) == 0)
                {
                    return child;
                }
            }

            return null;
        }
    }

    public class ByteToStringTrie
    {
        public TrieNode<byte, string> Root { get; } = new TrieNode<byte, string>();

        public void Insert(byte[] key, string value)
        {
            TrieNode<byte, string> currentNode = Root;

            for (int i = 0; i < key.Length; ++i)
            {
                var nextNode = currentNode.Find(key[i]);
                if (nextNode != null)
                {
                    currentNode = nextNode;
                }
                else
                {
                    var newNode = new TrieNode<byte, string>();
                    newNode.Key = key[i];

                    currentNode.Children.Add(newNode);

                    currentNode = newNode;
                }
            }

            currentNode.Value = value;
        }

        public void InsertTerminator(byte terminator)
        {
            TrieNode<byte, string> currentNode = Root;

            var nextNode = currentNode.Find(terminator);
            if (nextNode != null)
            {
                currentNode = nextNode;
            }
            else
            {
                var newNode = new TrieNode<byte, string>();
                newNode.Key = terminator;

                currentNode.Children.Add(newNode);

                currentNode = newNode;
            }

            currentNode.IsTerminator = true;
        }
    }

    public class StringToByteTrie
    {
        public TrieNode<char, byte[]> Root { get; } = new TrieNode<char, byte[]>();

        public void Insert(string key, byte[] value)
        {
            TrieNode<char, byte[]> currentNode = Root;

            for (int i = 0; i < key.Length; ++i)
            {
                var nextNode = currentNode.Find(key[i]);
                if (nextNode != null)
                {
                    currentNode = nextNode;
                }
                else
                {
                    var newNode = new TrieNode<char, byte[]>();
                    newNode.Key = key[i];

                    currentNode.Children.Add(newNode);

                    currentNode = newNode;
                }
            }

            currentNode.Value = value;
        }
    }

    public class CorrespondenceTable
    {
        public ByteToStringTrie BytesToString = new ByteToStringTrie();
        public StringToByteTrie StringToBytes = new StringToByteTrie();

        public void Parse(Endian endianess, string path)
        {
            BytesToString = new ByteToStringTrie();
            StringToBytes = new StringToByteTrie();

            using (var tblFile = File.OpenRead(path))
            {
                using (var reader = new StreamReader(tblFile, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        int equalPosition = line.IndexOf('=');

                        string rawKey = line.Substring(0, equalPosition);
                        string rawValue = line.Substring(equalPosition + 1);

                        uint intKey = uint.Parse(rawKey, System.Globalization.NumberStyles.HexNumber);

                        uint tempKey = intKey;
                        int numberOfBytes = 0;
                        do
                        {
                            tempKey >>= 8;
                            ++numberOfBytes;
                        }
                        while (tempKey != 0);

                        byte[] key = BitConverter.GetBytes(intKey);
                        key = key[0..numberOfBytes];

                        if (key.Length > 1)
                        {
                            key = key.Reverse().ToArray();
                        }

                        BytesToString.Insert(key, rawValue);
                        StringToBytes.Insert(rawValue, key);
                    }
                }
            }
        }
    }
}
