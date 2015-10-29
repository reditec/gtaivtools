using System;
using System.Collections.Generic;
using System.Text;

namespace RageLib.Common
{
    public class KeyUtilRDR : KeyUtil
    {
        public override string ExecutableName
        {
            get { return "default.xex"; }
        }

        protected override string[] PathRegistryKeys
        {
            get
            {
                return new[]
                           {
                              "" //Console game --> No registry path
                           };
            }
        }

        protected override uint[] SearchOffsets
        {
            get
            {
                return new uint[]
                           {
                               //EFIGS EXEs
                               0xFBA078 /* GOTY Edition - I don't know the version - My xextool says v0.0.0.12 (probably 1.02) */,
                               
                           };
            }
        }
    }
}
