using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace EmbeddedRegsConverter
{
    public partial class Form1 : Form
    {
        string outputHeader;
        Dictionary<string, uint> variables;
        Dictionary<uint, StructureCperipherialName> peripherialAdresses;

        string ExportVariables;

        Regex regexAddress = new Regex(@"#define\s+([A-Za-z_0-9]+)\s+([x0-9ABCDEFabcdef]+)(?=[Uu]|\s|$)");
        Regex regexBasePlusAddress = new Regex(@"#define\s+([A-Za-z0-9_]+)\s+\(\s*([A-Za-z0-9_]+)\s*\+\s*([A-Za-z0-9_]+)\s*\)");
        Regex regeVariable = new Regex(@"#define\s+([A-Za-z0-9_]+)\s+\(\s*\(\s*([A-Za-z0-9_]+)\s*\*\)\s*([A-Za-z0-9_]+)");
        Regex regeRedefine = new Regex(@"#define\s+([A-Za-z_0-9]+)\s+([A-Za-z_0-9]+)");
        Regex regexMaskAndPos = new Regex(@"#define\s+([A-Za-z_0-9]+)\s+\(\s*([x0-9ABCDEFabcdef]+)(?=[Uu\s)]|$)");

        Dictionary<string, StructureC> structures;
        string structSuffix;
        XmlDocument svdfile;

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
            peripherialAdresses = new Dictionary<uint, StructureCperipherialName>();

            StreamReader streamReader = new StreamReader(hfile);
            structures = new Dictionary<string, StructureC>();
            structSuffix = textBoxSuffix.Text;
            string? brief = null;

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

                            StructureCperipherialName pname = new StructureCperipherialName();
                            pname.Name = name;
                            string structdefname = GetNoTypeDefName(type);
                            if (structures.ContainsKey(structdefname))
                                pname.StructC = structures[structdefname];

                            uint? address = ParseUInt(matches[0].Groups[3].Value);
                            if (address != null)
                            {
                                ExportVariables += $"{name} = {address};{Environment.NewLine}";
                                if (!peripherialAdresses.ContainsKey((uint)address) && pname.StructC != null)
                                    peripherialAdresses.Add((uint)address, pname);
                            }
                            else
                            {
                                if (variables.ContainsKey(matches[0].Groups[3].Value))
                                {
                                    address = variables[matches[0].Groups[3].Value];
                                    ExportVariables += $"{name} = 0x{address:X};{Environment.NewLine}";
                                    if (!peripherialAdresses.ContainsKey((uint)address) && pname.StructC != null)
                                        peripherialAdresses.Add((uint)address, pname);
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
                else if (checkBoxStructs.Checked && line.StartsWith("typedef struct"))
                {
                    string? structline = null;
                    StructureC newstruct = new StructureC();
                    newstruct.Brief = brief;
                    brief = null;
                    do
                    {
                        structline = streamReader.ReadLine();
                    } while (ParseStructLine(structline, ref newstruct));
                    newstruct.TypeDefName = structline;
                    structures.Add(newstruct.Name, newstruct);
                    outputHeader += "//" + structline + Environment.NewLine;
                }
                else if (checkBoxStructs.Checked && line.Contains("@brief"))
                {
                    brief = line;
                    outputHeader += line + Environment.NewLine;
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

        string GetNoTypeDefName(string input)
        {
            string name = input.Trim();
            if (name.StartsWith('{'))
                name = name.Substring(1).TrimStart();
            if (name.StartsWith('}'))
                name = name.Substring(1).TrimStart();
            var nameEnd = name.LastIndexOf(structSuffix);
            if (nameEnd >= 0)
                name = name.Substring(0, nameEnd);
            if (name.EndsWith('_'))
                name = name.Substring(0, name.Length - 1);
            return name;
        }

        bool ParseStructLine(string? input, ref StructureC structC)
        {
            if (input == null)
                return false;
            input = input.TrimStart();
            if (input.StartsWith("{"))
                return true;
            else if (input.StartsWith("}"))
            {
                //end if stuct
                //remove '{'
                structC.Name = GetNoTypeDefName(input);
                return false;
            }
            else if (input.Contains(";"))
            {
                //variables list
                if (structC.Registers == null)
                    structC.Registers = new List<StructureRegister>();
                StructureRegister newStructValue = new StructureRegister();

                var splitted = input.Split(';');

                if (splitted.Length > 0)
                {
                    var variable = splitted[0].Trim().Split(' ');
                    //last object is variable name, others is type def
                    if (variable != null && variable.Length > 0)
                    {
                        newStructValue.Name = variable[variable.Length - 1];
                        newStructValue.Definition = "";
                        for (int i = 0; i < variable.Length - 1; i++)
                        {
                            newStructValue.Definition += variable[i] + " ";
                        }
                    }
                }
                if (splitted.Length > 1)
                {
                    newStructValue.Comment = splitted[1];
                    // recover ; if there was some in comments
                    for (int i = 2; i < splitted.Length; i++)
                    {
                        newStructValue.Comment += ";";
                        newStructValue.Comment += splitted[i];
                    }
                }
                if (splitted.Length > 0)
                    structC.Registers.Add(newStructValue);
            }
            return true;
        }

        void ParseStructBitDefinition(XmlNode node)
        {
            foreach (XmlNode peripherial in node.ChildNodes)
            {
                var name = peripherial.SelectSingleNode("name");
                uint? baseAddress = ParseUInt(peripherial.SelectSingleNode("baseAddress").InnerText);
                StructureC structure = null;
                if (baseAddress != null && peripherialAdresses.ContainsKey((uint)baseAddress))
                {
                    structure = peripherialAdresses[(uint)baseAddress].StructC;
                }
                else if (name != null && structures.ContainsKey(name.InnerText))
                {
                    structure = structures[name.InnerText];
                }
                else
                {
                    name = peripherial.SelectSingleNode("groupName");
                    if (name != null && structures.ContainsKey(name.InnerText))
                    {
                        structure = structures[name.InnerText];
                    }
                }
                if (structure != null)
                    ParseRegisters(structure, peripherial, 0);
            }
        }

        int ParseRegisters(StructureC structc, XmlNode parent, int startindex)
        {
            var regs = parent.SelectNodes("registers/register");
            int previousindex = 0;
            List<StructureC> matchingstructs = null;
            bool nesting = startindex == 0;
            if (nesting)
            {
                matchingstructs = FindsStructMatching(structc.Name);
            }
            for (int regIndex = startindex; regIndex < regs.Count; regIndex++)
            {
                var registerName = regs[regIndex].SelectSingleNode("name");
                if (registerName != null)
                {
                    string regNamestr = registerName.InnerText;
                    int index = FindRegIndex(structc, regNamestr);
                    if (nesting == false)
                    {
                        //in already nested structure name can be modified
                        if (index < 0 && regNamestr.Contains('0'))
                        {
                            //try remove 0 from name
                            regNamestr = registerName.InnerText.Replace("0", "");
                            index = FindRegIndex(structc, regNamestr);
                            //or try find part of name
                            if (index < 0)
                            {
                                //S0CR - get 'CR' from that line
                                //S0M0AR - get M0AR
                                var names = registerName.InnerText.Split('0', 2);
                                regNamestr = names[0];
                                if (names.Length > 1 && names[1].Length > regNamestr.Length)
                                    regNamestr = names[1];

                                index = FindRegIndex(structc, regNamestr);
                            }
                        }
                    }

                    if (index >= 0)
                    {
                        var stuctReg = structc.Registers[index];
                        var fields = regs[regIndex].SelectNodes("fields/field");
                        if (fields != null && fields.Count > 0)
                        {
                            var bitfields = new List<StructureBits>();
                            foreach (XmlNode field in fields)
                            {
                                StructureBits bits = new StructureBits();
                                bits.Offset = short.Parse(field.SelectSingleNode("bitOffset").InnerText);
                                bits.bitWidth = short.Parse(field.SelectSingleNode("bitWidth").InnerText);
                                bits.Name = field.SelectSingleNode("name").InnerText;
                                var access = field.SelectSingleNode("access");
                                if (access != null)
                                    bits.Comment = $"{field.SelectSingleNode("description")?.InnerText}, {access.InnerText}";
                                else
                                    bits.Comment = $"{field.SelectSingleNode("description")?.InnerText}";
                                bits.Comment = Regex.Replace(bits.Comment, @"\s+", " ");
                                bitfields.Add(bits);
                            }
                            if (stuctReg.Bitfields == null || stuctReg.Bitfields.Count < bitfields.Count)
                                stuctReg.Bitfields = bitfields;
                        }
                        //structc.Registers[index] = stuctReg;
                    }
                    else if (nesting)
                    {
                        //nested structure, example:
                        //DMA S0CR = DMAstream0.CR
                        for (int i = 0; matchingstructs != null && i < matchingstructs.Count; i++)
                        {
                            if (matchingstructs[i].HaveMatchingReg(regNamestr))
                            {
                                regIndex = ParseRegisters(matchingstructs[i], parent, regIndex);
                                matchingstructs.RemoveAt(i);
                            }
                        }
                    }
                    else { return regIndex; }
                    previousindex = index;
                }
            }
            return -1;
        }

        int FindRegIndex(StructureC structc, string name)
        {
            for (int i = 0; i < structc.Registers.Count; i++)
            {
                if (structc.Registers[i].Name == name)
                    return i;
            }
            for (int i = 0; i < structc.Registers.Count; i++)
            {
                //more wide search
                if (structc.Registers[i].Name.StartsWith(name) || structc.Registers[i].Name.EndsWith(name) /*|| name.StartsWith(structc.Registers[i].Name)*/)
                    return i;
            }
            if (name.Contains('_'))
            {
                //try search no _
                var nameNoUnderscore = name.Replace("_", "");
                for (int i = 0; i < structc.Registers.Count; i++)
                {
                    //more wide search
                    if (structc.Registers[i].Name.StartsWith(nameNoUnderscore) || structc.Registers[i].Name.EndsWith(nameNoUnderscore))
                        return i;
                }
                //try search with ending after _
                var nameSplit = name.Split('_');
                var nameEnding = nameSplit[nameSplit.Length - 1];
                for (int i = 0; i < structc.Registers.Count; i++)
                {
                    //more wide search
                    if (structc.Registers[i].Name.StartsWith(nameEnding) || structc.Registers[i].Name.EndsWith(nameEnding))
                        return i;
                }
            }
            return -1;
        }

        List<StructureC> FindsStructMatching(string partName)
        {
            //need to find any name matching structs
            var output = new List<StructureC>();
            var liststructs = structures.Values.ToList();
            for (int i = 0; i < liststructs.Count; i++)
            {
                string name = liststructs[i].Name;
                if (name != partName && name.Contains(partName) /* || partName.Contains(name)*/)
                {
                    output.Add(liststructs[i]);
                }
            }
            return output;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {

                svdfile = new XmlDocument();
                svdfile.Load(openFileDialog2.FileName);
                var node = svdfile.DocumentElement.SelectSingleNode("/device/peripherals");
                ParseStructBitDefinition(node);
                richHfile.Text = PrintStructures(richHfile.Text);
            }
        }

        string PrintStructures(string sourceHeader)
        {
            foreach (var structure in structures)
            {
                string structToText = "typedef struct" + Environment.NewLine + "{" + Environment.NewLine;
                //print registers
                foreach (var reg in structure.Value.Registers)
                {
                    bool bits = reg.Bitfields != null && reg.Bitfields.Count > 0;
                    if (bits)
                    {
                        structToText += "union {" + Environment.NewLine;
                    }

                    structToText += $"{reg.Definition} {reg.Name}; {reg.Comment}" + Environment.NewLine;
                    //print bitfields
                    if (bits)
                    {
                        structToText += "struct {" + Environment.NewLine;
                        var SortedBits = reg.Bitfields.OrderBy(o => o.Offset).ToList();
                        int bitsReservedByte = 0;
                        int bitsReservedID = 0;
                        foreach (var bit in SortedBits)
                        {
                            //add reserved bits
                            if (bitsReservedByte != bit.Offset)
                            {
                                int reservewidth = bit.Offset - bitsReservedByte;
                                structToText += $"{reg.Definition} reserved{bitsReservedID++} :{reservewidth};" + Environment.NewLine;
                                bitsReservedByte += reservewidth;
                            }
                            structToText += $"{reg.Definition} {bit.Name} :{bit.bitWidth}; /* {bit.Comment} */" + Environment.NewLine;
                            bitsReservedByte += bit.bitWidth;
                        }
                        structToText += "} " + reg.Name + "bits;" + Environment.NewLine + "};" + Environment.NewLine;
                    }
                }
                //print ending and replace in text
                structToText += structure.Value.TypeDefName;
                sourceHeader = sourceHeader.Replace("//" + structure.Value.TypeDefName, structToText);
            }
            return sourceHeader;
        }
    }
}