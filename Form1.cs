using System.Globalization;
using System.Text.RegularExpressions;

namespace EmbeddedRegsConverter
{
    public partial class Form1 : Form
    {
        string outputHeader;
        Dictionary<string, uint> variables;
        string ExportVariables;

        Regex regexAddress = new Regex(@"#define\s+([A-Za-z_0-9]+)\s+([x0-9ABCDEFabcdef]+)(?=[Uu]|\s|$)");
        Regex regexBasePlusAddress = new Regex(@"#define\s+([A-Za-z0-9_]+)\s+\(\s*([A-Za-z0-9_]+)\s*\+\s*([A-Za-z0-9_]+)\s*\)");
        Regex regeVariable = new Regex(@"#define\s+([A-Za-z0-9_]+)\s+\(\s*\(\s*([A-Za-z0-9_]+)\s*\*\)\s*([A-Za-z0-9_]+)");
        Regex regeRedefine = new Regex(@"#define\s+([A-Za-z_0-9]+)\s+([A-Za-z_0-9]+)");
        Regex regexMaskAndPos = new Regex(@"#define\s+([A-Za-z_0-9]+)\s+\(\s*([x0-9ABCDEFabcdef]+)(?=[Uu\s)]|$)");

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ScanHfile(openFileDialog1.FileName);
            }
        }


        void ScanHfile(string hfile)
        {
            outputHeader = "";
            variables = new Dictionary<string, uint>();
            ExportVariables = "SECTIONS {" + Environment.NewLine;


            StreamReader streamReader = new StreamReader(hfile);
            while (!streamReader.EndOfStream)
            {
                //all defines located in fifo order, so it is easy to parse them just as they come
                string? line = streamReader.ReadLine();
                if (line == null) break;
                line = line.TrimStart();
                if (line.StartsWith("#define"))
                {
                    var linewithcomment = line.Split(new[] { "/*", "//" }, StringSplitOptions.None);

                    if (linewithcomment[0].Contains("*"))
                    {
                        //Variable reference, replace it with our define
                        var matches = regeVariable.Matches(line);
                        if (matches.Count > 0 && matches[0].Groups.Count > 3)
                        {
                            //variable name, type, address or link
                            string name = matches[0].Groups[1].Value;
                            string type = matches[0].Groups[2].Value;
                            //replace define with extern variable
                            outputHeader += (checkVolatile.Checked ? "extern volatile " : "extern ") + type + " " + name + ";" + Environment.NewLine;

                            uint? address = ParseUInt(matches[0].Groups[3].Value);
                            if (address != null)
                                ExportVariables += $"{name} = {address};{Environment.NewLine}";
                            else
                            {
                                if (variables.ContainsKey(matches[0].Groups[3].Value))
                                {
                                    address = variables[matches[0].Groups[3].Value];
                                    ExportVariables += $"{name} = 0x{address:X};{Environment.NewLine}";
                                }
                                //todo else error
                            }
                        }
                        else
                            outputHeader += line + Environment.NewLine;
                    }
                    else if (linewithcomment[0].Contains('+'))
                    {
                        //todo add switch case cycle that changes between names and numbers unlimited times
                        uint? offset;
                        //address offset
                        var matches = regexBasePlusAddress.Matches(line);
                        if (matches.Count > 0 && matches[0].Groups.Count > 3)
                        {
                            offset = ParseUInt(matches[0].Groups[3].Value);
                            if (offset == null)
                            {
                                //name, offset, variable
                                offset = ParseUInt(matches[0].Groups[2].Value);
                                if (offset != null && variables.ContainsKey(matches[0].Groups[3].Value))
                                {
                                    offset += variables[matches[0].Groups[3].Value];
                                    variables.Add(matches[0].Groups[1].Value, (uint)offset);
                                }
                            }
                            else
                            {
                                //name, variable, offset
                                if (variables.ContainsKey(matches[0].Groups[2].Value))
                                {
                                    offset += variables[matches[0].Groups[2].Value];
                                    variables.Add(matches[0].Groups[1].Value, (uint)offset);
                                }
                            }
                        }
                        //add it to output
                        outputHeader += line + Environment.NewLine;
                    }
                    else
                    {
                        //pure address
                        var matches = regexAddress.Matches(line);
                        if (matches.Count > 0 && matches[0].Groups.Count > 2)
                        {
                            uint? val = ParseUInt(matches[0].Groups[2].Value);
                            if (val != null)
                                variables.Add(matches[0].Groups[1].Value, (uint)val);
                        }
                        else
                        {
                            //re-definition
                            matches = regeRedefine.Matches(line);
                            if (matches.Count > 0 && matches[0].Groups.Count > 2)
                            {
                                if (variables.ContainsKey(matches[0].Groups[2].Value))
                                    variables.Add(matches[0].Groups[1].Value, variables[matches[0].Groups[2].Value]);
                            }
                        }
                        //add it to output
                        outputHeader += line + Environment.NewLine;
                    }


                }
                else
                {
                    //just add it to output
                    outputHeader += line + Environment.NewLine;
                }
            }
            richHfile.Text = outputHeader;
            richLinkerFile.Text = ExportVariables + "}";
        }

        uint? ParseUInt(string input)
        {
            uint? n = null;
            try
            {
                if (input.StartsWith("0x"))
                {
                    n = UInt32.Parse(input.Substring(2).Replace('U', ' ').Replace('u', ' '), NumberStyles.HexNumber);
                }
                else
                {
                    n = UInt32.Parse(input);
                }
            }
            catch (FormatException) { }
            return n;
        }
    }
}