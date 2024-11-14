using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedRegsConverter
{
    // gcc -fstrict-volatile-bitfields
    class StructureBits
    {
        public short Offset;
        public short bitWidth;
        public string Name;
        public string Comment;
        public override string ToString()
        {
            return Name;
        }
        public static bool operator ==(StructureBits c1, StructureBits c2)
        {
            return c1.Name.Equals(c2.Name);
        }
        public static bool operator !=(StructureBits c1, StructureBits c2)
        {
            return !c1.Name.Equals(c2.Name);
        }
        public override bool Equals(object obj)
        {
            return this.Name.Equals(((StructureBits)obj).Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
    class StructureRegister
    {
        public string Name;
        public string Definition;
        public string Comment;
        public List<StructureBits> Bitfields;
        public override string ToString()
        {
            return Name;
        }
        public static bool operator ==(StructureRegister c1, StructureRegister c2)
        {
            return c1.Name.Equals(c2.Name);
        }
        public static bool operator !=(StructureRegister c1, StructureRegister c2)
        {
            return !c1.Name.Equals(c2.Name);
        }
        public override bool Equals(object obj)
        {
            return this.Name.Equals(((StructureRegister)obj).Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
    class StructureC
    {
        public string Name;
        public string TypeDefName;
        public string? Brief;
        public List<StructureRegister> Registers;
        public override string ToString()
        {
            return Name;
        }
        public static bool operator ==(StructureC c1, StructureC c2)
        {
            if (c1 is null && c2 is null)
                return true;
            if (c1 is null || c2 is null)
                return false;
            return c1.Name.Equals(c2?.Name);
        }
        public static bool operator !=(StructureC c1, StructureC c2)
        {
            if (c1 is null && c2 is null)
                return false;
            if (c1 is null || c2 is null)
                return true;
            return !c1.Name.Equals(c2?.Name);
        }
        public override bool Equals(object obj)
        {
            return this.Name.Equals(((StructureC)obj).Name);
        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        public bool HaveMatchingReg(string reg)
        {
            if (reg.Contains('0'))
            {
                //try remove 0 first.
                //CAN TI0R = TIR
                var nozeroName = reg.Replace("0", "");

                if (Registers[0].Name.StartsWith(nozeroName) || Registers[0].Name.EndsWith(nozeroName))
                    return true;
                //S0DR
                var splitted = reg.Split('0');
                //S + DR
                foreach (var partRegName in splitted)
                {
                    //DR - needed
                    if (partRegName.Length > 1)
                    {
                        //maybe dont search in all of them only 1st one?
                        //foreach (var register in Registers)
                            if (Registers[0].Name.StartsWith(partRegName) || Registers[0].Name.EndsWith(partRegName))
                                return true;
                    }
                }
            }
            else
            {
                foreach (var register in Registers)
                    if (register.Name.StartsWith(reg) || register.Name.EndsWith(reg))
                        return true;
            }
            return false;
        }
    }

    class StructureCperipherialName
    {
        public StructureC StructC;
        public string Name; 
        
        public override string ToString()
        {
            return $"{Name} {{{StructC.Name}}}" ;
        }
    }
}
