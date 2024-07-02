namespace System.Utils
{
    public struct JsonData
    {
        public static readonly char[][] RoundBrackets = { new char[] { '(', ')' } };
        public static readonly char[][] SquareBrackets = { new char[] { '[', ']' } };
        public static readonly char[][] CurlyBrackets = { new char[] { '{', '}' } };
        public static readonly char[][] SpeechMarks = { new char[] { '\"', '\"' } };

        public static readonly char[][] all = JsonTools.Concat(new char[][][] { RoundBrackets, SquareBrackets, CurlyBrackets, SpeechMarks });

        public static int CoreCount = Environment.ProcessorCount;
    }
    public static class JsonTools
    {
        /// <summary>
        /// Takes an array of arrays and returns one singular array made up of all the sub-arrays appended together
        /// </summary>
        /// <typeparam name="T">the type of array (can be inferred in almost all cases)</typeparam>
        /// <param name="arrays">the array of arrays</param>
        /// <returns>all input arrays appended into one</returns>
        public static T[] Concat<T>(this T[][] arrays)
        {
            int total = 0;
            int index = 0;
            foreach (T[] array in arrays)
            {
                total += array.Length;
            }
            T[] result = new T[total];
            for (int i = 0; i < arrays.Length; i++)
            {
                for (int j = 0; j < arrays[i].Length; j++)
                {
                    result[index] = arrays[i][j];
                    index++;
                }
            }
            return result;
        }
        /// <summary>
        /// Takes a list of arrays and returns one singular array made up of all the sub-arrays appended together
        /// </summary>
        /// <typeparam name="T">the type of array (can be inferred in almost all cases)</typeparam>
        /// <param name="arrays">the array of arrays</param>
        /// <returns>all input arrays appended into one</returns>
        public static T[] Concat<T>(this List<T[]> arrays)
        {
            int total = 0;
            int index = 0;
            foreach (T[] array in arrays)
            {
                total += array.Length;
            }
            T[] result = new T[total];
            for (int i = 0; i < arrays.Count; i++)
            {
                for (int j = 0; j < arrays[i].Length; j++)
                {
                    result[index] = arrays[i][j];
                    index++;
                }
            }
            return result;
        }
        public static string Concat(this string[] arrays, string seperator = "")
        {
            int total = 0;
            foreach (string array in arrays)
            {
                total += array.Length;
            }
            string result = "";
            for (int i = 0; i < arrays.Length - 1; i++)
            {
                result += arrays[i] + seperator;
            }
            result += arrays.Last();
            return result;
        }

        /// <summary>
        /// checks to see if two byte arrays are equal
        /// </summary>
        /// <param name="A">byte array A</param>
        /// <param name="B">byte array B</param>
        /// <returns>true if the two arrays are identical, otherwise false</returns>
        public static bool CompareBytes(byte[] A, byte[] B)
        {
            if (A.Length != B.Length) return false;
            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] != B[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// returns the index of <paramref name="target"/> within <paramref name="item"/> starting at <paramref name="startIndex"/>, however the function will not search between characters defined within <paramref name="pairs"/>
        /// </summary>
        /// <param name="item">the string to search through</param>
        /// <param name="target">the string to search for</param>
        /// <param name="pairs">an array of length 2 character arrays. When the funcion hits a character at [0] in an array, it will not continue the search until it meets the corresponding character in the pairs [1] position </param>
        /// <param name="startIndex">the index to begin the search from</param>
        /// <returns>the index of <paramref name="target"/> within <paramref name="item"/>, or -1 if not found</returns>
        public static int IndexOfExclusive(this string item, string target, char[][] pairs, int startIndex = 0)
        {
            int skipLayers = 0;
            List<char> currentChars = new();

            bool ranToEnd = false;
            int offset = startIndex;
            while (!ranToEnd)
            {
                int nextTarget = item.IndexOf(target, offset);
                if (nextTarget == -1) return -1;
                for (int i = 0; i < pairs.Length; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        int index = item.IndexOf(pairs[i][j], offset);
                        if (index < nextTarget && index != -1)
                        {
                            nextTarget = index;
                        }
                    }
                }


                bool checkAll(char tar)
                {
                    foreach (char[] arr in pairs)
                    {
                        if (arr[0] == tar)
                        {
                            currentChars.Add(arr[1]);
                            return true;
                        }
                    }
                    return false;
                }

                if (currentChars.Count > 0 && currentChars[^1] == item[nextTarget])
                {
                    if (skipLayers > 0)
                    {
                        skipLayers -= 1;
                        currentChars.RemoveAt(currentChars.Count - 1);
                    }
                    else
                    {
                        if (checkAll(item[nextTarget]))
                        {
                            skipLayers += 1;
                        }
                        else
                        {
                            skipLayers = 0;
                        }
                    }
                }
                else if (checkAll(item[nextTarget]))
                {
                    skipLayers += 1;
                }

                if (skipLayers == 0)
                {
                    bool found = false;
                    for (int j = 0; j < target.Length; j++)
                    {
                        if (nextTarget + j < item.Length && target[j] == item[nextTarget + j])
                        {
                            found = true;
                        }
                        else
                        {
                            found = false;
                            break;
                        }
                    }
                        if (found) return nextTarget;
                }

                offset = nextTarget + 1;
            }
            return -1;
        }


        public static T[] GetClosest<T>(this T[] item, T[][] array)
        {
            float[] results = new float[array.Length];

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = item.SoftMatch(array[i]);
            }

            float max = 0;
            foreach (float f in results)
            {
                if (f > max)
                {
                    max = f;
                }
            }
            return array[Array.IndexOf(results, max)];
        }
        public static string GetClosest(this string item, string[] array)
        {
            var results = new float[array.Length];

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = item.SoftMatch(array[i]);
            }

            float max = 0;
            foreach (float f in results)
            {
                if (f > max)
                {
                    max = f;
                }
            }
            return array[Array.IndexOf(results, max)];
        }

        public static float SoftMatch<T>(this T[] item, T[] target)
        {
            int[] AccCounts = new int[target.Length]; 
            for (int i = 0; i < target.Length; i++)
            {
                for (int j = 0; j < target.Length; j++)
                {
                    if (j + i < item.Length)
                    {
                        if ((item[j + i] ?? throw new NullReferenceException()).Equals(target[j]))
                        {
                            AccCounts[i]++;
                        }
                        else
                        {
                            AccCounts[i]--;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            int AccCount = 0;
            foreach(int i in AccCounts)
            {
                if (i > AccCount) AccCount = i;
            }
            return (float)AccCount / (target.Length);
        }
        public static float SoftMatch(this string item, string target)
        {
            int[] AccCounts = new int[target.Length];
            for (int i = 0; i < target.Length; i++)
            {
                for (int j = 0; j < target.Length; j++)
                {
                    if (j + i < item.Length)
                    {
                        if (item[j + i].Equals(target[j]))
                        {
                            AccCounts[i]++;
                        }
                        else
                        {
                            AccCounts[i]--;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            int AccCount = 0;
            foreach (int i in AccCounts)
            {
                if (i > AccCount) AccCount = i;
            }
            return (float)AccCount / (target.Length);
        }



        /// <summary>
        /// returns the index of <paramref name="target"/> within <paramref name="item"/> starting at <paramref name="startIndex"/>, however the function will not search between characters defined within <paramref name="pairs"/>
        /// </summary>
        /// <param name="item">the string to search through</param>
        /// <param name="target">the character to search for</param>
        /// <param name="pairs">an array of length 2 character arrays. When the funcion hits a character at [0] in an array, it will not continue the search until it meets the corresponding character in the pairs [1] position </param>
        /// <param name="startIndex">the index to begin the search from</param>
        /// <returns>the index of <paramref name="target"/> within <paramref name="item"/>, or -1 if not found</returns>

        public static int IndexOfExclusive(this string item, char target, char[][] pairs, int startIndex = 0)
        {
            int skipLayers = 0;
            List<char> currentChars = new();

            bool ranToEnd = false;
            int offset = startIndex;
            while (!ranToEnd)
            {
                int nextTarget = item.IndexOf(target, offset);
                if (nextTarget == -1) return -1;
                for (int i = 0; i < pairs.Length; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        int index = item.IndexOf(pairs[i][j], offset);
                        if (index < nextTarget && index != -1)
                        {
                            nextTarget = index;
                        }
                    }
                }


                bool checkAll(char tar)
                {
                    foreach (char[] arr in pairs)
                    {
                        if (arr[0] == tar)
                        {
                            currentChars.Add(arr[1]);
                            return true;
                        }
                    }
                    return false;
                }

                if (currentChars.Count > 0 && currentChars[^1] == item[nextTarget])
                {
                    if (skipLayers > 0)
                    {
                        skipLayers -= 1;
                        currentChars.RemoveAt(currentChars.Count - 1);
                    }
                    else
                    {
                        if (checkAll(item[nextTarget]))
                        {
                            skipLayers += 1;
                        }
                        else
                        {
                            skipLayers = 0;
                        }
                    }
                }
                else if (checkAll(item[nextTarget]))
                {
                    skipLayers += 1;
                }

                if (skipLayers == 0)
                {
                    bool found = (target == item[nextTarget]);
                    if (found) return nextTarget;
                }

                offset = nextTarget + 1;
            }

            return -1;
        }

        /// <summary>
        /// Removes the item at <paramref name="index"/> from <paramref name="array"/> and returns the new array
        /// </summary>
        /// <typeparam name="T">the type of array (can be inferred in almost all cases)</typeparam>
        /// <param name="array">the primary array</param>
        /// <param name="index">the index to remove</param>
        /// <returns><paramref name="array"/> except with the item at <paramref name="index"/> removed</returns>
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            T[] tmp = new T[array.Length - 1];
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = array[i + (i >= index ? 1 : 0)];
            }
            return tmp;
        }

        /// <summary>
        /// Inserts the item <paramref name="item"/> into <paramref name="array"/> at index <paramref name="index"/> and returns the new array
        /// </summary>
        /// <typeparam name="T">the type of array (can be inferred in almost all cases)</typeparam>
        /// <param name="array">the primary array</param>
        /// <param name="index">the index at which to insert the new item into</param>
        /// <param name="item">the item to insert</param>
        /// <returns><paramref name="array"/> except with <paramref name="item"/> inserted into <paramref name="index"/></returns>
        public static T[] InsertAt<T>(this T[] array, int index, T item)
        {
            T[] tmp = new T[array.Length + 1];
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = array[i - (i >= index ? 1 : 0)];
            }
            tmp[index] = item;
            return tmp;
        }

        /// <summary>
        /// Takes an array of characters and returns ite as a string
        /// </summary>
        /// <param name="chars">the character array to convert</param>
        /// <returns>the string representation of the character array</returns>
        public static string CharArrToString(this char[] chars)
        {
            string Out = "";
            for (int i = 0; i < chars.Length; i++)
            {
                Out += chars[i];
            }
            return Out;
        }

        /// <summary>
        /// returns the string <paramref name="item"/> with any leading or trailing instances of <paramref name="target"/> removed
        /// </summary>
        /// <param name="item">string to trim</param>
        /// <param name="target">character to trim off of <paramref name="item"/></param>
        /// <returns>the string <paramref name="item"/> with leading or trailing instances of <paramref name="target"/> trimmed</returns>
        public static string Trim(this string item, char target)
        {
            string Out = item;
            for (int i = 0; i < Out.Length; i++)
            {
                if (Out[0] == target)
                {
                    Out = CharArrToString(RemoveAt(Out.ToCharArray(), 0));
                }
                else
                {
                    break;
                }
            }
            for (int i = Out.Length - 1; i > 0; i -= 2)
            {
                if (Out[i] == target)
                {
                    Out = CharArrToString(RemoveAt(Out.ToCharArray(), i));
                }
                else
                {
                    break;
                }
            }
            return Out;
        }

        /// <summary>
        /// replaces any instances of <paramref name="oldStr"/> within <paramref name="masterStr"/> with <paramref name="newStr"/>, if the boolean returned by <paramref name="comparison"/> is true
        /// if <paramref name="comparison"/> is not supplied, the replacement will proceed whenever <paramref name="oldStr"/> is found
        /// </summary>
        /// <param name="masterStr">the parent string to search through</param>
        /// <param name="oldStr">the string to search for in <paramref name="masterStr"/></param>
        /// <param name="newStr">the string to repace instances of <paramref name="oldStr"/> within <paramref name="masterStr"/> with</param>
        /// <param name="comparison">an optional boolean function taking a single parameter of index, which will be run before making a replacement</param>
        /// <returns>the string <paramref name="masterStr"/> except with any instances of <paramref name="oldStr"/> replaced with <paramref name="newStr"/> where <paramref name="comparison"/> returned true (or was left null)</returns>
        public static string DynReplace(this string masterStr, string oldStr, string newStr, Func<int, bool>? comparison = null)
        {
            char[] charVer = masterStr.ToCharArray();
            for (int i = charVer.Length - 1; i >= 0; i--)
            {
                bool found = false;
                for (int j = oldStr.Length - 1; j >= 0; j--)
                {
                    if (i + j < charVer.Length && oldStr[j] == charVer[i + j])
                    {
                        found = true;
                    }
                    else
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    //enact the replacement
                    if (comparison != null)
                    {
                        if (comparison(i))
                        {
                            for (int j = 0; j < oldStr.Length; j++)
                            {
                                charVer = charVer.RemoveAt(i + j);
                            }
                            for (int j = 0; j < newStr.Length; j++)
                            {
                                charVer = charVer.InsertAt(i + j, newStr[j]);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < oldStr.Length; j++)
                        {
                            charVer = charVer.RemoveAt(i + j);
                        }
                        for (int j = 0; j < newStr.Length; j++)
                        {
                            charVer = charVer.InsertAt(i + j, newStr[j]);
                        }
                    }
                }
            }
            return CharArrToString(charVer);
        }

        /// <summary>
        /// returns a rudimentary json from an input array <paramref name="array"/>, containing an item <c>"data"</c> which itself then contained the json array representation of <paramref name="array"/>
        /// </summary>
        /// <param name="array">the array to insert as data</param>
        /// <returns>a json with one item <c>"data"</c> containing the json representaion of <paramref name="array"/></returns>
        public static string MakeJson(this string[] array)
        {
            string start = "{\"data\":[";
            string end = "]}";

            if (array.Length > 0)
            {
                string mid = $"\"{array[0]}\"";

                for (int i = 1; i < array.Length; i++)
                {
                    mid += $",\"{array[i]}\"";
                }

                return start + mid + end;
            }
            else
            {
                return "NULL";
            }
        }

        /// <summary>
        /// returns the json array representation of <paramref name="array"/>
        /// </summary>
        /// <param name="array">the array to convert</param>
        /// <returns>json respresentaion of <paramref name="array"/></returns>
        public static string MakeJsonArray(this string[] array)
        {
            string start = "[";
            string end = "]";

            if (array.Length > 0)
            {
                string mid = $"\"{array[0]}\"";

                for (int i = 1; i < array.Length; i++)
                {
                    mid += $",\"{array[i]}\"";
                }

                return start + mid + end;
            }
            else
            {
                return "NULL";
            }
        }

        /// <summary>
        /// adds <paramref name="item"/> onto <paramref name="list"/> if <paramref name="list"/> does not already contain <paramref name="item"/>
        /// </summary>
        /// <typeparam name="T">the type of array (can be inferred in almost all cases)</typeparam>
        /// <param name="list">the list to add <paramref name="item"/> to</param>
        /// <param name="item">the item to add onto <paramref name="list"/></param>
        /// <returns><paramref name="list"/> but with item <paramref name="item"/> added if it was not already present</returns>
        public static List<T> Add_norepeat<T>(this List<T> list, T item, out bool success)
        {
            if (list.IndexOf(item) == -1)
            {
                list.Add(item);
                success = true;
            }
            else
            {
                success = false;
            }
            return list;
        }

        public static List<T> Add_norepeat<T>(this List<T> list, T item)
        {
            if (list.IndexOf(item) == -1)
            {
                list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// see <c>List<typeparamref name="T"/> Add_norepeat</c>, this just works with arrays as opposed to lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T[] Add_norepeat<T>(this T[] list, T item, out bool success)
        {
            if (!list.Contains(item))
            {
                T[] tmp = new T[list.Length + 1];
                for (int i = 0; i < list.Length; i++)
                {
                    tmp[i] = list[i];
                }
                tmp[^1] = item;
                success = true;
                return tmp;
            }
            else
            {
                success = false;
            }
            return list;
        }

        public static T[] Add_norepeat<T>(this T[] list, T item)
        {
            if (!list.Contains(item))
            {
                T[] tmp = new T[list.Length + 1];
                for (int i = 0; i < list.Length; i++)
                {
                    tmp[i] = list[i];
                }
                tmp[^1] = item;
            }
            return list;
        }

        /// <summary>
        /// takes a json array as a string and parses it into a string array
        /// </summary>
        /// <param name="data">the string to parse</param>
        /// <returns>a string array representing the input json array</returns>
        public static string[] ParseArray(this string data)
        {
            data = data.Replace("\"", "");
            Func<int, bool> comparer = (int index) =>
            {
                if (data[index - 1] == ',' || data[index + 1] == ',')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };
            data = DynReplace(data, " ", "", comparer);//only removes whitespace characters if they are next to a comma (works as a whitespace trim before the array has actually been seperated)
            data = data[1..^1];
            int count = 1;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == ',')
                {
                    count++;
                }
            }
            string[] Out = new string[count];
            string tmp = "";
            int j = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == ',')
                {
                    Out[j] = tmp;
                    j++;
                    tmp = "";
                }
                else
                {
                    tmp += data[i];
                }
            }
            Out[count - 1] = tmp;
            for (int i = 0; i < Out.Length; i++)
            {
                Out[i] = Out[i].Trim(' ');
            }
            return Out;
        }

        /// <summary>
        /// takes a json array as a string and parses it into a string array, ignoring characters between pairs defined
        /// </summary>
        /// <param name="data">the string to parse</param>
        /// <param name="pairs">pairs to not search within</param>
        /// <param name="removeSpeechMarks">whether speech marks should be removed or not</param>
        /// <returns></returns>
        public static string[] ParseArray(this string data, char[][] pairs, bool removeSpeechMarks = true)
        {
            data = data.Trim();
            if (data[0] == '[') data = data[1..];
            if (data[^1] == ']') data = data[..^1];
            string[] Out = data.SplitExclusive(',', pairs);

            for (int i = 0; i < Out.Length; i++)
            {
                Out[i] = Out[i].Trim();
                if (removeSpeechMarks)
                {
                    Out[i] = Out[i].Trim('"');
                }
            }
            return Out;
        }

        /// <summary>
        /// Works like a regular split function, however will not operate between defined pairs
        /// </summary>
        /// <param name="data">the string to split into an array</param>
        /// <param name="splitOn">the character to use as a delimeter</param>
        /// <param name="pairs">pairs to not operate within</param>
        /// <returns></returns>
        public static string[] SplitExclusive(this string data, char splitOn, char[][] pairs)
        {
            List<string> total = new();

            int NOI = -1;
            int LNOI = 0;

            while(true)
            {
                NOI = data.IndexOfExclusive(splitOn, pairs, NOI + 1);
                if (NOI != -1)
                {
                    total.Add(data[LNOI..NOI]);
                    LNOI = NOI + 1;
                }
                else
                {
                    total.Add(data[LNOI..]);
                    break;
                }
            }
            

            return total.ToArray();
        }
    }

    public class JSON
    {
        public class JSON_element
        {
            public object? Value;

            /// <summary>
            /// implementation of a class ToString() method
            /// </summary>
            /// <returns>The JSON text representation of the layer this is called upon and every child layer in a single line string with no whitespaces</returns>
            public override string ToString()
            {
                string rendered = "";
                void drawLayer(JSON_element layer, int stack = 0)
                {
                    var myType = layer.GetType();
                    if (myType == typeof(JSON_data))
                    {
                        if (stack == 0)
                        {
                            rendered += $"{layer.Value}";
                        }
                        else
                        {
                            if  (double.TryParse((layer.Value ?? throw new NullReferenceException()).ToString(), out _))
                            {
                                rendered += $"{layer.Value}".ReplaceLineEndings(@"\n");
                            }
                            else
                            {
                                rendered += $"\"{layer.Value}\"".ReplaceLineEndings(@"\n");
                            }
                        }
                    }
                    else if (myType == typeof(JSON_layer.Dict))
                    {
                        var keys = layer.Keys;
                        rendered += $"{{";
                        for (int i = 0; i < keys.Length - 1; i++)
                        {
                            rendered += $"\"{keys[i]}\":";
                            drawLayer(layer[keys[i]], stack + 1);
                            rendered += ",";
                        }
                        rendered += $"\"{keys[^1]}\":";
                        drawLayer(layer[keys[^1]], stack + 1);
                        rendered += $"}}";
                    }
                    else if (myType == typeof(JSON_layer.Array))
                    {
                        int max = layer.Keys.Length - 1;
                        rendered += $"[";
                        for (int i = 0; i < max; i++)
                        {
                            drawLayer(layer[i], stack + 1);
                            rendered += ",";
                        }
                        drawLayer(layer[max], stack + 1);
                        rendered += $"]";
                    }
                }
                drawLayer(this);
                return rendered;
            }

            /// <summary>
            /// alternative ToString() method
            /// </summary>
            /// <returns>The JSON text representation of the layer this is called upon and every child layer in a single line string with standard JSON formatting added</returns>
            public string ToFormattedString()
            {
                string rendered = "";
                void drawLayer(JSON_element layer, int stack = 0)
                {
                    string indent = "";
                    for (int i = 0; i < stack; i++)
                    {
                        indent += "  ";
                    }
                    var myType = layer.GetType();
                    if (myType == typeof(JSON_data))
                    {
                        if (stack == 0)
                        {
                            rendered += $"{layer.Value}";
                        }
                        else
                        {
                            if (double.TryParse((layer.Value ?? throw new NullReferenceException()).ToString(), out _))
                            {
                                rendered += $"{layer.Value}".ReplaceLineEndings(@"\n");
                            }
                            else
                            {
                                rendered += $"\"{layer.Value}\"".ReplaceLineEndings(@"\n");
                            }
                        }
                    }
                    else if (myType == typeof(JSON_layer.Dict))
                    {
                        var keys = layer.Keys;
                        rendered += $"{{\n";
                        for (int i = 0; i < keys.Length - 1; i++)
                        {
                            rendered += $"{indent}  \"{keys[i]}\": ";
                            drawLayer(layer[keys[i]], stack + 1);
                            rendered += ",\n";
                        }
                        rendered += $"{indent}  \"{keys[^1]}\": ";
                        drawLayer(layer[keys[^1]], stack + 1);
                        rendered += $"\n{indent}}}";
                    }
                    else if (myType == typeof(JSON_layer.Array))
                    {
                        int max = layer.Keys.Length - 1;
                        rendered += $"[\n";
                        for (int i = 0; i < max; i++)
                        {
                            rendered += indent + "  ";
                            drawLayer(layer[i], stack + 1);
                            rendered += ",\n";
                        }
                        rendered += indent + "  ";
                        drawLayer(layer[max], stack + 1);
                        rendered += $"\n{indent}]";
                    }
                }
                drawLayer(this);
                return rendered;
            }
            public JSON_element this[int index]
            {
                get { return getter(index); }
                set { setter(index, value); }
            }
            public JSON_element this[string index]
            {
                get { return getter(index); }
                set { setter(index, value); }
            }

            public JSON_element this[object index]
            {
                get
                {
                    if (index.GetType() == typeof(string))
                    {
                        return getter((string)index);
                    }
                    else if (index.GetType() == typeof(int))
                    {
                        return getter((int)index);
                    }
                    else
                    {
                        throw new Exception("Invalid key type");
                    }
                }
            }

            public virtual object[] Keys
            {
                get
                {
                    if (GetType().BaseType == typeof(JSON_layer))
                    {
                        var myVal = (JSON_element[])(Value ?? throw new NullReferenceException());
                        var myKeys = new object[myVal.Length];
                        for (int i = 0; i < myVal.Length; i++)
                        {
                            try
                            {
                                myKeys[i] = ((JSON_pair)myVal[i]).Key;
                            }
                            catch (InvalidCastException)
                            {
                                myKeys[i] = i;
                            }
                        }
                        return myKeys;
                    }
                    throw new JSON_Errors.InvalidLayerType();
                }
            }

            private JSON_element getter(int index)
            {
                if (GetType().BaseType == typeof(JSON_layer))
                {
                    var myVal = (JSON_element[])(Value ?? throw new NullReferenceException());
                    return myVal[index];
                }
                throw new KeyNotFoundException();
            }
            private JSON_element getter(string index)
            {
                if (GetType().BaseType == typeof(JSON_layer))
                {
                    var myVal = (JSON_element[])(Value ?? throw new NullReferenceException());
                    foreach (var val in myVal)
                    {
                        if (index == ((JSON_pair)val).Key.ToString())
                        {
                            return (JSON_element)(val.Value ?? throw new NullReferenceException());
                        }
                    }
                    throw new KeyNotFoundException();
                }
                throw new Exception("Invalid json layer type");
            }

            private void setter(int index, JSON_element val)
            {
                ((JSON_element[])(Value ?? throw new NullReferenceException()))[index] = val;
            }
            private void setter(string index, JSON_element val)
            {
                var myVal = ((JSON_element[])(Value ?? throw new NullReferenceException()));
                for (int i = 0; i < myVal.Length; i++)
                {
                    if (index == ((JSON_pair)myVal[i]).Key.ToString())
                    {
                        myVal[i].Value = val;
                    }
                }
            }

            public void Add(JSON_element item)
            {
                var array = (JSON_element[])(Value ?? throw new NullReferenceException());
                var arrayOut = new JSON_element[array.Length + 1];
                for (int i = 0; i < array.Length; i++)
                {
                    arrayOut[i] = array[i];
                }
                arrayOut[^1] = item;
                Value = arrayOut;
            }

            public void Remove(JSON_element item)
            {
                var array = (JSON_element[])(Value ?? throw new NullReferenceException());
                if (array.Contains(item))
                {
                    var arrayOut = new JSON_element[array.Length - 1];
                    int index = 0;
                    for (int i = 0; i < array.Length; i++)
                    {                        
                        if (item == array[i])
                        {
                            continue;
                        }
                        else
                        {
                            arrayOut[index] = array[i];
                            index++;
                        }
                    }
                    Value = arrayOut;
                }
                else
                {
                    return;
                }
            }

            public override bool Equals(object? obj) => obj != null && obj.ToString() == (Value ?? throw new NullReferenceException()).ToString();

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public static bool operator ==(JSON_element? left, object? right)
            {
                return ((left ?? throw new NullReferenceException()).Value ?? throw new NullReferenceException()).ToString() == (right ?? throw new NullReferenceException()).ToString();
            }
            public static bool operator !=(JSON_element? left, object? right)
            {
                return !(left == right);
            }
            public static bool operator <(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return leftX < rightX;
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static bool operator >(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return leftX > rightX;
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static bool operator <=(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return leftX <= rightX;
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static bool operator >=(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return leftX >= rightX;
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }

            //operator overrides

            public static JSON_element operator +(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return new JSON_data(leftX + rightX, typeof(double));
                        }
                    }
                    else
                    {
                        return new JSON_data((left.Value ?? throw new NullReferenceException()).ToString() + (right.Value ?? throw new NullReferenceException()).ToString(), typeof(string));
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator +(JSON_element left, double right)
            {
                if ((left.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    if (leftD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX))
                        {
                            return new JSON_data(leftX + right, typeof(double));
                        }
                    }
                    else
                    {
                        return new JSON_data((left.Value ?? throw new NullReferenceException()).ToString() + right.ToString(), typeof(string));
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator ++(JSON_element left)
            {
                return left + 1;
            }

            public static JSON_element operator -(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return new JSON_data(leftX - rightX, typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator -(JSON_element left, double right)
            {
                if ((left.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    if (leftD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX))
                        {
                            return new JSON_data(leftX - right, typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator --(JSON_element left)
            {
                return left - 1;
            }

            public static JSON_element operator *(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return new JSON_data(leftX * rightX, typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator *(JSON_element left, double right)
            {
                if ((left.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    if (leftD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX))
                        {
                            return new JSON_data(leftX * right, typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator /(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return new JSON_data(leftX / rightX, typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator /(JSON_element left, double right)
            {
                if ((left.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    if (leftD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX))
                        {
                            return new JSON_data(leftX / right, typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator %(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return new JSON_data(leftX % rightX, typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator %(JSON_element left, double right)
            {
                if ((left.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    if (leftD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX))
                        {
                            return new JSON_data(leftX % right, typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator ^(JSON_element left, JSON_element right)
            {
                if ((left.GetType() == typeof(JSON_data)) && (right.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    var rightD = (JSON_data)right;
                    if (leftD.type == typeof(double) && rightD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX) && double.TryParse(right.ToString(), out double rightX))
                        {
                            return new JSON_data(Math.Pow(leftX, rightX), typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public static JSON_element operator ^(JSON_element left, double right)
            {
                if ((left.GetType() == typeof(JSON_data)))
                {
                    var leftD = (JSON_data)left;
                    if (leftD.type == typeof(double))
                    {
                        if (double.TryParse(left.ToString(), out double leftX))
                        {
                            return new JSON_data(Math.Pow(leftX, right), typeof(double));
                        }
                    }
                }
                throw new JSON_Errors.InvalidLayerType();
            }
            public bool ContainsKey(string key)
            {
                if (GetType().BaseType == typeof(JSON_layer))
                {
                    var myVal = (JSON_element[])(Value ?? throw new NullReferenceException());
                    foreach (var val in myVal)
                    {
                        if (((JSON_pair)val).Key == key)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                throw new JSON_Errors.InvalidLayerType();
            }
        }

        protected static class JSON_Errors
        {

            [Serializable]
            public class InvalidLayerType : Exception
            {
                public InvalidLayerType() { }
                public InvalidLayerType(string message) : base(message) { }
                public InvalidLayerType(string message, Exception inner) : base(message, inner) { }
                protected InvalidLayerType(
                  Runtime.Serialization.SerializationInfo info,
                  Runtime.Serialization.StreamingContext context) : base(info, context) { }
            };
        }

        public virtual object[] Keys
        {
            get
            {
                return Value.Keys;
            }
        }

        public bool ContainsKey(string key)
        {
            return Value.ContainsKey(key);
        }

        public void Add(JSON_element item)
        {
            Value.Add(item);
        }
        public void Remove(JSON_element item)
        {
            Value.Remove(item);
        }

        public class JSON_data : JSON_element
        {
            // a single data endpoint
            public Type type;
            public JSON_data(object val, Type? type = null)
            {
                if (type == null) this.type = typeof(string);
                else this.type = type;
                Value = val;
            }
        }

        public class JSON_layer : JSON_element
        {
            //this is a dictionary in function
            public class Dict : JSON_layer
            {
                public Dict(JSON_element[] objArr)
                {
                    Value = objArr;
                }
            }
            public class Array : JSON_layer
            {
                public Array(JSON_element[] objArr)
                {
                    Value = objArr;
                }
            }
        }


        public class JSON_pair : JSON_element
        {
            public string Key;
            public JSON_pair(string _key, JSON_element obj)
            {
                Value = obj;
                Key = _key;
            }
        }

        public string Draw()
        {
            string rendered = "";
            void drawLayer(JSON_element layer, int stack = 0)
            {
                string indent = "";
                for (int i = 0; i < stack; i++)
                {
                    indent += "||";
                }
                indent += "->";
                if (layer.GetType() == typeof(JSON_data))
                {
                    rendered += $"{indent} {layer.ToString().ReplaceLineEndings(@"\n")}\n";
                }
                else
                {
                    object[] keys = layer.Keys;
                    foreach (var key in keys)
                    {
                        var val = layer[key];
                        rendered += $"{indent} {key}\n";
                        drawLayer(val, stack + 1);
                    }
                }
            }
            drawLayer(Value, 0);
            return rendered;
        }

        public JSON_element Value;

        /// <summary>
        /// implementation of a class ToString() method
        /// </summary>
        /// <returns>The JSON text representation of the layer this is called upon and every child layer in a single line string with no whitespaces</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// alternative ToString() method
        /// </summary>
        /// <returns>The JSON text representation of the layer this is called upon and every child layer in a single line string with standard JSON formatting added</returns>
        public string ToFormattedString()
        {
            return Value.ToFormattedString();
        }
        public JSON(string text)
        {
            Value = parseJSON(text);
        }

        public JSON(JSON_element ime)
        {
            Value = ime;
        }

        public JSON_element this[int index]
        {
            get { return Value[index]; }
            set { Value[index] = value; }
        }

        public JSON_element this[string index]
        {
            get { return Value[index]; }
            set { Value[index] = value; }
        }

        public static JSON_element parseJSON(string text)
        {
            JSON_element? Out = null;
            Thread parser = new(() =>
            {
                Out = LocalParse(text);

                static JSON_element LocalParse(string text)
                {
                    //identify type (array, dict or data)
                    try
                    {
                        text = text.Trim();
                        if (text.Length >= 2 && text[0] == '{' && text[^1] == '}')
                        {
                            text = text[1..^1];
                        }

                        if (text.Length >= 2 && text[0] == '[')
                        {
                            //is an array
                            var myArr = JsonTools.ParseArray(text, JsonData.all, false);
                            var JSArr = new JSON_element[myArr.Length];
                            for (int i = 0; i < myArr.Length; i++)
                            {
                                JSArr[i] = LocalParse(myArr[i]);
                            }
                            return new JSON_layer.Array(JSArr);
                        }
                        else if (text.IndexOfExclusive(':', JsonData.all) > -1)
                        {
                            //is a dict
                            var pairs = text.SplitExclusive(',', JsonData.all);
                            var dict = new JSON_element[pairs.Length];
                            for (int i = 0; i < pairs.Length; i++)
                            {
                                int index = pairs[i].IndexOfExclusive(":", JsonData.all);
                                var pair = new string[2];
                                pair[0] = pairs[i][0..index].Trim().Trim('"');
                                pair[1] = pairs[i][(index + 1)..].Trim();
                                dict[i] = new JSON_pair(pair[0], LocalParse(pair[1]));
                            }
                            return new JSON_layer.Dict(dict);
                        }
                        else
                        {
                            //is data
                            text = text.Trim();
                            if (!(text.Length >= 2 && text[0] == '"' && text[^1] == '"'))
                            {
                                try
                                {
                                    return new JSON_data(double.Parse(text), typeof(double));
                                }
                                catch (Exception) { }
                            }
                            text = text.Trim('"');
                            int index = text.IndexOf('\\');
                            while (index > -1)
                            {
                                char c = text[index + 1];
                                text = text.Remove(index, 2);
                                text = text.Insert(index, "\n");
                                index = text.IndexOf('\\', index + 1);
                            }
                            return new JSON_data(@text, typeof(string));
                        }
                    }
                    catch (Exception e)
                    {                    
                        Console.WriteLine("Parsing error:\n\n" + e);
                        throw new Exception(e.ToString());
                    }
                }
            }, 1024*1024*64);
            parser.Start();
            parser.Join();
            return (Out ?? throw new NullReferenceException());
        }
    }
}