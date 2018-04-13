using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KTMSMQ_SENDER
{
    class SNRInfo
    {

        public class CSNRInfo
        {

            public string Cash_Info { get; set; }
            public int LEVEL1_COUNT { get; set; }
            public int LEVEL2_COUNT { get; set; }
            public int LEVEL3_COUNT { get; set; }
            public int LEVEL4_COUNT { get; set; }

            public string OperationTime { get; set; }
            public string Operationtype { get; set; }
            public CLevelSection[] LevelSection { get; set; }
        }

        public class SETBANKNOTE_TYPES
        {

            public int usNumOfNoteTypes { get; set; }
            public BANKNOTE_TYPES[] OFBANKNOTE { get; set; }
    }
         
        public class  BANKNOTE_TYPES
        {

             
            public int UsNoteID { get; set; } 
            public string cCurrencyID { get; set; }
            public int ulValues { get; set; }
            public int usRelease { get; set; }
            public int BConfigured { get; set; }

            

        }

        public class CLevelSection
        {

            public string LavelSectionName { get; set; }
            public int Index { get; set; }
            public string Currency { get; set; }
            public int Value { get; set; }
            public int Release { get; set; }
            public int NoteID { get; set; }
            public int Level { get; set; }
            public string SerialNumber { get; set; }
            public string ImageFile { get; set; }

        }

        public class IniFile
        {
            string Path;
            string EXE = Assembly.GetExecutingAssembly().GetName().Name;

            [DllImport("kernel32")]
            static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

            [DllImport("kernel32")]
            static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

            public IniFile(string IniPath = null  )
            {
                Path = new FileInfo(IniPath + ".ini").FullName.ToString();
            }

            public string Read(string Key, string Section = null)
            {
                var RetVal = new StringBuilder(255);
                GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
                return RetVal.ToString();
            }

            public void Write(string Key, string Value, string Section = null)
            {
                WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
            }

            public void DeleteKey(string Key, string Section = null)
            {
                Write(Key, null, Section ?? EXE);
            }

            public void DeleteSection(string Section = null)
            {
                Write(null, null, Section ?? EXE);
            }

            public bool KeyExists(string Key, string Section = null)
            {
                return Read(Key, Section).Length > 0;
            }
        }

        public class CIM_CASHINSTATUS
        {
            
            public int wStatus { get; set; }
            public int usNumOfRefused { get; set; }
            public int usNumOfNoteNumbers { get; set; }
          
            public lppNoteNumber[] NoteNumber  { get; set; }

    }
        public class lppNoteNumber
        {
            public int usNoteID { get; set; }
            public int ulCount { get; set; }
            public int Value { get; set; }
            public string Currency { get; set; }
            public int BConfigured { get; set; } 
            public string lpszSerialNumber { get; set; }
            public string lpszImageFileName { get; set; }

        }

        public class NoteIDValue
        {
            public int Value { get; set; }
            public string Currency { get; set; }
            public int usNoteID { get; set; }
            public int BConfigured { get; set; }

        }



        public class NoteSerialNumberINFO
        {
            public int usCount { get; set; }
            public int usreturn { get; set; } 
            public ImageAndSerialNumber[]    NoteItemsSerialsImage{ get; set; }

    }

        public class ImageAndSerialNumber
            {
            public int usLevel { get; set; }
            public int usNoteID { get; set; }
            public string lpszSerialNumber { get; set; }
            public string lpszImageFileName { get; set; }

        }



        }
}
