using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OCRForXJXQ
{
    using Newtonsoft.Json;
    using System.Text.RegularExpressions;

    public class OCRTable
    {
        public string GuessTableName;
        public int ColCount = -1;
        public int RowCount;
        public Dictionary<string, string> valueDict;
        static Regex regex_ch = new Regex(@"[\u4E00-\u9FA5]");
        /// <summary>
        /// 通过oTable对象解析表名和内容
        /// 表单元格解析为一个字典：键由表的序例号，行号，当前行的列号组成，三个元素间用"_"分隔
        /// </summary>
        /// <param name="oTable"></param>
        public OCRTable(OTable oTable)
        {
            valueDict = new Dictionary<string, string>();
            var tableName = new StringBuilder();
            Dictionary<int, int> colCountDict = new Dictionary<int, int>();
            var regionIdx = 0;
            int maxRow = int.MinValue;
            int tableIndex = 0;
            foreach (var region in oTable.result.regions)
            {
                regionIdx++;
                if (region.type == "text")
                {
                    foreach (var blk in region.Blocks)
                    {
                        var word = blk.Words.Replace("\n", "").Replace(" ", "");
                        if (regex_ch.IsMatch(word))
                            tableName.Append(word);
                    }
                }
                else if (region.type == "table")
                {
                    tableIndex++;
                    foreach (var blk in region.Blocks)
                    {
                        var word = blk.Words.Replace("\n", "").Replace(" ", "");
                        var curRow = 0;
                        var curCol = 0;
                        if (blk.Rows.Count > 0)
                            curRow = blk.Rows[0];
                        if (colCountDict.ContainsKey(curRow))
                            curCol = colCountDict[curRow];
                        foreach (var row in blk.Rows)
                        {
                            if (!colCountDict.ContainsKey(row))
                                colCountDict.Add(row, 0);
                            colCountDict[row]++;
                            if (maxRow < row)
                                maxRow = row;
                        }
                        valueDict.Add(string.Format("{0}_{1}_{2}", tableIndex, curRow, curCol), word);
                    }
                }
            }
            GuessTableName = tableName.ToString();
            RowCount = maxRow;
        }
        
        /// <summary>
        /// 使用adjustDict对内容进行校正
        /// </summary>
        /// <param name="adjustDict"></param>
        public void AdjustStringByDict(Dictionary<string,string> adjustDict)
        {
            foreach(string key in valueDict.Keys)
            {
                var value = valueDict[key];
                foreach(var adKey in adjustDict.Keys)
                    value.Replace(adKey, adjustDict[adKey]);
                valueDict[key] = value;
            }
        }

        /// <summary>
        /// 该函数被覆写
        /// 形成一个多行文本，第一行为表名，之后为键-值对，以":"分隔
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GuessTableName);
            sb.Append(System.Environment.NewLine);
            foreach (var key in valueDict.Keys)
                sb.Append(string.Format("{0}:{1}{2}", key, valueDict[key], System.Environment.NewLine));
            return sb.ToString();
        }
    }

    public class OTable
    {
        [JsonProperty("result")]
        public Result result;
    }

    [JsonObject("result")]
    public class Result
    {
        [JsonProperty("words_region_count")]
        public int regionCunt;
        [JsonProperty("words_region_list")]
        public List<Region> regions;
    }

    [JsonObject("Region")]
    public class Region
    {
        [JsonProperty("type")]
        public string type;
        [JsonProperty("words_block_count")]
        public int blockCount;
        [JsonProperty("words_block_list")]
        public List<Block> Blocks;
    }

    [JsonObject("Block")]
    public class Block
    {
        [JsonProperty("words")]
        public string Words;
        [JsonProperty("rows")]
        public List<int> Rows;
        [JsonProperty("columns")]
        public List<int> Columns;
    }


}
